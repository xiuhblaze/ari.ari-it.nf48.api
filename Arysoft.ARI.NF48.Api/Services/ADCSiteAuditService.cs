using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class ADCSiteAuditService
    {
        public readonly ADCSiteAuditRepository _repository;

        // CONSTRUCTOR

        public ADCSiteAuditService()
        { 
            _repository = new ADCSiteAuditRepository();
        }

        // METHODS

        public PagedList<ADCSiteAudit> Gets(ADCSiteAuditQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.ADCSiteID.HasValue)
                items = items.Where(x => x.ADCSiteID == filters.ADCSiteID);

            if (filters.Status.HasValue && filters.Status != StatusType.Nothing)
            {
                items = items.Where(x => x.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != StatusType.Nothing)
                    : items.Where(e => e.Status != StatusType.Nothing
                        && e.Status != StatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            {
                case ADCSiteAuditOrderType.AuditStep:  
                    items = items.OrderBy(x => x.AuditStep);
                    break;
                case ADCSiteAuditOrderType.Value:
                    items = items.OrderBy(x => x.Value);
                    break;
                case ADCSiteAuditOrderType.Status:
                    items = items.OrderBy(x => x.Status);
                    break;
                case ADCSiteAuditOrderType.AuditStepDesc:
                    items = items.OrderByDescending(x => x.AuditStep);
                    break;
                case ADCSiteAuditOrderType.ValueDesc:
                    items = items.OrderByDescending(x => x.Value);
                    break;
                case ADCSiteAuditOrderType.StatusDesc:
                    items = items.OrderByDescending(x => x.Status);
                    break;
                default:
                    items = items.OrderBy(x => x.AuditStep);
                    break;
            }
            
            var pagedItems = PagedList<ADCSiteAudit>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<ADCSiteAudit> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync    

        public async Task<ADCSiteAudit> CreateAsync(ADCSiteAudit item)
        {
            // Validations

            if (item.ADCSiteID == Guid.Empty)
                throw new ArgumentException("The ADCSite is required.");

            // Set values

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
                throw new BusinessException($"ADCSiteAuditService.CreateAsync: {ex.Message}");
            }

            return item;
        } // CreateAsync

        public async Task<ADCSiteAudit> UpdateAsync(ADCSiteAudit item)
        { 
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to Update was not found");

            ValidateUpdateItem(item, foundItem);
            SetValuesUpdateItem(item, foundItem);

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();                
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCSiteAuditService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task<List<ADCSiteAudit>> UpdateListAsync(List<ADCSiteAudit> items)
        {
            if (!(items?.Any() ?? false)) // Valida si la lista es nula o vacía
                throw new BusinessException("The list of ADC Concept Values to Update is empty");

            var areUpdatedItems = false;
            var updatedItems = new List<ADCSiteAudit>();

            foreach (var item in items)
            {
                var foundItem = await _repository.GetAsync(item.ID)
                    ?? throw new BusinessException($"One of the records (ADC Concept Value) to Update was not found: {item.ID }");

                ValidateUpdateItem(item, foundItem);
                SetValuesUpdateItem(item, foundItem);
                _repository.Update(foundItem);
                areUpdatedItems = true;
                updatedItems.Add(foundItem);
            }

            if (areUpdatedItems)
            { 
                try
                {
                    await _repository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"ADCSiteAuditService.UpdateListAsync: {ex.Message}");
                }
            }

            return updatedItems;
        } // UpdateListAsync

        public async Task DeleteAsync(ADCSiteAudit item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to Delete was not found");

            // Validations
            // - aun no tengo validaciones

            // Excecute queries

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
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCSiteAuditService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync

        // PRIVATE

        private void ValidateUpdateItem(ADCSiteAudit item, ADCSiteAudit foundItem)
        {
            // Validations

            // - Validar que tenga el AuditStep
            if (item.AuditStep == null || item.AuditStep == AuditStepType.Nothing)
                throw new BusinessException("The Audit Step is required.");

            // - Validar que no esté para ese ADCSite en mismo AuditStep
            if (_repository.ExistsAuditStep(
                item.ADCSiteID, 
                item.AuditStep ?? AuditStepType.Nothing, 
                item.ID))
                throw new BusinessException("The Audit Step already exists for that ADCSite");

            // - De acuerdo al tipo de AuditCycle, ver si es valido el AuditStep

        } // validateUpdateItem 

        private void SetValuesUpdateItem(ADCSiteAudit item, ADCSiteAudit foundItem)
        {
            foundItem.Value = item.Value;
            foundItem.AuditStep = item.AuditStep;
            foundItem.Status = foundItem.Status == StatusType.Nothing && item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status != StatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

        } // SetValuesUpdateItem
    }
}