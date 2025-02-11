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
using System.Web;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class AuditorStandardService
    {
        private readonly AuditorStandardRepository _repository;

        // CONSTRUCTOR

        public AuditorStandardService()
        {
            _repository = new AuditorStandardRepository();
        }

        // METHODS

        public PagedList<AuditorStandard> Gets(AuditorStandardQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.AuditorID != null && filters.AuditorID != Guid.Empty)
            {
                items = items.Where(e => e.AuditorID == filters.AuditorID);
            }

            if (filters.StandardID != null && filters.StandardID != Guid.Empty)
            {
                items = items.Where(e => e.StandardID == filters.StandardID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(e =>
                    e.Comments != null && e.Comments.ToLower().Contains(filters.Text)
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
                case AuditorStandardOrderType.Auditor:
                    items = items.OrderBy(e => e.Auditor.FirstName)
                        .ThenBy(e => e.Auditor.MiddleName); 
                    break;
                case AuditorStandardOrderType.Standard:
                    items = items.OrderBy(e => e.Standard.Name);
                    break;
                case AuditorStandardOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case AuditorStandardOrderType.AuditorDesc:
                    items = items.OrderByDescending(e => e.Auditor.FirstName)
                        .ThenByDescending(e => e.Auditor.MiddleName);
                    break;
                case AuditorStandardOrderType.StandardDesc:
                    items = items.OrderByDescending(e => e.Standard.Name);
                    break;
                case AuditorStandardOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.Standard.Name);
                    break;
            }

            var pagedItems = PagedList<AuditorStandard>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<AuditorStandard> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<AuditorStandard> AddAsync(AuditorStandard item)
        {
            // Asigning values

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
                throw new BusinessException($"AuditorStandardService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<AuditorStandard> UpdateAsync(AuditorStandard item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            if (item.Status == StatusType.Nothing) item.Status = StatusType.Active;


            if (foundItem.Status == StatusType.Nothing)
            {
                if (item.StandardID == null || item.StandardID == Guid.Empty)
                    throw new BusinessException("The Standard ID must not be empty");

                if (await _repository.ExistStandardAsync(foundItem.AuditorID, item.StandardID ?? Guid.Empty, item.ID))
                    throw new BusinessException("The Standard already exists for the Auditor.");

                foundItem.StandardID = item.StandardID; // Solo cuando es nuevo, se puede asignar este valor
            }

            // Assigning values

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
                throw new BusinessException($"AuditorStandardService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(AuditorStandard item)
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
                throw new BusinessException($"AuditorStandardService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}