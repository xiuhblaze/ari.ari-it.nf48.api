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

            if (item.InitialStep == AuditStepType.Nothing)
                throw new BusinessException("An initial step is required");

            if (item.CycleType == AuditCycleType.Nothing)
                throw new BusinessException("A cycle type is required");

            // - Que no haya una asignación en este ciclo del mismo standard
            if (await _repository.IsStandardInCycleAsync(item.AuditCycleID, (Guid)item.StandardID, item.ID))
                throw new BusinessException("There is already a current standard in this cycle");

            // - Que no exista otro ciclo activo con este standard
            //TODO: Considerar si se debe validar solo si el ciclo está activo
            //TODO: REVISAR EL USO DE ESTA VALIDACION -xBlaze
            //if (await _repository.IsStandardInAnyOrganizationActiveCycleAsync(foundItem.AuditCycle.OrganizationID, (Guid)item.StandardID, item.ID))
            //    throw new BusinessException("There is already a current standard in another active cycle");

            // Assigning values

            if (item.Status == StatusType.Nothing) item.Status = StatusType.Active;

            foundItem.StandardID = item.StandardID;
            foundItem.InitialStep = item.InitialStep;
            foundItem.CycleType = item.CycleType;
            foundItem.Status = foundItem.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;
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
    }
}