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
    public class NSSCSubCategoryService
    {
        private readonly NSSCSubCategoryRepository _repository;

        // CONSTRUCTOR

        public NSSCSubCategoryService()
        {
            _repository = new NSSCSubCategoryRepository();
        } // NSSCSubCategoryService

        // METHODS

        public PagedList<NSSCSubCategory> Gets(NSSCSubCategoryQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.NSSCCategoryID != null) 
            {
                items = items.Where(e => e.NSSCCategoryID == filters.NSSCCategoryID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e =>
                    (e.Name != null && e.Name.ToLower().Contains(filters.Text))
                    || (e.Description != null && e.Description.ToLower().Contains(filters.Text))
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
                case NSSCSubCategoryOrderType.Category:
                    items = items.OrderBy(e => e.NSSCCategory.Name)
                        .ThenBy(e => e.Name);
                    break;
                case NSSCSubCategoryOrderType.Name:
                    items = items.OrderBy(e => e.Name);
                    break;
                case NSSCSubCategoryOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case NSSCSubCategoryOrderType.CategoryDesc:
                    items = items.OrderByDescending(e => e.NSSCCategory.Name)
                        .ThenByDescending(e => e.NSSCCategory.Name);
                    break;
                case NSSCSubCategoryOrderType.NameDesc:
                    items = items.OrderByDescending(e => e.Name);
                    break;
                case NSSCSubCategoryOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.NSSCCategory.Name)
                        .ThenBy(e => e.Name);
                    break;
            } // order

            // Paging

            var pagedItems = PagedList<NSSCSubCategory>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<NSSCSubCategory> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<NSSCSubCategory> AddAsync(NSSCSubCategory item)
        {
            // Validations

            if (string.IsNullOrEmpty(item.UpdatedUser))
                throw new BusinessException("Must specify a username");

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
                throw new BusinessException($"NSSCSubCategoryService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<NSSCSubCategory> UpdateAsync(NSSCSubCategory item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            item.Status = item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;

            // - Que no exista ese nombre en la categoria asociada

            // Assigning values

            foundItem.Name = item.Name;
            foundItem.Description = item.Description;
            foundItem.Status = foundItem.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;
            foundItem.Created = DateTime.UtcNow;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Excecute queries

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"NSSCSubCategoryService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(NSSCSubCategory item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            // - no validations yet


            // Execute queries

            if (foundItem.Status == StatusType.Deleted)
            {
                // - validar que no tenga activities asociadas
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
                throw new BusinessException($"NSSCSubCategoryService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}