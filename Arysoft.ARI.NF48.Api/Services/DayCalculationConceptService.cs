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
    public class DayCalculationConceptService
    {
        private readonly DayCalculationConceptRepository _repository;

        // CONSTRUCTOR

        public DayCalculationConceptService()
        { 
            _repository = new DayCalculationConceptRepository();
        }

        // METHODS

        public PagedList<DayCalculationConcept> Gets(DayCalculationConceptQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.StandardID.HasValue)
            {
                items = items.Where(e => e.StandardID == filters.StandardID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => 
                    (e.Description != null && e.Description.ToLower().Contains(filters.Text))
                    // || (e.Description != null && e.Description.ToLower().Contains(filters.Text))
                );
            }

            if (filters.Unit.HasValue && filters.Unit != DayCalculationConceptUnitType.Nothing)
            {
                items = items.Where(e => e.Unit == filters.Unit);
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

            // Order by

            switch (filters.Order)
            {
                case DayCalculationConceptOrderType.Description:
                    items = items.OrderBy(e => e.Description);
                    break;
                case DayCalculationConceptOrderType.Unit:
                    items = items.OrderBy(e => e.Unit); 
                    break;
                case DayCalculationConceptOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case DayCalculationConceptOrderType.DescriptionDesc:
                    items = items.OrderByDescending(e => e.Description);
                    break;
                case DayCalculationConceptOrderType.UnitDesc:
                    items = items.OrderByDescending(e => e.Unit);
                    break;
                case DayCalculationConceptOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
            }

            // Paging

            var pagedItems = PagedList<DayCalculationConcept>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<DayCalculationConcept> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<DayCalculationConcept> AddAsync(DayCalculationConcept item)
        {
            item.ID = Guid.NewGuid();
            item.Unit = DayCalculationConceptUnitType.Nothing;
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
            _repository.Add(item);
            _repository.SaveChanges();

            return item;
        } // AddAsync

        public async Task<DayCalculationConcept> UpdateAsync(DayCalculationConcept item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - Que no sea el mismo Standard y la misma descripcion

            // Assigning values

            foundItem.StandardID = item.StandardID;
            foundItem.Description = item.Description;
            foundItem.Increase = item.Increase;
            foundItem.Decrease = item.Decrease;
            foundItem.Unit = item.Unit;
            foundItem.Status = foundItem.Status == StatusType.Nothing
                ? StatusType.Active
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
                throw new BusinessException(ex.Message);
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(DayCalculationConcept item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - Que no esté aplicado en algún Application Form

            if (foundItem.Status == StatusType.Deleted)
            {
                // Aplicar validaciones mas estrictas
                _repository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status < StatusType.Inactive
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _repository.Update(foundItem);
            }

            _repository.SaveChanges();
        } // DeleteAsync
    }
}