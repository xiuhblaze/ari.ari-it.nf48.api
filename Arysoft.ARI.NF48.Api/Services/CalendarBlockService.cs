using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class CalendarBlockService
    {
        private readonly CalendarBlockRepository _repository;

        // CONSTRUCTOR

        public CalendarBlockService()
        { 
            _repository = new CalendarBlockRepository();
        } // CalendarBlockService

        // FUNCTIONS

        public PagedList<CalendarBlock> Gets(CalendarBlockQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.OwnerID.HasValue && filters.OwnerID.Value != Guid.Empty) 
            { 
                items = items.Where(i => i.OwnerID == filters.OwnerID.Value);
            }

            if (filters.BlockType.HasValue && filters.BlockType.Value != CalendarBlockType.Nothing)
            {
                items = items.Where(i => i.BlockType == filters.BlockType.Value);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(i =>
                    (i.Description != null && i.Description.ToLower().Contains(filters.Text))
                    || (i.ExtraInfo != null && i.ExtraInfo.ToLower().Contains(filters.Text)) 
                );
            }

            if (filters.StartDate.HasValue)
            {
                items = items.Where(i => i.EndDate >=  filters.StartDate.Value);
            }

            if (filters.EndDate.HasValue) 
            { 
                items = items.Where(i => i.StartDate <= filters.EndDate.Value);
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
                case CalendarBlockOrderType.Description:
                    items = items.OrderBy(i => i.Description);
                    break;
                case CalendarBlockOrderType.StartDate:
                    items = items.OrderBy(i => i.StartDate);
                    break;
                case CalendarBlockOrderType.BlockType:
                    items = items.OrderBy(i => i.BlockType); 
                    break;
                case CalendarBlockOrderType.DescriptionDesc:
                    items = items.OrderByDescending(i => i.Description);
                    break;
                case CalendarBlockOrderType.StartDateDesc:
                    items = items.OrderByDescending(i => i.StartDate);
                    break;
                case CalendarBlockOrderType.BlockTypeDesc:
                    items = items.OrderByDescending(i => i.BlockType);
                    break;
            }

            // Pagination

            var pagedItems = PagedList<CalendarBlock>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<CalendarBlock> GetAsync(Guid id, bool asNoTracking = false)
        {
            var item = await _repository.GetAsync(id, asNoTracking)
                ?? throw new BusinessException("CalendarBlock not found");

            return item;
        } // GetAsync

        public async Task<CalendarBlock> CreateAsync(CalendarBlock item)
        {
            // Validations

            if (string.IsNullOrEmpty(item.UpdatedUser))
                throw new BusinessException("The user who created the item was not found.");

            // Assigning values
            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Executing queries
            try
            {
                await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
                _repository.Add(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"CalendarBlockService.CreateAsync: {ex.Message}");
            }

            return item;
        } // CreateAsync

        public async Task<CalendarBlock> UpdateAsync(CalendarBlock item)
        {
            // Validations

            // 1. Que exista el elemento en la BDD            
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("CalendarBlock to update not found", HttpStatusCode.NotFound);

            await ValidateUpdatedItemAsync(item, foundItem);

            // Assingning values

            if (foundItem.Status == StatusType.Nothing)
            {
                foundItem.OwnerID = item.OwnerID;
                foundItem.OwnerType = item.OwnerType;
            }

            foundItem.BlockType = item.BlockType;
            foundItem.Description = item.Description;
            foundItem.StartDate = item.StartDate;
            foundItem.EndDate = item.EndDate;
            foundItem.AllowOverride = item.AllowOverride;
            foundItem.ExtraInfo = item.ExtraInfo;
            foundItem.Status = foundItem.Status == StatusType.Nothing && item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status != StatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Executing queries

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"CalendarBlockService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(CalendarBlock item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("CalendarBlock to delete not found");

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
                _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"CalendarBlockService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync

        #region PRIVATE

        private async Task ValidateUpdatedItemAsync(CalendarBlock item, CalendarBlock foundItem)
        {
            // 2. Validar los datos minimos (BlockType, Description, StartDate, EndDate)
            // 3. Validar que no haya duplicado del mismo propietario, tipo y rango de tiempo, incluye los de OwnerID = null
            // 4. Validar de que Status a que Status puede cambiar

            if (!item.BlockType.HasValue)
                throw new BusinessException("Must specify a type of block");

            if (string.IsNullOrEmpty(item.Description))
                throw new BusinessException("Must specify a description");

            if (!item.StartDate.HasValue)
                throw new BusinessException("Must specify the start date");

            if (!item.EndDate.HasValue)
                throw new BusinessException("Must specify the end date");

            // Evaluar si es necesario pues podria haber dos bloqueos con diferente descripción
            if (await _repository.ExistCalendarBlockAsync(
                item.OwnerID,
                item.StartDate ?? new DateTime(),
                item.EndDate ?? new DateTime(),
                item.BlockType ?? CalendarBlockType.Nothing,
                item.ID)) throw new BusinessException("The CalendarBlock already exists");

        } // ValidateUpdatedItem

        #endregion
    }
}