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
    public class NSSCActivityService
    {
        private readonly NSSCActivityRepository _repository;

        // CONSTRUCTOR

        public NSSCActivityService()
        { 
            _repository = new NSSCActivityRepository();
        } // NSSCActivityService

        // METHODS

        public PagedList<NSSCActivity> Gets(NSSCActivityQueryFilters filters) 
        {
            var items = _repository.Gets();

            // Filters

            if (filters.NSSCSubCategoryID != null)
            {
                items = items.Where(e => e.NSSCSubCategoryID == filters.NSSCSubCategoryID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            { 
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => 
                    (e.Name != null && e.Name.ToLower().Contains(filters.Text))
                    && (e.Description != null && e.Description.ToLower().Contains(filters.Text))
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
                case NSSCActivityOrderType.SubCategory:
                    items = items.OrderBy(e => e.NSSCSubCategory.Name)
                        .ThenBy(e => e.Name);
                    break;
                case NSSCActivityOrderType.Name:
                    items = items.OrderBy(e => e.Name);
                    break;
                case NSSCActivityOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case NSSCActivityOrderType.SubCategoryDesc:
                    items = items.OrderByDescending(e => e.NSSCSubCategory.Name)
                        .ThenByDescending(e => e.Name);
                    break;
                case NSSCActivityOrderType.NameDesc:
                    items = items.OrderByDescending(e => e.Name);
                    break;
                case NSSCActivityOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
            }

            // Paging

            var pagedItems = PagedList<NSSCActivity>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<NSSCActivity> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<NSSCActivity> AddAsync(NSSCActivity item)
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
                throw new BusinessException($"NSSCActivityService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<NSSCActivity> UpdateAsync(NSSCActivity item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            item.Status = item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;

            // - Que no exista ese nombre en la sub categoria asociada

            // Assigning values

            foundItem.Name = item.Name;
            foundItem.Description = item.Description;
            foundItem.Status = foundItem.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;
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
                throw new BusinessException($"NSSCActivityService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(NSSCActivity item)
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
                throw new BusinessException($"NSSCActivityService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}