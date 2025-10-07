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
    public class StandardService
    {
        private readonly StandardRepository _standardRepository;

        // CONSTRUCTOR

        public StandardService()
        {
            _standardRepository = new StandardRepository();
        }

        // METHODS

        public PagedList<Standard> Gets(StandardQueryFilters filters)
        {
            var items = _standardRepository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(s =>
                    (s.Name != null && s.Name.ToLower().Contains(filters.Text))
                    || (s.Description != null && s.Description.ToLower().Contains(filters.Text))
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
                case StandardsOrderType.Name:
                    items = items.OrderBy(s => s.Name);
                    break;
                case StandardsOrderType.Status:
                    items = items.OrderBy(s => s.Status);
                    break;
                case StandardsOrderType.Update:
                    items = items.OrderBy(s => s.Updated);
                    break;
                case StandardsOrderType.NameDesc:
                    items = items.OrderByDescending(s => s.Name);
                    break;
                case StandardsOrderType.StatusDesc:
                    items = items.OrderByDescending(s => s.Status);
                    break;
                case StandardsOrderType.UpdateDesc:
                    items = items.OrderByDescending(s => s.Updated);
                    break;
                default:
                    items = items.OrderBy(s => s.Name);
                    break;
            }

            // Paging

            var pagedItems = PagedList<Standard>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Standard> GetAsync(Guid id)
        { 
            return await _standardRepository.GetAsync(id);
        } // GetAsync

        public async Task<Standard> AddAsync(Standard item)
        {
            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Excecute queries

            try 
            { 
                await _standardRepository.DeleteTmpByUserAsync(item.UpdatedUser);
                _standardRepository.Add(item);
                await _standardRepository.SaveChangesAsync();
            } 
            catch (Exception ex) 
            { 
                throw new BusinessException($"StandardService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<Standard> UpdateAsync(Standard item)
        {
            // Validations

            var foundItem = await _standardRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (await _standardRepository.ExistNameAsync(item.Name, item.ID))
                throw new BusinessException("The standard name already exist");

            if (item.StandardBase == null || item.StandardBase == StandardBaseType.Nothing)
                throw new BusinessException("The standard base is required");

            // Assigning values

            foundItem.Name = item.Name;
            foundItem.Description = item.Description;
            foundItem.MaxReductionDays = item.MaxReductionDays;
            foundItem.SalesMaxReductionDays = item.SalesMaxReductionDays;
            foundItem.StandardBase = item.StandardBase;
            foundItem.Status = foundItem.Status == StatusType.Nothing && item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status != StatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Excecute queries
            try
            {
                _standardRepository.Update(foundItem);
                await _standardRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Standard item)
        {
            var foundItem = await _standardRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            if (foundItem.Status == StatusType.Deleted)
            {
                // Validations
                if (await _standardRepository.IsAnyAssociationAsync(foundItem.ID))
                    throw new BusinessException("The standard has associations with other records and cannot be deleted.");

                _standardRepository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status == StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _standardRepository.Update(foundItem);
            }

            try
            { 
                _standardRepository.SaveChanges();
            }
            catch (Exception ex) 
            { 
                throw new BusinessException($"StandardService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}