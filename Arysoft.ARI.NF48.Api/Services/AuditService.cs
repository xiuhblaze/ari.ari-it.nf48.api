using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.IO;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class AuditService
    {
        public readonly AuditRepository _repository;

        // CONSTRUCTOR

        public AuditService()
        {
            _repository = new AuditRepository();
        }

        // METHODS

        public PagedList<Audit> Gets(AuditQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty)
            {
                items = items
                    .Where(e => 
                        e.OrganizationID == filters.OrganizationID
                    );
            }

            if (filters.AuditCycleID != null && filters.AuditCycleID != Guid.Empty)
            {
                items = items
                    .Where(e => e.AuditStandards
                        .Where(asd => asd.AuditCycleID == filters.AuditCycleID)
                        .Any()
                    );
            }

            if (filters.AuditorID != null && filters.AuditorID != Guid.Empty)
            {
                items = items
                    .Where(e => e.AuditAuditors != null && e.AuditAuditors
                        .Where(aa => aa.AuditorID == filters.AuditorID)
                        .Any()
                    );
            }

            if (filters.StandardID != null && filters.StandardID != Guid.Empty)
            {
                items = items
                    .Where(e => e.AuditStandards != null && e.AuditStandards
                        .Where(ads => ads.StandardID == filters.StandardID)
                        .Any()
                    );
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items
                    .Where(e =>
                        (e.Description != null && e.Description.ToLower().Contains(filters.Text))
                        || (e.AuditAuditors != null && e.AuditAuditors
                            .Where(aa => aa.Auditor != null &&
                                ((aa.Auditor.FirstName != null && aa.Auditor.FirstName.ToLower().Contains(filters.Text))
                                || (aa.Auditor.MiddleName != null && aa.Auditor.MiddleName.ToLower().Contains(filters.Text))
                                || (aa.Auditor.LastName != null && aa.Auditor.LastName.ToLower().Contains(filters.Text)))
                            ).Any())
                        || (e.AuditStandards != null && e.AuditStandards
                            .Where(ads => ads.Standard != null && 
                                ((ads.Standard.Name != null && ads.Standard.Name.ToLower().Contains(filters.Text))
                                || (ads.Standard.Description != null && ads.Standard.Description.ToLower().Contains(filters.Text)))
                            ).Any())
                    );
            }

            if (filters.StartDate != null)
            {
                items = items.Where(e => e.EndDate >= filters.StartDate);
            }

            if (filters.EndDate != null)
            {
                items = items
                    .Where(e => e.StartDate <= filters.EndDate);
            }

            if (filters.Status != null && filters.Status != AuditStatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != AuditStatusType.Nothing)
                    : items.Where(e => e.Status != AuditStatusType.Nothing && e.Status != AuditStatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            {
                case AuditOrderType.Date:
                    items = items.OrderBy(e => e.StartDate);
                    break;
                case AuditOrderType.Status:
                    items = items.OrderBy(e => e.Status);
                    break;
                case AuditOrderType.DateDesc:
                    items = items.OrderByDescending(e => e.StartDate);
                    break;
                case AuditOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status);
                    break;
                default:
                    items = items.OrderBy(e => e.StartDate);
                    break;
            }

            // Validar si están en ejecución para ponerlos InProcess o Finished
            // y agregar una nota

            var hasChanges = false;
            var noteRepository = new BaseRepository<Note>();

            foreach (var item in items)
            {
                // Agregando la última hora del día
                var endDate = item.EndDate.Value.Date.AddDays(1).AddSeconds(-1);
                var hasInternalChanges = false; // para solo llamar una vez a .Update

                if (item.Status == AuditStatusType.Confirmed && 
                    item.StartDate <= DateTime.UtcNow && endDate >= DateTime.UtcNow)
                {
                    item.Status = AuditStatusType.InProcess;
                    hasInternalChanges = true;
                    hasChanges = true;
                }

                if ((item.Status == AuditStatusType.Confirmed || item.Status == AuditStatusType.InProcess) &&
                    endDate < DateTime.UtcNow)
                {   
                    item.Status = AuditStatusType.Finished;
                    hasInternalChanges = true;
                    hasChanges = true;
                }

                if (hasInternalChanges)
                {   
                    var note = new Note
                    {
                        ID = Guid.NewGuid(),
                        OwnerID = item.ID,
                        Text = $"Status changed to {item.Status.ToString().ToUpper()}",
                        UpdatedUser = "system",
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    };
                    noteRepository.Add(note);
                    
                    _repository.Update(item);
                }
            }

            if (hasChanges)
            {
                noteRepository.SaveChanges();
                _repository.SaveChanges();
            }   

            // Paging

            var pagedItems = PagedList<Audit>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Audit> GetAsync(Guid id)
        {
            // Validar si está en ejecución para ponerlo InProcess o Finished
            var item = await _repository.GetAsync(id);
            var hasChanges = false;
            var endDate = item.EndDate.Value.Date.AddDays(1).AddSeconds(-1);

            if (item.Status == AuditStatusType.Confirmed && 
                item.StartDate <= DateTime.UtcNow && endDate >= DateTime.UtcNow)
            {
                item.Status = AuditStatusType.InProcess;
                //_repository.Update(item);
                hasChanges = true;
            }

            // En confirmed ambién por si se registra una auditoria antigua
            if ((item.Status == AuditStatusType.Confirmed || item.Status == AuditStatusType.InProcess) 
                && endDate < DateTime.UtcNow)
            {
                item.Status = AuditStatusType.Finished;
                //_repository.Update(item);
                hasChanges = true;
            }

            if (hasChanges) 
            {
                var note = new Note
                {
                    OwnerID = item.ID,
                    Text = $"Status changed to {item.Status.ToString().ToUpper()}",
                    UpdatedUser = "system"
                };
                var noteService = new NoteService();
                await noteService.AddAsync(note);

                _repository.Update(item);
                _repository.SaveChanges();
            }

            return item;
        } // GetAsync

        public async Task<Audit> AddAsync(Audit item)
        {
            // Validations
            if (item.OrganizationID == null || item.OrganizationID == Guid.Empty)
                throw new BusinessException("Must first assign an organization");

            //var _auditCycleRepository = new AuditCycleRepository();
            //var auditCycle = await _auditCycleRepository.GetAsync(item.AuditCycleID)
            //    ?? throw new BusinessException("The audit cycle associated is not found");
            var _organizationRepository = new OrganizationRepository();
            var organization = await _organizationRepository.GetAsync(item.OrganizationID.Value)
                ?? throw new BusinessException("The organization associated is not found");

            // - Validar que el ciclo sea el activo o sea en el futuro

            // - Validar que se tenga la documentación del ciclo hasta Audit Programme
            // CheckMinimalAuditCycleDocumentation(auditCycle); // xBlaze 20250312: Deshabilitado hasta que se suba información pasada

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = AuditStatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries

            try {

                // Borrando carpetas de registros temporales
                var items = _repository.Gets()
                    .Where(e => e.UpdatedUser.ToUpper() == item.UpdatedUser.ToUpper().Trim()
                        && e.Status == AuditStatusType.Nothing);

                foreach (var i in items)
                { 
                    FileRepository.DeleteDirectory($"~/files/organizations/{i.OrganizationID}/Audits/{i.ID}");
                }

                await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
                _repository.Add(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<Audit> UpdateAsync(Audit item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            await ValidateAuditAsync(item, foundItem);            

            // Assigning values

            //if (item.Status == AuditStatusType.Nothing)
            //    item.Status = AuditStatusType.Scheduled;

            foundItem.Description = item.Description;
            foundItem.StartDate = item.StartDate;
            foundItem.EndDate = item.EndDate;            
            foundItem.IsMultisite = item.IsMultisite;
            foundItem.Days = item.Days;
            foundItem.IncludeSaturday = item.IncludeSaturday;
            foundItem.IncludeSunday = item.IncludeSunday;
            foundItem.ExtraInfo = item.ExtraInfo;
            foundItem.Status = foundItem.Status == AuditStatusType.Nothing && item.Status == AuditStatusType.Nothing
                ? AuditStatusType.Scheduled
                : item.Status != AuditStatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Execute queries

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Audit item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            if (foundItem.Status == AuditStatusType.Deleted)
            {
                var fileDirectory = $"~/files/organizations/{foundItem.OrganizationID}/Audits/{foundItem.ID}";
                FileRepository.DeleteDirectory(fileDirectory);

                _repository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status < AuditStatusType.Canceled
                    ? AuditStatusType.Canceled
                    : AuditStatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _repository.Update(foundItem);
            }

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync

        public async Task<bool> HasAuditorAnAudit(
            Guid auditorID, 
            DateTime startDate, 
            DateTime endDate, 
            Guid? auditExceptionID)
        {
            return await _repository.HasAuditorAnAudit(
                auditorID, 
                startDate, 
                endDate, 
                auditExceptionID);
        } // HasAuditorAnAudit

        public async Task<bool> IsAnyStandardStepAuditInAuditCycle(
            Guid auditCycleID,
            Guid standardID,
            AuditStepType step,
            Guid? auditExceptionID)
        {
            return await _repository.IsAnyStandardStepAuditInAuditCycle(
                auditCycleID,
                standardID,
                step,
                auditExceptionID);
        } // IsAnyEqualStandardStepAuditInAuditCycle

        public Audit GetNextAudit(
            Guid? ownerID,
            DateTime? initialDate,
            AuditNextAuditOwnerType owner)
        {
            return _repository.GetNextAudit(ownerID, initialDate, owner);
        } // GetNextAuditAsync

        // SITES

        public async Task AddSiteAsync(Guid id, Guid siteID)
        {
            await _repository.AddSiteAsync(id, siteID);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditService.AddSiteAsync: {ex.Message}");
            }
        } // AddSiteAsync

        public async Task DelSiteAsync(Guid id, Guid siteID)
        {
            await _repository.DelSiteAsync(id, siteID);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditService.DelSiteAsync: {ex.Message}");
            }
        } // DelSiteAsync

        // PRIVATE

        private async Task ValidateAuditAsync(Audit newItem, Audit currentItem)
        {
            // ----------------------------------------------------------------
            // - Que la auditoria no se translapen con otra
            //   auditoria del mismo ciclo, del mismo paso y en el mismo sitio
            // - Validar que los auditores no estén programados en otra
            //   auditoria en la misma fecha
            // - Si va a cambiar de Status,
            //   validar que tenga la información completa requerida por el Status nuevo como:
            // - Standars activos
            // - Auditores activos y con el Standard correcto 

            // TODO: AQUI VOY

            if (newItem.StartDate > newItem.EndDate)
                throw new BusinessException("The start date must be less than the end date");

            // NOTE: Esta validación queda en espera de mejora pues si se puede
            // programar una auditoria en el mismo ciclo y paso, pero no se puede
            // que sea del mismo sitio, parte que no se ha implementado aun. -xB 20250430
            // ACTUALIZACION: Creo que con validar si es multisitio ya es suficiente

            // #CHANGE_CYCLES: Evaluar esta validación una vez se implementen los cambios -xB 20251205
            //if (!(newItem.IsMultisite.HasValue && newItem.IsMultisite.Value))
            //{
            //    var isAnyAuditStandardStepProgrammed = false;

            //    foreach (var auditStandard in currentItem.AuditStandards
            //        .Where(aus => aus.Status == StatusType.Active))
            //    {
            //        if (await _repository.IsAnyStandardStepAuditInAuditCycle(
            //            currentItem.AuditCycleID,
            //            auditStandard.StandardID ?? Guid.Empty,
            //            auditStandard.Step ?? AuditStepType.Nothing,
            //            currentItem.ID))
            //        {
            //            isAnyAuditStandardStepProgrammed = true;
            //            break;
            //        }
            //    }
            //    if (isAnyAuditStandardStepProgrammed)
            //        throw new BusinessException("At last one standard step is already programmed in another audit");
            //} // IsMultisite

            var isAuditorBusy = false;
            foreach (var auditAuditor in currentItem.AuditAuditors
                .Where(aa => aa.Status == StatusType.Active))
            {
                if (await _repository.HasAuditorAnAudit(
                    auditAuditor.AuditorID ?? Guid.Empty, 
                    newItem.StartDate ?? DateTime.MinValue, 
                    newItem.EndDate ?? DateTime.MinValue, 
                    newItem.ID))
                {
                    isAuditorBusy = true;
                    break;
                }
            }
            if (isAuditorBusy)
                throw new BusinessException("At least one auditor is assigned to another audit event");

        } // ValidateAudit

        //private void CheckMinimalAuditCycleDocumentation(AuditCycle auditCycle)
        //{
        //    if (auditCycle.AuditCycleDocuments == null || auditCycle.AuditCycleDocuments.Count == 0)
        //        throw new BusinessException("The audit cycle don't have any document");

        //    bool haveAppForm = false;
        //    bool haveADC = false;
        //    bool haveProposal = false;
        //    bool haveContract = false;
        //    bool haveAuditProgramme = false;

        //    haveAppForm = auditCycle.AuditCycleDocuments
        //        .Where(acd =>
        //            acd.DocumentType == AuditCycleDocumentType.AppForm
        //            && acd.Status == StatusType.Active)
        //        .Any();

        //    haveADC = auditCycle.AuditCycleDocuments
        //        .Where(acd =>
        //            acd.DocumentType == AuditCycleDocumentType.ADC
        //            && acd.Status == StatusType.Active)
        //        .Any();

        //    haveProposal = auditCycle.AuditCycleDocuments
        //        .Where(acd =>
        //            acd.DocumentType == AuditCycleDocumentType.Proposal
        //            && acd.Status == StatusType.Active)
        //        .Any();

        //    haveContract = auditCycle.AuditCycleDocuments
        //        .Where(acd =>
        //            acd.DocumentType == AuditCycleDocumentType.Contract
        //            && acd.Status == StatusType.Active)
        //        .Any();

        //    haveAuditProgramme = auditCycle.AuditCycleDocuments
        //        .Where(acd =>
        //            acd.DocumentType == AuditCycleDocumentType.AuditProgramme
        //            && acd.Status == StatusType.Active)
        //        .Any();

        //    if (!haveAppForm 
        //        || !haveADC 
        //        || !haveProposal 
        //        || !haveContract 
        //        || !haveAuditProgramme
        //    )
        //        throw new BusinessException("Must have at last a App form, an ADC, a Proposal, a Contract and a Confirmation Letter active document");

        //} // CheckMinimalAuditCycleDocumentation
    }
}