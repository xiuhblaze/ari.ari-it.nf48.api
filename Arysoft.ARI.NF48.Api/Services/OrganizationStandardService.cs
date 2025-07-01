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
    public class OrganizationStandardService
    {
        private readonly OrganizationStandardRepository _repository;

        // CONSTRUCTOR 

        public OrganizationStandardService()
        {
            _repository = new OrganizationStandardRepository();
        }

        // METHODS

        public PagedList<OrganizationStandard> Gets(OrganizationStandardQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty)
            {
                items = items.Where(e => e.OrganizationID == filters.OrganizationID);
            }

            if (filters.StandardID != null && filters.StandardID != Guid.Empty)
            {
                items = items.Where(e => e.StandardID == filters.StandardID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(e =>
                    e.ExtraInfo != null && e.ExtraInfo.ToLower().Contains(filters.Text)
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
                case OrganizationStandardOrderType.Organization:
                    items = items.OrderBy(e => e.Organization.Name);
                    break;
                case OrganizationStandardOrderType.Standard:
                    items = items.OrderBy(e => e.Standard.Name);
                    break;
                case OrganizationStandardOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case OrganizationStandardOrderType.OrganizationDesc:
                    items = items.OrderByDescending(e => e.Organization.Name);
                    break;
                case OrganizationStandardOrderType.StandardDesc:
                    items = items.OrderByDescending(e => e.Standard.Name);
                    break;
                case OrganizationStandardOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.Standard.Name);
                    break;
            }

            var pagedItems = PagedList<OrganizationStandard>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<OrganizationStandard> GetAsync(Guid id)
        { 
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<OrganizationStandard> AddAsync(OrganizationStandard item)
        {
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
                throw new BusinessException($"OrganizationStandardService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<OrganizationStandard> UpdateAsync(OrganizationStandard item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            if (foundItem.Status == StatusType.Nothing) // Si es nuevo
            {
                if (item.StandardID == null || item.StandardID == Guid.Empty)
                    throw new BusinessException("The Standard ID must not be empty");

                if (await _repository.ExistStandardAsync(foundItem.OrganizationID, item.StandardID ?? Guid.Empty, foundItem.ID))
                    throw new BusinessException("The Standard already exists for the Organization.");

                //TODO: - Ver que el estandard sea valido y activo

                foundItem.StandardID = item.StandardID;
            }

            // Assigning values

            foundItem.ExtraInfo = item.ExtraInfo;
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
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"OrganizationStandardService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(OrganizationStandard item)
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
                throw new BusinessException($"OrganizationStandardService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}