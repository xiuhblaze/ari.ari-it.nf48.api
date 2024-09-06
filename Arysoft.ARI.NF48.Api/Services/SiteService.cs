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
    public class SiteService
    {
        private readonly SiteRepository _siteRepository;

        // CONSTRUCTOR

        public SiteService()
        {
            _siteRepository = new SiteRepository();
        }

        // METHODS

        public PagedList<Site> Gets(SiteQueryFilters filters)
        {
            var items = _siteRepository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e =>
                    e.Description.ToLower().Contains(filters.Text)
                    || e.LocationDescription.ToLower().Contains(filters.Text)
                );
            }

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty)
            {
                items = items.Where(e => e.OrganizationID == filters.OrganizationID);
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
                case SiteOrderType.Description:
                    items = items.OrderBy(e => e.Description);
                    break;
                case SiteOrderType.Order:
                    items = items.OrderBy(e => e.Order);
                    break;
                case SiteOrderType.Status:
                    items = items.OrderBy(e => e.Status);
                    break;
                case SiteOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case SiteOrderType.DescriptionDesc:
                    items = items.OrderByDescending(e => e.Description);
                    break;
                case SiteOrderType.OrderDesc:
                    items = items.OrderByDescending(e => e.Order);
                    break;
                case SiteOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status);
                    break;
                case SiteOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.Order);
                    break;
            }

            // Paging

            var pagedItems = PagedList<Site>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Site> GetAsync(Guid id)
        { 
            return await _siteRepository.GetAsync(id);
        } // GetAsync

        public async Task<Site> AddAsync(Site item)
        {
            // Validations

            //if (string.IsNullOrEmpty(item.UpdatedUser)) // Debe de validar si existe el usuario
            //    throw new BusinessException("Updated user was not specified");

            if (item.OrganizationID == null || item.OrganizationID == Guid.Empty)
                throw new BusinessException("Must first assign Organization");

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;
            
            // Execute queries

            await _siteRepository.DeleteTmpByUser(item.UpdatedUser);
            _siteRepository.Add(item);
            await _siteRepository.SaveChangesAsync();

            return item;
        } // AddAsync

        public async Task<Site> UpdateAsync(Site item)
        { 
            // Validations

            var foundItem = await _siteRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (item.Order != foundItem.Order)  // Si el orden cambió, reajustar la de todos los demas Site
            {
                UpdateOrder(item);
            }

            // Assigning values

            foundItem.Description = item.Description;
            foundItem.LocationDescription = item.LocationDescription;
            foundItem.Order = item.Order;
            foundItem.Status = foundItem.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Execute queries

            try
            { 
                _siteRepository.Update(foundItem);
                await _siteRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Site item)
        {
            var foundItem = await _siteRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");


            if (foundItem.Status == StatusType.Deleted)
            {
                if (foundItem.Shifts.Any())
                    throw new BusinessException("The record want to delete, still has Shifts");

                _siteRepository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status == StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _siteRepository.Update(foundItem);
            }

            _siteRepository.SaveChanges();
        } // DeleteAsync

        // PRIVATE

        private void UpdateOrder(Site site)
        {
            var items = _siteRepository.Gets()
                .Where(m => 
                    m.OrganizationID == site.OrganizationID 
                    && m.ID != site.ID
                    && m.Status == StatusType.Active)
                .OrderBy(m => m.Order)
                .ToList();

            int i = 1;
            foreach (var item in items)
            {
                if (i != site.Order)
                { 
                    item.Order = i;
                    _siteRepository.Update(item);
                }
                i++;
            }
        } // UpdateOrder
    }
}