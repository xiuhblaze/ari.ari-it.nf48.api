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
    public class RoleService
    {
        private readonly RoleRepository _roleRepository;

        // CONSTRUCTOR

        public RoleService()
        {
            _roleRepository = new RoleRepository();
        }

        // METHODS

        public PagedList<Role> Gets(RoleQueryFilters filters)
        {
            var items = _roleRepository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => 
                    (e.Name != null && e.Name.ToLower().Contains(filters.Text))
                    || (e.Description != null && e.Description.ToLower().Contains(filters.Text))
                    || (e.UpdatedUser != null && e.UpdatedUser.ToLower().Contains(filters.Text))
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
                    : items.Where(e => e.Status != StatusType.Nothing
                        && e.Status != StatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            {
                case RoleOrderType.Name:
                    items = items.OrderBy(i => i.Name);
                    break;
                case RoleOrderType.Updated:
                    items = items.OrderBy(i => i.UpdatedUser); 
                    break;
                case RoleOrderType.NameDesc:
                    items = items.OrderByDescending(i => i.Name);
                    break;
                case RoleOrderType.UpdatedDesc:
                    items = items.OrderByDescending(i => i.UpdatedUser);
                    break;
                default:
                    items = items.OrderBy(i => i.Name);
                    break;
            }

            // Paging

            var pagedItems = PagedList<Role>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Role> GetAsync(Guid id)
        {
            return await _roleRepository.GetAsync(id);
        } // GetAsync

        public async Task<Role> AddAsync(Role item)
        {
            // Validations

            if (string.IsNullOrEmpty(item.UpdatedUser))            
                throw new BusinessException("User was not specified");
            
            // Assign values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;
            item.UpdatedUser = item.UpdatedUser;

            // Execute queries

            await _roleRepository.DeleteTmpByUserAsync(item.UpdatedUser);
            _roleRepository.Add(item);
            await _roleRepository.SaveChangesAsync();

            return item;
        } // AddAsync

        public async Task<Role> UpdateAsync(Role item)
        {
            var foundItem = await _roleRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - Otro rol con el mismo nombre

            // Assigning values

            foundItem.Name = item.Name;
            foundItem.Description = item.Description;
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
                _roleRepository.Update(foundItem);
                await _roleRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Role item)
        {
            var foundItem = await _roleRepository.GetAsync(item.ID)
                ?? throw new BusinessException("Item was not found");

            if (foundItem.Status == StatusType.Deleted)
            {
                _roleRepository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status == StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _roleRepository.Update(foundItem);
            }

            _roleRepository.SaveChanges();
        } // DeleteAsync
    }
}