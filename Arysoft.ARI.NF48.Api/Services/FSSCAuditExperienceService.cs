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
    public class FSSCAuditExperienceService
    {
        private readonly BaseRepository<FSSCAuditExperience> _repository;

        // CONSTRUCTOR

        public FSSCAuditExperienceService()
        {
            _repository = new BaseRepository<FSSCAuditExperience>();
        } // FSSCAuditExperienceService

        // METHODS

        public PagedList<FSSCAuditExperience> Gets(FSSCAuditExperienceQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            { 
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(e =>
                    e.Description != null && e.Description.ToLower().Contains(filters.Text)
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
                case FSSCAuditExperienceOrderType.Description:
                    items = items.OrderBy(e => e.Description);
                    break;
                case FSSCAuditExperienceOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case FSSCAuditExperienceOrderType.DescriptionDesc:
                    items = items.OrderByDescending(e => e.Description);
                    break;
                case FSSCAuditExperienceOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
            }

            // Paging

            var pagedItems = PagedList<FSSCAuditExperience>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<FSSCAuditExperience> GetAsync(Guid id)
        { 
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<FSSCAuditExperience> AddAsync(FSSCAuditExperience item)
        {
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
                throw new BusinessException($"FSSCAuditExperienceService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<FSSCAuditExperience> UpdateAsync(FSSCAuditExperience item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - no validations yet

            // Assigning values

            if (item.Status == StatusType.Nothing) item.Status = StatusType.Active;

            foundItem.FSSCJobExperienceID = item.FSSCJobExperienceID;
            foundItem.Description = item.Description;
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
                throw new BusinessException($"FSSCAuditExperienceService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(FSSCAuditExperience item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            // - no validations yet

            // Execute queries

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
                throw new BusinessException($"FSSCAuditExperienceService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}