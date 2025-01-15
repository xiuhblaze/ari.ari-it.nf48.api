using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class OrganizationService
    {
        private readonly OrganizationRepository _organizationRepository;

        // CONSTRUCTOR

        public OrganizationService()
        {
            _organizationRepository = new OrganizationRepository();
        }

        // METHODS

        public PagedList<Organization> Gets(OrganizationQueryFilters filters)
        {
            var items = _organizationRepository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e =>
                    (e.Name != null && e.Name.ToLower().Contains(filters.Text))
                    || (e.LegalEntity != null && e.LegalEntity.ToLower().Contains(filters.Text))
                    || (e.Website != null && e.Website.ToLower().Contains(filters.Text))
                    || (e.Phone != null && e.Phone.ToLower().Contains(filters.Text))
                );
            }

            if (filters.Status != null && filters.Status != OrganizationStatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != OrganizationStatusType.Nothing)
                    : items.Where(e => e.Status != OrganizationStatusType.Nothing && e.Status != OrganizationStatusType.Deleted);
            }

            // Generando los valores calculados

            foreach (var item in items)
            {
                item.CertificatesStatus = GetCertificatesStatus(item);
            }



            // Calculated filters

            if (filters.CertificatesStatus != null && filters.CertificatesStatus != OrganizationCertificatesStatusType.Nothing)
            {
                items = items.Where(e => e.CertificatesStatus == filters.CertificatesStatus);
            }

            // Order

            switch (filters.Order)
            {
                case OrganizationOrderType.Name:
                    items = items.OrderBy(e => e.Name);
                    break;
                case OrganizationOrderType.LegalEntity:
                    items = items.OrderBy(e => e.LegalEntity);
                    break;
                case OrganizationOrderType.Status:
                    items = items.OrderBy(e => e.Status)
                        .ThenBy(e => e.Name);
                    break;
                case OrganizationOrderType.CertificatesStatus:
                    items = items.OrderBy(e => e.CertificatesStatus)
                        .ThenBy(e => e.Name);
                    break;
                case OrganizationOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case OrganizationOrderType.NameDesc:
                    items = items.OrderByDescending(e => e.Name);
                    break;
                case OrganizationOrderType.LegalEntityDesc:
                    items = items.OrderByDescending(e => e.LegalEntity);
                    break;
                case OrganizationOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status)
                        .ThenByDescending(e => e.Name);
                    break;
                case OrganizationOrderType.CertificatesStatusDesc:
                    items = items.OrderByDescending(e => e.CertificatesStatus)
                        .ThenByDescending(e => e.Name);
                    break;
                case OrganizationOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.Name);
                    break;
            }

            // Paging

            var pagedItems = PagedList<Organization>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Organization> GetAsync(Guid id)
        { 
            var item = await _organizationRepository.GetAsync(id);

            item.CertificatesStatus = GetCertificatesStatus(item);

            return item;
        } // GetAsync

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
            // Validations

            // - Que el nombre no exista

            var foundItem = await _organizationRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Assigning values

            foundItem.Name = item.Name;
            foundItem.LegalEntity = item.LegalEntity;
            foundItem.LogoFile = item.LogoFile;
            foundItem.QRFile = item.QRFile;
            foundItem.Website = item.Website;
            foundItem.Phone = item.Phone;
            foundItem.COID = item.COID;
            foundItem.Status = item.Status == OrganizationStatusType.Nothing 
                ? OrganizationStatusType.New 
                : item.Status;
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
                //  contacts, applications, sites, shifts, ...
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

        private static OrganizationCertificatesStatusType GetCertificatesStatus(Organization item)
        {
            if (item.Certificates != null)
            {
                var anyInDanger = item.Certificates
                    .Where(c =>
                        c.Status == StatusType.Active
                        && c.ValidityStatus == CertificateValidityStatusType.Danger)
                    .Any();

                if (anyInDanger) return OrganizationCertificatesStatusType.Danger;
                else {
                    var anyInWarning = item.Certificates
                        .Where(c =>
                            c.Status == StatusType.Active
                            && c.ValidityStatus == CertificateValidityStatusType.Warning)
                        .Any();

                    if (anyInWarning) return OrganizationCertificatesStatusType.Warning;
                }

                return OrganizationCertificatesStatusType.Success;
            }

            return OrganizationCertificatesStatusType.Nothing;
        } // GetCertificatesStatus
    }
}