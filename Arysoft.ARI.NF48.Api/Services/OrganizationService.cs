using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.IO;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using Arysoft.ARI.NF48.Api.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class OrganizationService : IDisposable
    {
        private readonly OrganizationRepository _organizationRepository;
        private readonly AuditRepository _auditRepository;

        // CONSTRUCTOR

        public OrganizationService()
        {
            _organizationRepository = new OrganizationRepository();
            _auditRepository = new AuditRepository();
        }

        // METHODS

        public async Task<PagedList<Organization>> GetsAsync(OrganizationQueryFilters filters)
        {
            // Query sin ejecutar: todo lo siguiente se traduce a SQL
            var items = _organizationRepository.Query();

            // Filters

            if (filters.StandardID != null && filters.StandardID != Guid.Empty)
            {
                var standardID = filters.StandardID.Value;
                items = items.Where(e => e.OrganizationStandards
                    .Any(os => os.StandardID == standardID
                        && os.Status != StatusType.Nothing
                    ));
            }

            if (filters.Folio != null)
            {
                var folio = filters.Folio.Value;
                items = items.Where(e =>
                    e.Folio == folio ||
                    e.FolderFolio == folio
                );
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                // Sin Strings.EscapeLikeValue: EF6 ya escapa %, _ y [ en Contains,
                // escapar a mano los duplicaría y rompería búsquedas como "100%".
                var text = filters.Text.ToLower().Trim();
                items = items.Where(e =>
                    (e.Name != null && e.Name.ToLower().Contains(text))
                    || e.Companies.Any(c =>
                        (c.Name != null && c.Name.ToLower().Contains(text))
                        || (c.LegalEntity != null && c.LegalEntity.ToLower().Contains(text))
                    )
                    || e.Sites.Any(s =>
                        (s.Address != null && s.Address.ToLower().Contains(text))
                        || (s.Country != null && s.Country.ToLower().Contains(text))
                    )
                    || (e.Website != null && e.Website.ToLower().Contains(text))
                    || (e.Phone != null && e.Phone.ToLower().Contains(text))
                    || (e.ExtraInfo != null && e.ExtraInfo.ToLower().Contains(text))
                );
            }

            if (filters.StandardBase != null && filters.StandardBase != StandardBaseType.Nothing)
            {
                var standardBase = filters.StandardBase.Value;
                items = items.Where(e => e.OrganizationStandards
                    .Any(os => os.Standard != null
                        && os.Standard.StandardBase == standardBase
                        && os.Status != StatusType.Nothing
                    ));
            }

            // Filtrado por alertas de ciclos de certificación
            if (filters.CertificateCycleAlert != null && filters.CertificateCycleAlert != OrganizationCertificateCycleAlertType.Nothing)
            {
                DateTime alertDate = DateTime.UtcNow;
                // EF6 no traduce AddYears/AddMonths sobre la columna: la aritmética se pasa
                // al lado del parámetro (EndDate - 1 año < hoy  <=>  EndDate < hoy + 1 año)
                DateTime limitOneYear = alertDate.AddYears(1);
                DateTime limitThreeMonths = alertDate.AddMonths(3);

                switch (filters.CertificateCycleAlert)
                {
                    case OrganizationCertificateCycleAlertType.Active:
                        items = items.Where(e => e.AuditCycles
                            .Any(ac => ac.Status == StatusType.Active)
                        );
                        break;

                    case OrganizationCertificateCycleAlertType.LastYear:
                        items = items.Where(e => e.AuditCycles
                            .Any(ac =>
                                ac.Status == StatusType.Active
                                && ac.EndDate < limitOneYear
                                && alertDate <= ac.EndDate
                            )
                        );
                        break;

                    case OrganizationCertificateCycleAlertType.LeftThreeMonths:
                        items = items.Where(e => e.AuditCycles
                            .Any(ac =>
                                ac.Status == StatusType.Active
                                && ac.EndDate < limitThreeMonths
                                && alertDate <= ac.EndDate
                            )
                        );
                        break;

                    case OrganizationCertificateCycleAlertType.Expired:
                        items = items.Where(e => e.AuditCycles
                            .Any(ac =>
                                ac.Status == StatusType.Active
                                && ac.EndDate < alertDate
                            )
                        );
                        break;
                }
            }

            // Status
            if (filters.Status != null && filters.Status != OrganizationStatusType.Nothing)
            {
                var status = filters.Status.Value;
                items = items.Where(e => e.Status == status);
            }
            else
            {
                // Un listado general omitiendo los registros de tipo Applicant
                items = items.Where(e => e.Status != OrganizationStatusType.Applicant);

                var includeDeleted = filters.IncludeDeleted ?? false;
                items = includeDeleted
                    ? items.Where(e => e.Status != OrganizationStatusType.Nothing)
                    : items.Where(e => e.Status != OrganizationStatusType.Nothing && e.Status != OrganizationStatusType.Deleted);
            }

            // Generando los valores calculados

            //foreach (var item in items)
            //{
            //    item.CertificatesValidityStatus = GetCertificatesValidityStatus(item);
            //}

            //// Calculated filters

            //if (filters.CertificatesValidityStatus != null && filters.CertificatesValidityStatus != CertificateValidityStatusType.Nothing)
            //{
            //    items = items.Where(e => e.CertificatesValidityStatus == filters.CertificatesValidityStatus);
            //}

            // Order (siempre debe haber OrderBy antes de Skip/Take en EF6; el ThenBy(ID)
            // desempata para que las páginas no repitan ni se salten filas)

            switch (filters.Order)
            {
                case OrganizationOrderType.Folio:
                    items = items.OrderBy(e => e.Folio).ThenBy(e => e.ID);
                    break;
                case OrganizationOrderType.Name:
                    items = items.OrderBy(e => e.Name).ThenBy(e => e.ID);
                    break;
                case OrganizationOrderType.Status:
                    items = items.OrderBy(e => e.Status)
                        .ThenBy(e => e.Name)
                        .ThenBy(e => e.ID);
                    break;
                case OrganizationOrderType.FolderFolio:
                    items = items.OrderBy(e => e.FolderFolio).ThenBy(e => e.ID);
                    break;
                //case OrganizationOrderType.CertificatesValidityStatus:
                //    items = items.OrderBy(e => e.CertificatesValidityStatus)
                //        .ThenBy(e => e.Name);
                //    break;
                case OrganizationOrderType.Updated:
                    items = items.OrderBy(e => e.Updated).ThenBy(e => e.ID);
                    break;
                case OrganizationOrderType.FolioDesc:
                    items = items.OrderByDescending(e => e.Folio).ThenBy(e => e.ID);
                    break;
                case OrganizationOrderType.NameDesc:
                    items = items.OrderByDescending(e => e.Name).ThenBy(e => e.ID);
                    break;
                case OrganizationOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status)
                        .ThenByDescending(e => e.Name)
                        .ThenBy(e => e.ID);
                    break;
                case OrganizationOrderType.FolderFolioDesc:
                    items = items.OrderByDescending(e => e.FolderFolio).ThenBy(e => e.ID);
                    break;
                //case OrganizationOrderType.CertificatesValidityStatusDesc:
                //    items = items.OrderByDescending(e => e.CertificatesValidityStatus)
                //        .ThenByDescending(e => e.Name);
                //    break;
                case OrganizationOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated).ThenBy(e => e.ID);
                    break;
                default:
                    items = items.OrderByDescending(e => e.Folio).ThenBy(e => e.ID);
                    break;
            }

            // Paging
            var pagedItems = PagedList<Organization>
                .Create(items, filters.PageNumber, filters.PageSize);
            await _organizationRepository.LoadListRelationsAsync(pagedItems);

            return pagedItems;
        } // GetsAsync

        /// <summary>
        /// Próxima auditoría de cada organización de la página, resuelta por lote
        /// (ver AuditRepository.GetNextAuditsByOrganizationsAsync). La llave es el ID de
        /// la organización. Los Audit viven en el contexto de _auditRepository, que se
        /// libera junto con el servicio: hay que mapearlos antes de eso.
        /// </summary>
        public async Task<Dictionary<Guid, Audit>> GetNextAuditsAsync(IEnumerable<Organization> organizations)
        {
            var ids = organizations.Select(o => o.ID).ToList();

            return await _auditRepository.GetNextAuditsByOrganizationsAsync(ids);
        } // GetNextAuditsAsync

        public async Task<Organization> GetAsync(Guid id)
        {
            var item = await _organizationRepository.GetAsync(id)
                ?? throw new BusinessException("Item not found");

            // item.CertificatesValidityStatus = GetCertificatesValidityStatus(item);

            return item;
        } // GetAsync

        public async Task<Organization> GetAsync(int folio)
        {
            var item = await _organizationRepository.GetAsync(folio)
                ?? throw new BusinessException("Item not found");

            // item.CertificatesValidityStatus = GetCertificatesValidityStatus(item);

            return item;
        } // GetAsync

        /// <summary>
        /// Obtiene un arreglo de enteros para MiniStatisticsCards con los siguientes 
        /// datos estadísticos:
        /// [0] - Cantidad de organizaciones activas
        /// [1] - Cantidad de organizaciones solicitantes
        /// [2] - Cantidad de organizaciones activas con al menos un certificado en 
        ///       su último año
        /// </summary>
        /// <returns></returns>
        public async Task<int[]> GetMiniStatisticDataAsync()
        {
            var items = _organizationRepository.Gets();

            var activesCount = items.Count(i =>
                i.Status == OrganizationStatusType.Active
            );

            var applicantsCount = items.Count(i =>
                i.Status == OrganizationStatusType.Applicant
            );

            // Revisar si en el AuditCycle activo esta su fecha de termino en
            // este año
            var activesWarningCount = items.Count(
                i => i.Status == OrganizationStatusType.Active
                    && i.AuditCycles != null
                    && i.AuditCycles
                        .Where(ac => ac.Status == StatusType.Active)
                        .Any(ac => ac.EndDate != null
                            && ac.EndDate.Value.AddYears(-1) < DateTime.UtcNow
                            && DateTime.UtcNow <= ac.EndDate.Value
                        )
            );

            return new int[] { activesCount, applicantsCount, activesWarningCount };
        } // GetMiniStatisticDataAsync

        // EDIT METHODS

        public async Task<Organization> AddAsync(Organization item)
        {
            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = OrganizationStatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries
            try
            {
                // Borrando carpetas de registros temporales
                var items = _organizationRepository.Gets()
                    .Where(o =>
                        o.UpdatedUser.ToUpper() == item.UpdatedUser.ToUpper() &&
                        o.Status == OrganizationStatusType.Nothing)
                    .ToList();
                if (items != null && items.Count > 0)
                {
                    foreach (var i in items)
                    {
                        FileRepository.DeleteDirectory($"~/files/organizations/{i.ID}");
                    }
                }

                await _organizationRepository.DeleteTmpByUserAsync(item.UpdatedUser);
                _organizationRepository.Add(item);
                await _organizationRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"OrganizationService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<Organization> UpdateAsync(Organization item)
        {
            var foundItem = await _organizationRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            if (await _organizationRepository.ExistOrganizationNameAsync(item.Name, foundItem.ID))
                throw new BusinessException("The organization name already exist");

            // Assigning values

            // Si cambió a estatus activo, generar el folio
            if (foundItem.Status < OrganizationStatusType.Active
                && item.Status == OrganizationStatusType.Active
                && foundItem.Folio == null)
            {
                // Validar si tiene la documentación minima
                //if (!HasMinimalDocumentation(foundItem))
                //    throw new BusinessException("The applicant does not have the minimum required documentation");

                // CheckMinimalDocumentation(foundItem); // xBlaze 20250312: Deshabilitado hasta que se suba información pasada

                // Generando folio
                foundItem.Folio = await _organizationRepository.GetNextFolioAsync();
            }

            foundItem.Name = item.Name;
            foundItem.LogoFile = item.LogoFile;
            foundItem.Website = item.Website;
            foundItem.Phone = item.Phone; ;
            foundItem.ExtraInfo = item.ExtraInfo;
            foundItem.FolderFolio = item.FolderFolio;
            foundItem.Status = foundItem.Status == OrganizationStatusType.Nothing && item.Status == OrganizationStatusType.Nothing
                ? OrganizationStatusType.Applicant
                : item.Status != OrganizationStatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Execute queries

            try
            {
                _organizationRepository.Update(foundItem);
                await _organizationRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"OrganizationService.UpdateAsync: {ex.Message}");
            }

            // foundItem.CertificatesValidityStatus = GetCertificatesValidityStatus(foundItem);

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Organization item)
        {
            var foundItem = await _organizationRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            // - Que no tenga certificados activos, who knows

            if (foundItem.Status == OrganizationStatusType.Deleted)
            {
                //! Considerar eliminar todas las asociaciones al registro antes de su eliminación tales como
                //  contacts, applications, sites, shifts, ... DONE! Se eliminan en cascada

                FileRepository.DeleteDirectory($"~/files/organizations/{foundItem.ID}");
                _organizationRepository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status < OrganizationStatusType.Inactive
                    ? OrganizationStatusType.Inactive
                    : OrganizationStatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _organizationRepository.Update(foundItem);
            }

            try
            {
                await _organizationRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"OrganizationService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync

        // PRIVATE

        //private static CertificateValidityStatusType GetCertificatesValidityStatus(Organization item)
        //{
        //    if (item != null 
        //        && item.Certificates != null 
        //        && item.Certificates
        //            .Where(i => i.Status != CertificateStatusType.Nothing)
        //            .Count() > 0
        //    )
        //    {
        //        foreach (var certificate in item.Certificates)
        //        {
        //            certificate.ValidityStatus = CertificateService.GetValidityStatus(certificate);
        //        }

        //        var anyInDanger = item.Certificates
        //            .Where(c =>
        //                c.Status == CertificateStatusType.Active
        //                && c.ValidityStatus == CertificateValidityStatusType.Danger)
        //            .Any();

        //        if (anyInDanger) return CertificateValidityStatusType.Danger;
        //        else {
        //            var anyInWarning = item.Certificates
        //                .Where(c =>
        //                    c.Status == CertificateStatusType.Active
        //                    && c.ValidityStatus == CertificateValidityStatusType.Warning)
        //                .Any();

        //            if (anyInWarning) return CertificateValidityStatusType.Warning;
        //        }

        //        return CertificateValidityStatusType.Success;
        //    }

        //    return CertificateValidityStatusType.Nothing;
        //} // GetCertificatesStatus

        private void CheckMinimalDocumentation(Organization item)
        {
            if (item.AuditCycles == null || item.AuditCycles.Count == 0)
                throw new BusinessException("You must create one audit cycle and add the minimal documentation");

            var auditCycle = item.AuditCycles.First(ac => ac.Status == StatusType.Active)
                ?? throw new BusinessException("Must have at least one active audit cycle");

            if (auditCycle.AuditCycleDocuments == null || auditCycle.AuditCycleDocuments.Count == 0)
                throw new BusinessException("The audit cycle don't have any document");

            bool haveAppForm = false;
            bool haveADC = false;
            bool haveProposal = false;
            bool haveContract = false;

            haveAppForm = auditCycle.AuditCycleDocuments
                .Where(acd =>
                    acd.DocumentType == AuditCycleDocumentType.AppForm
                    && acd.Status == StatusType.Active)
                .Any();

            haveADC = auditCycle.AuditCycleDocuments
                .Where(acd =>
                    acd.DocumentType == AuditCycleDocumentType.ADC
                    && acd.Status == StatusType.Active)
                .Any();

            haveProposal = auditCycle.AuditCycleDocuments
                .Where(acd =>
                    acd.DocumentType == AuditCycleDocumentType.Proposal
                    && acd.Status == StatusType.Active)
                .Any();

            haveContract = auditCycle.AuditCycleDocuments
                .Where(acd =>
                    acd.DocumentType == AuditCycleDocumentType.Contract
                    && acd.Status == StatusType.Active)
                .Any();

            if (!haveAppForm || !haveADC || !haveProposal || !haveContract)
                throw new BusinessException("Must have at last a App form, an ADC, a Proposal and a Contract active document");

            //return true;
        } // CheckMinimalDocumentation

        // DISPOSE

        public void Dispose()
        {
            _organizationRepository.Dispose();
            _auditRepository.Dispose();
        } // Dispose
    }
}