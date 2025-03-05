using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.IO;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class AuditService
    {
        public readonly AuditRepository _repository;

        // CONSTRUCTOR

        public AuditService()
        {
            _repository = new AuditRepository();
        }

        // METHODS

        public PagedList<Audit> Gets(AuditQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty)
            {
                items = items
                    .Where(e => 
                        e.AuditCycle != null 
                        && e.AuditCycle.OrganizationID == filters.OrganizationID
                    );
            }

            if (filters.AuditCycleID != null && filters.AuditCycleID != Guid.Empty)
            {
                items = items
                    .Where(e => e.AuditCycleID == filters.AuditCycleID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items
                    .Where(e =>
                        (e.Description != null && e.Description.ToLower().Contains(filters.Text))
                    );
            }

            if (filters.StartDate != null)
            {
                items = items.Where(e => e.EndDate >= filters.StartDate);
            }

            if (filters.EndDate != null)
            {
                items = items
                    .Where(e => e.StartDate <= filters.EndDate);
            }

            if (filters.Status != null && filters.Status != AuditStatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != AuditStatusType.Nothing)
                    : items.Where(e => e.Status != AuditStatusType.Nothing && e.Status != AuditStatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            {
                case AuditOrderType.Date:
                    items = items.OrderBy(e => e.StartDate);
                    break;
                case AuditOrderType.Status:
                    items = items.OrderBy(e => e.Status);
                    break;
                case AuditOrderType.DateDesc:
                    items = items.OrderByDescending(e => e.StartDate);
                    break;
                case AuditOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status);
                    break;
                default:
                    items = items.OrderBy(e => e.StartDate);
                    break;
            }

            // Paging

            var pagedItems = PagedList<Audit>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Audit> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<Audit> AddAsync(Audit item)
        {
            // Validations
            if (item.AuditCycleID == null || item.AuditCycleID == Guid.Empty)
                throw new BusinessException("Must first assign an audit cycle");

            // - Validar que el ciclo sea el activo o sea en el futuro

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = AuditStatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries

            try {

                // Borrando carpetas de registros temporales
                var items = _repository.Gets()
                    .Where(e => e.UpdatedUser.ToUpper() == item.UpdatedUser.ToUpper().Trim()
                        && e.Status == AuditStatusType.Nothing);

                foreach (var i in items)
                { 
                    FileRepository.DeleteDirectory($"~/files/organizations/{i.AuditCycle.OrganizationID}/Cycles/{i.AuditCycle.ID}/{i.ID}");
                }

                await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
                _repository.Add(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<Audit> UpdateAsync(Audit item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - Que las fechas de auditoria no se translapen con otra auditoria del mismo ciclo

            // Assigning values

            //if (item.Status == AuditStatusType.Nothing)
            //    item.Status = AuditStatusType.Scheduled;

            foundItem.Description = item.Description;
            foundItem.StartDate = item.StartDate;
            foundItem.EndDate = item.EndDate;
            foundItem.HasWitness = item.HasWitness;
            foundItem.Status = item.Status == AuditStatusType.Nothing
                ? AuditStatusType.Scheduled
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
                throw new BusinessException($"AuditService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Audit item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            if (foundItem.Status == AuditStatusType.Deleted)
            {
                _repository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status < AuditStatusType.Canceled
                    ? AuditStatusType.Canceled
                    : AuditStatusType.Deleted;
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
                throw new BusinessException($"AuditService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}