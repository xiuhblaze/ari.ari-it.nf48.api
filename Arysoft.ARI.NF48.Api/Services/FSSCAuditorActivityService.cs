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
    public class FSSCAuditorActivityService
    {
        private readonly FSSCAuditorActivityRepository _repository;

        // CONSTRUCTOR

        public FSSCAuditorActivityService()
        { 
            _repository = new FSSCAuditorActivityRepository();
        } // FSSCAuditorActivityService

        // METHODS

        public PagedList<FSSCAuditorActivity> Gets(FSSCAuditorActivityQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(e => 
                    (e.Education != null && e.Education.ToLower().Contains(filters.Text))
                    || (e.LegalRequirements != null && e.LegalRequirements.ToLower().Contains(filters.Text))
                    || (e.SpecificTraining != null && e.SpecificTraining.ToLower().Contains(filters.Text))
                    || (e.Comments != null && e.Comments.ToLower().Contains(filters.Text))
                );
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
                case FSSCAuditorActivityOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case FSSCAuditorActivityOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated); 
                    break;
            }

            // Paging

            var pagedItems = PagedList<FSSCAuditorActivity>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<FSSCAuditorActivity> GetAsync(Guid id)
        { 
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<FSSCAuditorActivity> AddAsync(FSSCAuditorActivity item)
        {
            // Assinging values

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
                throw new BusinessException($"FSSCAuditorActivityService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<FSSCAuditorActivity> UpdateAsync(FSSCAuditorActivity item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - no validations yet

            // Assigning values

            if (item.Status == StatusType.Nothing) item.Status = StatusType.Active;

            foundItem.Education = item.Education;
            foundItem.LegalRequirements = item.LegalRequirements;
            foundItem.SpecificTraining = item.SpecificTraining;
            foundItem.Comments = item.Comments;
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
                throw new BusinessException($"FSSCAuditorActivityService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(FSSCAuditorActivity item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            // - no validations yet

            // Execute queries

            if (foundItem.Status == StatusType.Deleted)
            {
                // - validar que no tenga "job o audits experiences" asociadas
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
                throw new BusinessException($"FSSCAuditorActivityService.DeleteAsync: {ex.Message}");
            }
        }
    }
}