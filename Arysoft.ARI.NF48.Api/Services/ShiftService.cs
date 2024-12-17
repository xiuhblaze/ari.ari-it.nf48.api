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
    public class ShiftService
    {
        private readonly ShiftRepository _shiftRepository;

        // CONSTRUCTOR

        public ShiftService()
        {
            _shiftRepository = new ShiftRepository();
        }

        // METHODS

        public PagedList<Shift> Gets(ShiftQueryFilters filters)
        {
            var items = _shiftRepository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => 
                    e.ActivitiesDescription != null && e.ActivitiesDescription.ToLower().Contains(filters.Text)
                );
            }

            if (filters.SiteID != null && filters.SiteID != Guid.Empty)
            {
                items = items.Where(e => e.SiteID == filters.SiteID);
            }

            if (filters.Type != null && filters.Type != ShiftType.Nothing)
            {
                items = items.Where(e => e.Type == filters.Type);
            }

            // HACK: En modo de prueba, no estoy seguro de vayan afuncionar correctamente
            if (filters.ShiftStart != null)
            {
                items = items.Where(e => e.ShiftStart <= filters.ShiftStart && e.ShiftEnd >= filters.ShiftStart);
            }

            if (filters.ShiftEnd != null)
            {
                items = items.Where(e => e.ShiftStart <= filters.ShiftEnd && e.ShiftEnd >= filters.ShiftEnd);
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
                case ShiftOrderType.Type:
                    items = items.OrderBy(e => e.Type);
                    break;
                case ShiftOrderType.NoEmployees:
                    items = items.OrderBy(e => e.NoEmployees);
                    break;
                case ShiftOrderType.Status:
                    items = items.OrderBy(e => e.Status);
                    break;
                case ShiftOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case ShiftOrderType.TypeDesc:
                    items = items.OrderByDescending(e => e.Type);
                    break;
                case ShiftOrderType.NoEmployeesDesc:
                    items = items.OrderByDescending(e => e.NoEmployees);
                    break;
                case ShiftOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status);
                    break;
                case ShiftOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
            }

            // Paging

            var pagedItems = PagedList<Shift>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Shift> GetAsync(Guid id)
        {
            return await _shiftRepository.GetAsync(id);
        } // GetAsync

        public async Task<Shift> AddAsync(Shift item)
        {
            // Validations

            if (string.IsNullOrEmpty(item.UpdatedUser)) // Debe de validar si existe el usuario
                throw new BusinessException("User was not specified");

            if (item.SiteID == null || item.SiteID == Guid.Empty)
                throw new BusinessException("Must first assing Site");

            // HACK: Tal vez validar que el Site esté en estatus activo

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;
            item.UpdatedUser = item.UpdatedUser;

            // Execute queries

            await _shiftRepository.DeleteTmpByUser(item.UpdatedUser);
            _shiftRepository.Add(item);
            await _shiftRepository.SaveChangesAsync();

            return item;
        } // AddAsync

        public async Task<Shift> UpdateAsync(Shift item)
        {
            // Validations

            var foundItem = await _shiftRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (item.Status == StatusType.Nothing) item.Status = StatusType.Active;

            // Assigning values

            foundItem.Type = item.Type;
            foundItem.NoEmployees = item.NoEmployees;
            foundItem.ShiftStart = item.ShiftStart;
            foundItem.ShiftEnd = item.ShiftEnd;
            foundItem.ActivitiesDescription = item.ActivitiesDescription;
            foundItem.Status = foundItem.Status == StatusType.Nothing 
                ? StatusType.Active 
                : item.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Execute queries

            try
            { 
                _shiftRepository.Update(foundItem);            
                await _shiftRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Shift item)
        {
            var foundItem = await _shiftRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            if (foundItem.Status == StatusType.Deleted)
            {
                _shiftRepository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status == StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _shiftRepository.Update(foundItem);
            }

            _shiftRepository.SaveChanges();
        } // DeleteAsync
    }
}