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
                        e.AuditCycle != null 
                        && e.AuditCycle.OrganizationID == filters.OrganizationID
                    );
            }

            if (filters.AuditCycleID != null && filters.AuditCycleID != Guid.Empty)
            {
                items = items
                    .Where(e => e.AuditCycleID == filters.AuditCycleID);
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
                            .Where(aa => 
                                (aa.Auditor.FirstName != null && aa.Auditor.FirstName.ToLower().Contains(filters.Text))
                                || (aa.Auditor.MiddleName != null && aa.Auditor.MiddleName.ToLower().Contains(filters.Text))
                                || (aa.Auditor.LastName != null && aa.Auditor.LastName.ToLower().Contains(filters.Text))
                            ).Any())
                        || (e.AuditStandards != null && e.AuditStandards
                            .Where(ads => 
                                (ads.Standard.Name != null && ads.Standard.Name.ToLower().Contains(filters.Text))
                                || (ads.Standard.Description != null && ads.Standard.Description.ToLower().Contains(filters.Text))
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

            var hasChanges = false;
            foreach (var item in items)
            {
                // Agregando la última hora del día
                var endDate = item.EndDate.Value.Date.AddDays(1).AddSeconds(-1);
                var hasInternalChanges = false; // para solo llamar una vez a .Update

                if (item.Status == AuditStatusType.Confirmed && 
                    item.StartDate <= DateTime.UtcNow && endDate >= DateTime.UtcNow)
                {
                    item.Status = AuditStatusType.InProcess;
                    //_repository.Update(item);
                    hasInternalChanges = true;
                    hasChanges = true;
                }

                if ((item.Status == AuditStatusType.Confirmed || item.Status == AuditStatusType.InProcess) &&
                    endDate < DateTime.UtcNow)
                {   
                    item.Status = AuditStatusType.Finished;
                    //_repository.Update(item);
                    hasInternalChanges = true;
                    hasChanges = true;
                }

                if (hasInternalChanges) _repository.Update(item);
            }

            if (hasChanges)
            {
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
                _repository.Update(item);
                _repository.SaveChanges();
            }

            return item;
        } // GetAsync

        public async Task<Audit> AddAsync(Audit item)
        {
            // Validations
            if (item.AuditCycleID == null || item.AuditCycleID == Guid.Empty)
                throw new BusinessException("Must first assign an audit cycle");

            var _auditCycleRepository = new AuditCycleRepository();
            var auditCycle = await _auditCycleRepository.GetAsync(item.AuditCycleID)
                ?? throw new BusinessException("The audit cycle associated is not found");

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
                    FileRepository.DeleteDirectory($"~/files/organizations/{i.AuditCycle.OrganizationID}/Cycles/{i.AuditCycle.ID}/{i.ID}");
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

            if (item.StartDate >= item.EndDate)
                throw new BusinessException("The start date must be less than the end date");

            // - Que las fechas de auditoria no se translapen con otra auditoria del mismo ciclo

            // - Validar que los auditores no estén programados en otra auditoria en la misma fecha
            var hasAuditorBusy = false;
            foreach (var auditor in foundItem.AuditAuditors)
            { 
                if (await _repository.HasAuditorAnAudit(auditor.ID, item.StartDate.Value, item.EndDate.Value, item.ID))
                {
                    hasAuditorBusy = false;
                }
            }
            if (hasAuditorBusy)
                throw new BusinessException("At least one auditor is assigned to another audit event");

            // Assigning values

            //if (item.Status == AuditStatusType.Nothing)
            //    item.Status = AuditStatusType.Scheduled;

            foundItem.Description = item.Description;
            foundItem.StartDate = item.StartDate;
            foundItem.EndDate = item.EndDate;
            foundItem.HasWitness = item.HasWitness;
            foundItem.Status = item.Status == AuditStatusType.Nothing
                ? AuditStatusType.Scheduled
                : item.Status;
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

        public async Task<bool> HasAuditorAnAudit(Guid auditorID, DateTime startDate, DateTime endDate, Guid? auditExceptionID)
        {
            return await _repository.HasAuditorAnAudit(auditorID, startDate, endDate, auditExceptionID);
        } // HasAuditorAnAudit

        // PRIVATE

        private void CheckMinimalAuditCycleDocumentation(AuditCycle auditCycle)
        {
            if (auditCycle.AuditCycleDocuments == null || auditCycle.AuditCycleDocuments.Count == 0)
                throw new BusinessException("The audit cycle don't have any document");

            bool haveAppForm = false;
            bool haveADC = false;
            bool haveProposal = false;
            bool haveContract = false;
            bool haveAuditProgramme = false;

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

            haveAuditProgramme = auditCycle.AuditCycleDocuments
                .Where(acd =>
                    acd.DocumentType == AuditCycleDocumentType.AuditProgramme
                    && acd.Status == StatusType.Active)
                .Any();

            if (!haveAppForm 
                || !haveADC 
                || !haveProposal 
                || !haveContract 
                || !haveAuditProgramme
            )
                throw new BusinessException("Must have at last a App form, an ADC, a Proposal, a Contract and a Confirmation Letter active document");

        } // CheckMinimalAuditCycleDocumentation
    }
}