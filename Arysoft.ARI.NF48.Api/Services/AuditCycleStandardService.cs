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
    public class AuditCycleStandardService
    {
        private readonly AuditCycleStandardRepository _repository;

        // CONSTRUCTOR

        public AuditCycleStandardService()
        {
            _repository = new AuditCycleStandardRepository();
        }

        // METHODS

        public PagedList<AuditCycleStandard> Gets(AuditCycleStandardQueryFilters filters)
        { 
            var items = _repository.Gets();

            // Filters

            if (filters.AuditCycleID != null)
            {
                items = items.Where(e => e.AuditCycleID == filters.AuditCycleID);
            }

            if (filters.StandardID != null)
            {
                items = items.Where(e => e.StandardID == filters.StandardID);
            }

            if (filters.InitialStep != null)
            {
                items = items.Where(e => e.InitialStep == filters.InitialStep);
            }

            if (filters.CycleType != null)
            {
                items = items.Where(e => e.CycleType == filters.CycleType);
            }

            if (filters.Status != null && filters.Status != StatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != StatusType.Nothing)
                    : items.Where(e => e.Status != StatusType.Nothing && e.Status != StatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            {
                case AuditCycleStandardOrderType.StandardName:
                    items = items.OrderBy(e => e.Standard.Name);
                    break;
                case AuditCycleStandardOrderType.StandardNameDesc:
                    items = items.OrderByDescending(e => e.Standard.Name);
                    break;
                default:
                    items = items.OrderBy(e => e.Standard.Name);
                    break;
            }

            var pagedItems = PagedList<AuditCycleStandard>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<AuditCycleStandard> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<AuditCycleStandard> AddAsync(AuditCycleStandard item)
        { 
            // Validations

            if (item.AuditCycleID == Guid.Empty)
                throw new BusinessException("Audit cycle is required");

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries
            try
            {
                await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
                _repository.Add(item);
                await _repository.SaveChangesAsync(); // Async para esperar aquí a ver si sucede un error
            } 
            catch (Exception ex)
            {
                throw new BusinessException($"AuditCycleStandard.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<AuditCycleStandard> UpdateAsync(AuditCycleStandard item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // HACK: Validar el step de acuerdo al CycleType plano asignarlo automáticamente
            if (item.InitialStep == AuditStepType.Nothing)
                throw new BusinessException("An initial step is required");

            if (item.CycleType == AuditCycleType.Nothing)
                throw new BusinessException("A cycle type is required");
                        
            if (foundItem.Status == StatusType.Nothing) // Es es nuevo...
            {
                // Validar que traiga el standard
                if (item.StandardID == null || item.StandardID == Guid.Empty)
                    throw new BusinessException("A standard is required");
            }

            // - Que no haya una asignación en este ciclo del mismo standard
            if (await _repository.IsStandardInCycleAsync(foundItem.AuditCycleID, item.StandardID.Value, item.ID))
                throw new BusinessException("There is already a current standard in this cycle");

            // - Que no exista el standard en otro ciclo activo y si el ciclo no es activo,
            //   permitir agregarlo
            //   HACK: Actualizarlo para que no se sobrepongan las fechas del ciclo
            //   y de ahi permitirlo o no (xBlaze: 20250926)
            if (foundItem.Status != item.Status 
                && item.Status == StatusType.Active
                && foundItem.AuditCycle.Status == StatusType.Active
            )
            {
                if (await _repository.IsStandardActiveInOrganizationAnyAuditCycleAsync(foundItem.AuditCycle.OrganizationID, (Guid)item.StandardID, item.ID))
                    throw new BusinessException("There is already a current standard in another active cycle");
            }

            // Assigning values

            if (foundItem.Status == StatusType.Nothing) // Solo si es nuevo, se asigna el standard
            { 
                foundItem.StandardID = item.StandardID;
            }
            foundItem.InitialStep = item.InitialStep;
            foundItem.CycleType = item.CycleType;
            foundItem.Status = foundItem.Status == StatusType.Nothing && item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status != StatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync(); // Async para esperar aquí a ver si sucede un error

            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditCycleStandard.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(AuditCycleStandard item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            if (foundItem.Status == StatusType.Deleted)
            {
                // - Validar que no existan documentos o auditorias asociadas al
                //   standard en el ciclo
                if (await IsAnyItemInStandardAuditCycle(foundItem))
                    throw new BusinessException("There are items associated with this standard in the audit cycle");

                _repository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status == StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _repository.Update(foundItem);
            }

            try
            {
                await _repository.SaveChangesAsync(); // Async para esperar aquí a ver si sucede un error
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditCycleStandard.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync

        // PRIVATE METHODS

        private async Task<bool> IsAnyItemInStandardAuditCycle(AuditCycleStandard item)        {
            var auditCycleDocumentsRepository = new AuditCycleDocumentRepository();
            var auditsRepository = new AuditRepository();

            // - Validar que no existan documentos en AuditCycleDocuments
            if (await auditCycleDocumentsRepository
                .IsAnyStandardDocumentInAuditCycleAsync(item.StandardID.Value, item.AuditCycleID))
                return true;

            // - Validar que no existan auditorias en AuditStandards
            if (await auditsRepository
                .IsAnyStandardInAuditForAuditCycleAsync(item.StandardID.Value, item.AuditCycleID))
                return true;

            return false;
        } // IsAnyItemInStandardAuditCycle
    }
}