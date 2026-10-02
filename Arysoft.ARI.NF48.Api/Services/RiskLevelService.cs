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
    public class RiskLevelService
    {
        private readonly BaseRepository<RiskLevel> _riskLevelRepository;

        // CONSTRUCTOR

        public RiskLevelService()
        {
            _riskLevelRepository = new BaseRepository<RiskLevel>();
        }

        // METHODS

        public PagedList<RiskLevel> Gets(RiskLevelQueryFilters filters)
        {
            var items = _riskLevelRepository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(x =>
                    x.BusinessSector.ToLower().Contains(filters.Text.ToLower())
                );
            }

            if (filters.StandardID != null)
            {
                items = items.Where(x => x.StandardID == filters.StandardID);
            }

            if (filters.Category != null && filters.Category != RiskLevelCategoryType.Nothing)
            {
                items = items.Where(x => x.Category == filters.Category);
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
                case RiskLevelOrderType.StandardName:
                    items = items.OrderBy(x => x.Standard.Name)
                        .ThenBy(x => x.Category)
                        .ThenBy(x => x.BusinessSector);
                    break;
                case RiskLevelOrderType.Category:
                    items = items.OrderBy(x => x.Category)
                        .ThenBy(x => x.BusinessSector);
                    break;
                case RiskLevelOrderType.BusinessSector:
                    items = items.OrderBy(x => x.BusinessSector);
                    break;
                case RiskLevelOrderType.StandardNameDesc:
                    items = items.OrderByDescending(x => x.Standard.Name)
                        .ThenByDescending(x => x.Category)
                        .ThenByDescending(x => x.BusinessSector);
                    break;
                case RiskLevelOrderType.CategoryDesc:
                    items = items.OrderByDescending(x => x.Category)
                        .ThenByDescending(x => x.BusinessSector);
                    break;
                case RiskLevelOrderType.BusinessSectorDesc:
                    items = items.OrderByDescending(x => x.BusinessSector);
                    break;
                default:
                    items = items.OrderBy(x => x.Standard.Name)
                        .ThenBy(x => x.Category)
                        .ThenBy(x => x.BusinessSector);
                    break;
            }

            // Paging

            var pagedItems = PagedList<RiskLevel>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<RiskLevel> GetAsync(Guid id)
        {
            var item = await _riskLevelRepository.GetAsync(id);
            return item;
        } // GetAsync

        public async Task<RiskLevel> CreateAsync(RiskLevel item)
        {
            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Executing queries

            try
            {
                await _riskLevelRepository.DeleteTmpByUserAsync(item.UpdatedUser);
                _riskLevelRepository.Add(item);
                await _riskLevelRepository.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new BusinessException($"RisklevelSerivice.AddAsync: {ex.Message}");
            }

            return item;
        } // CreateAsync

        public async Task<RiskLevel> UpdateAsync(RiskLevel item)
        { 
            // Validations

            var foundItem = await _riskLevelRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (foundItem.Status == StatusType.Nothing) 
            {
                if (!item.StandardID.HasValue) throw new BusinessException("The standard is required");

                // - Validar que el ID sea de un standard válido (aunque no esté activo)
                var standard = await new StandardRepository().GetAsync(item.StandardID.Value)
                    ?? throw new BusinessException("The selected standard does not exist");
                if (standard.Status == StatusType.Deleted) throw new BusinessException("The selected standard is not active");
            }

            if (string.IsNullOrEmpty(item.BusinessSector)) 
                throw new BusinessException("The business sector is required");

            // Assignin values

            if (foundItem.Status == StatusType.Nothing)
            { 
                foundItem.StandardID = item.StandardID;
            }

            foundItem.Category = item.Category;
            foundItem.BusinessSector = item.BusinessSector;
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
                _riskLevelRepository.Update(foundItem);
                await _riskLevelRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"RisklevelSerivice.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(RiskLevel item)
        {
            var foundItem = await _riskLevelRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            if (foundItem.Status == StatusType.Deleted)
            {
                _riskLevelRepository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status == StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _riskLevelRepository.Update(foundItem);
            }

            try
            {
                _riskLevelRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"RisklevelSerivice.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}
