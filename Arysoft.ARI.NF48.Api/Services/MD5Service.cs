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
    public class MD5Service
    {
        private readonly MD5Repository _repository;

        // CONSTRUCTOR

        public MD5Service()
        {
            _repository = new MD5Repository();
        }

        // METHODS

        public PagedList<MD5> Gets(MD5QueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.NumEmployees != null && filters.NumEmployees > 0)
            {
                items = items.Where(e =>
                    e.StartValue <= filters.NumEmployees
                    && e.EndValue >= filters.NumEmployees);
            }

            if (filters.Days != null && filters.Days > 0)
            {
                items = items.Where(e => e.Days == filters.Days);
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
                case MD5OrderType.StartValue:
                    items = items.OrderBy(i => i.StartValue);
                    break;
                case MD5OrderType.Days:
                    items = items.OrderBy(i => i.Days);
                    break;
                case MD5OrderType.StartValueDesc:
                    items = items.OrderByDescending(i => i.StartValue);
                    break;
                case MD5OrderType.DaysDesc:
                    items = items.OrderByDescending(i => i.Days);
                    break;
                default:
                    items = items.OrderBy(i => i.StartValue);
                    break;
            }

            // Pagination

            var pagedItems = PagedList<MD5>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<MD5> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<MD5> CreateAsync(MD5 item)
        {
            // Validate

            await ValidateDataAsync(item);

            // Assign values

            item.ID = Guid.NewGuid();
            //item.Status = StatusType.Nothing;
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
                throw new BusinessException($"MD5Service.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<MD5> UpdateAsync(MD5 item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The MD5 item to update was not found");

            // Validate

            await ValidateDataAsync(item);

            // Assign values

            foundItem.StartValue = item.StartValue;
            foundItem.EndValue = item.EndValue;
            foundItem.Days = item.Days;
            foundItem.Status = foundItem.Status == StatusType.Nothing && item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status != StatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"MD5Service.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(MD5 item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The MD5 item to delete was not found");

            // Validate

            if (foundItem.Status == StatusType.Deleted)
            {
                // Validar cuando sea una eliminación física
                _repository.Delete(foundItem);
            }
            else
            {
                // Assign values
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
                throw new BusinessException($"MD5Service.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync

        private async Task ValidateDataAsync(MD5 item)
        {
            // TODO: - Validar que el usuario exista y esté activo

            if (item.StartValue.HasValue && item.StartValue <= 0)
                throw new BusinessException("The start value must be greater than zero");

            if (item.EndValue.HasValue && item.EndValue <= 0)
                throw new BusinessException("The end value must be greater than zero");

            if (item.Days.HasValue && item.Days <= 0)
                throw new BusinessException("The days must be greater than zero");

            if (item.StartValue.HasValue && item.EndValue.HasValue)
            {
                if (item.StartValue > item.EndValue)
                    throw new BusinessException("The start value must be less than or equal to the end value");

                // - Que el rango de empleados no se solape con otro rango existente
                if (await _repository.IsInRangeAsync(item.StartValue ?? 0, item.EndValue ?? 0))
                    throw new BusinessException("The range of employees already exists in the database");
            }
        }
    }
}