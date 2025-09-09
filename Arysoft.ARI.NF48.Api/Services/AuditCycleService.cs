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
    public class AuditCycleService
    {
        private readonly AuditCycleRepository _repository;

        // CONSTRUCTOR

        public AuditCycleService()
        {
            _repository = new AuditCycleRepository();
        }

        // METHODS

        public PagedList<AuditCycle> Gets(AuditCycleQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty)
            {
                items = items.Where(e => e.OrganizationID == filters.OrganizationID);
            }   

            if (filters.StartDate != null)
            {
                items = items.Where(e => e.StartDate >= filters.StartDate);
            }

            if (filters.EndDate != null)
            {
                items = items.Where(e => e.EndDate <= filters.EndDate);
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
                case AuditCycleOrderType.Date:
                    items = items.OrderBy(e => e.StartDate);
                    break;
                case AuditCycleOrderType.DateDesc:
                    items = items.OrderByDescending(e => e.StartDate);
                    break;
                case AuditCycleOrderType.Status:
                    items = items.OrderBy(e => e.Status);
                    break;
                case AuditCycleOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status);
                    break;
                default:
                    items = items.OrderByDescending(e => e.StartDate);
                    break;
            }

            // Paging

            var pagedItems = PagedList<AuditCycle>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<AuditCycle> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<AuditCycle> AddAsync(AuditCycle item)
        { 
            // Validations

            if (item.OrganizationID == Guid.Empty)
                throw new BusinessException("Organization is required");

            // - Validar que la organizacion exista
            // - Validar que la organización tenga un status de activo
            // - Validar que el standard exista
            // - Validar que el standard pertenezca a la organizacion
            // - Validar que el standard tenga un status de activo
            // - Validar que el usuario exista (no hay que confiarse)
            // - TODO: Consultar que otro requisito necesita la organización para poder crear un ciclo

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
                throw new BusinessException($"AuditCycle.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<AuditCycle> UpdateAsync(AuditCycle item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // TODO: Esto se debe de validar al integrar un standard, puede haber
            //     más de un ciclo activo por organización pero con diferente
            //     standard
            // ACTUALIZACION (xb-20250212): Al parecer solo puede haber un ciclo activo por organización, un standard nuevo se agregaria al ciclo
            // - Validar que las fechas de inicio y termino no se superpongan a otro
            // ACTUALIZACION (xb-20250225): Nop puede haber más de un ciclo activo por organización, pero no con el mismo standard
            //if (await _repository.IsAnyCycleBetweenDatesByOrganizationAsync(item.OrganizationID, (DateTime)item.StartDate, (DateTime)item.EndDate))
            //{
            //    throw new BusinessException("There is already an audit cycle between the dates provided");
            //}

            // - StartDate not must be greater than EndDate
            if (item.StartDate > item.EndDate)
                throw new BusinessException("The start date must be less than the end date");

            // Assigning values

            // if (item.Status == StatusType.Nothing) item.Status = StatusType.Active;

            // Si cambia a ciclo activo, verificar que no exista otro ciclo activo
            // con el mismo standard
            if (item.Status == StatusType.Active && foundItem.Status != StatusType.Active)
            {
                if (_repository.IsAnyCycleActiveByOrganizationAndStandard(foundItem.OrganizationID, foundItem.AuditCycleStandards, foundItem.ID))
                    throw new BusinessException("There is a other active cycle with the same standard");
            }

            foundItem.Name = item.Name;
            foundItem.StartDate = item.StartDate;
            foundItem.EndDate = item.EndDate;
            foundItem.Periodicity = item.Periodicity;
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
                await _repository.SaveChangesAsync(); // Async para esperar aquí a ver si sucede un error

            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditCycle.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(AuditCycle item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations


            // Excecute queries

            if (foundItem.Status == StatusType.Deleted)
            {
                // - Validar que no existan auditorías asociadas al ciclo
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
                throw new BusinessException($"AuditCycle.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync 
    }
}