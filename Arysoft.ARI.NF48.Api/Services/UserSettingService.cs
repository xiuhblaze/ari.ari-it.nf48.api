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
    public class UserSettingService
    {
        private readonly UserSettingRepository _repository;

        // CONSTRUCTOR

        public UserSettingService()
        {
            _repository = new UserSettingRepository();
        }

        // METHODS

        public PagedList<UserSetting> Gets(UserSettingQueryFilters filters)
        {
            var items = _repository.Gets();

            if (filters.UserID != null && filters.UserID != Guid.Empty)
                items = items.Where(m => m.UserID == filters.UserID);

            if (!string.IsNullOrEmpty(filters.Text))
            { 
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(m => 
                    (m.Settings != null && m.Settings.ToLower().Contains(filters.Text))
                    || (m.UpdatedUser != null && m.UpdatedUser.ToLower().Contains(filters.Text))
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
                case UserSettingOrderType.UserName:
                    items = items.OrderBy(m => m.User != null ? m.User.Username : string.Empty);
                    break;
                case UserSettingOrderType.Updated:
                    items = items.OrderBy(m => m.Updated);
                    break;
                case UserSettingOrderType.UserNameDesc:
                    items = items.OrderByDescending(m => m != null ? m.User.Username : string.Empty);
                    break;
                case UserSettingOrderType.UpdatedDesc:
                    items = items.OrderByDescending(m => m.Updated);
                    break;
                default:
                    items = items.OrderBy(m => m.User?.Username ?? string.Empty);
                    break;
            }

            // Pagination

            var pagedItems = PagedList<UserSetting>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<UserSetting> GetAsync(Guid id, bool asNoTracking = false)
        {
            return await _repository.GetAsync(id, asNoTracking);
        } // GetAsync

        public async Task<UserSetting> CreateAsync(UserSetting item)
        {
            // Validations
            if (item.UserID == Guid.Empty)
                throw new ArgumentException("The User is required.");

            // - Que no haya otro registro para el usuario
            if (await _repository.IsUserSettingExistsAsync(item.UserID, item.ID))
                throw new BusinessException("A record for this user already exists.");

            // Set values

            item.ID = Guid.NewGuid();
            item.Status = string.IsNullOrEmpty(item.Settings)
                ? StatusType.Nothing
                : StatusType.Active;
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
                throw new BusinessException($"UserSettingService.CreateAsync: {ex.Message}");
            }

            return item;
        } // CreateAsync

        public async Task<UserSetting> UpdateAsync(UserSetting item)
        { 
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to Update was not found");

            // Validations

            // - No se me ocurre ninguna

            // Set values

            foundItem.Settings = item.Settings;
            foundItem.Status = item.Status;
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
                throw new BusinessException($"UserSettingService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(UserSetting item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to Delete was not found");

            // Set values

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

            // Execute queries
            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"UserSettingService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}