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
    public class AuditStandardService
    {
        public readonly AuditStandardRepository _repository;

        // CONSTRUCTOR

        public AuditStandardService()
        {
            _repository = new AuditStandardRepository();
        }

        // METHODS

        public PagedList<AuditStandard> Gets(AuditStandardQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.AuditID != null && filters.AuditID != Guid.Empty)
            {
                items = items.Where(e => e.AuditID == filters.AuditID);
            }

            if (filters.StandardID != null && filters.StandardID != Guid.Empty)
            {
                items = items.Where(e => e.StandardID == filters.StandardID);
            }

            if (filters.Step != null && filters.Step != Enumerations.AuditStepType.Nothing)
            {
                items = items.Where(e => e.Step == filters.Step);
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
                case AuditStandardOrderType.StandardName:
                    items = items.OrderBy(e => e.Standard.Name);
                    break;
                case AuditStandardOrderType.Status:
                    items = items.OrderBy(e => e.Status);
                    break;
                case AuditStandardOrderType.StandardNameDesc:
                    items = items.OrderByDescending(e => e.Standard.Name);
                    break;
                case AuditStandardOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status);
                    break;
                default:
                    items = items.OrderBy(e => e.Standard.Name);
                    break;
            }

            // Paging

            var pagedItems = PagedList<AuditStandard>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<AuditStandard> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<AuditStandard> AddAsync(AuditStandard item)
        {
            // Validations

            if (item.AuditID == null || item.AuditID == Guid.Empty)
                throw new BusinessException("Must first assign an audit");

            // - Solo en los status de la auditoria de: [Nothing, Sheduled,
            //   Confirmed] se pueden agregar standards
            
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
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditStandardService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<AuditStandard> UpdateAsync(AuditStandard item)
        {
            var _auditCycleRepository = new AuditCycleRepository();
            var _standardRepository = new StandardRepository();
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            var foundStandard = await _standardRepository.GetAsync(item.StandardID.Value)
                ?? throw new BusinessException("The standard was not found");

            var foundAuditCycle = await _auditCycleRepository.GetAsync(item.AuditCycleID.Value)
                ?? throw new BusinessException("The audit cycle was not found");

            // - La primera vez debe de traer el StandardID
            if (foundItem.Status == StatusType.Nothing)
            {
                //if (item.StandardID == null || item.StandardID == Guid.Empty)
                //    throw new BusinessException("The standard is missing");
                if (item.AuditCycleID == null || item.AuditCycleID == Guid.Empty)
                    throw new BusinessException("The standard is missing");

                // - Validar que el standard esté activo
                if (foundStandard.Status != StatusType.Active)
                    throw new BusinessException("The standard is not active");

                // - Validar que el audit cycle esté activo o inactivo
                if (foundAuditCycle.Status != StatusType.Active 
                    && foundAuditCycle.Status != StatusType.Inactive)
                    throw new BusinessException("The audit cycle is not in a valid status");
            }

            // - Que no esté duplicado el standard en el mismo audit
            var query = _repository.Gets()
                .Where(x => x.AuditID == foundItem.AuditID
                    && x.StandardID == foundItem.StandardID
                    && x.ID != item.ID);
            if (query.Any())
                throw new BusinessException("The standard is already assigned to this audit");

            if (item.Status != foundItem.Status) // Cambio de estatus
            {
                switch (item.Status) // Si el nuevo status es...
                {
                    case StatusType.Active:
                        
                        // // #CHANGE_CYCLES: Evaluar esta validación una vez se implementen los cambios -xBlaze 20251205
                        // - Que el standard no este asignado con otra auditoria del mismo audit
                        //   cycle y el mismo step
                        //if (foundItem.Audit != null 
                        //    && foundItem.Audit.Status != AuditStatusType.Nothing
                        //    && !(foundItem.Audit.IsMultisite.HasValue && foundItem.Audit.IsMultisite.Value))
                        //{ 
                        //    var auditsRepository = new AuditRepository();
                        //    var hasStandard = await auditsRepository.IsAnyStandardStepAuditInAuditCycle(
                        //        foundItem.Audit.AuditCycleID,
                        //        foundItem.StandardID.Value,
                        //        item.Step.Value,
                        //        foundItem.Audit.ID
                        //    );
                        //    if (hasStandard)
                        //        throw new BusinessException("The standard with this step is already assigned to another audit");
                        //}
                        break;
                }
            }

            //   ciclo y mismo step
            // - Si al menos un standard en su Step es de tipo special, todos deben de ser igual
            //   DUDA: Cuando es una auditoria especial, se seleccionan Standares a revisar?
            //         R: Si se seleccionan los standares y se marcan como special


            // - Que el standard no este asignado con otra auditoria del mismo - se movió al switch de más arriba
            //if (item.Status == StatusType.Active 
            //    && foundItem.Audit != null 
            //    && foundItem.Audit.Status != AuditStatusType.Nothing
            //    && !(foundItem.Audit.IsMultisite.HasValue && foundItem.Audit.IsMultisite.Value))
            //{ 
            //    var auditsRepository = new AuditRepository();
            //    var hasStandard = await auditsRepository.IsAnyStandardStepAuditInAuditCycle(
            //        foundItem.Audit.AuditCycleID,
            //        foundItem.StandardID.Value,
            //        item.Step.Value,
            //        foundItem.Audit.ID
            //    );
            //    if (hasStandard)
            //        throw new BusinessException("The standard with this step is already assigned to another audit");

            //}

            // Assigning values

            if (foundItem.Status == StatusType.Nothing) // Primera vez que se asigna
            {
                // Asignando el audit cycle y el standard
                foundItem.AuditCycleID = item.AuditCycleID.Value;
                foundItem.StandardID = foundAuditCycle.StandardID; //TODO: Probar esto!!!
            }

            foundItem.Step = item.Step;
            foundItem.ExtraInfo = item.ExtraInfo;
            foundItem.Status = foundItem.Status == StatusType.Nothing && item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status != StatusType.Nothing
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
                throw new BusinessException($"AuditStandardService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(AuditStandard item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            if (foundItem.Status == StatusType.Deleted)
            {
                // - Validar si hay documentos asociados con la auditoria y el standard (AuditDocuments)

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
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditStandardService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}