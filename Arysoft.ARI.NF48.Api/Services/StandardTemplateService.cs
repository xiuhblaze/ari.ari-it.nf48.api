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
    public class StandardTemplateService
    {
        private readonly StandardTemplateRepository _standardTemplateRepository;

        // CONSTRUCTOR

        public StandardTemplateService()
        {
            _standardTemplateRepository = new StandardTemplateRepository();
        }

        // METHODS

        public PagedList<StandardTemplate> Gets(StandardTemplateQueryFilters filters)
        {
            var items = _standardTemplateRepository.Gets();

            // Filters

            if (filters.StandardID != null && filters.StandardID != Guid.Empty)
            {
                items = items.Where(st => st.StandardID == filters.StandardID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower();
                items = items.Where(st =>
                    (st.Description != null && st.Description.ToLower().Contains(filters.Text))                    
                    || (st.Version != null && st.Version.ToLower().Contains(filters.Text))
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
                case StandardTemplateOrderType.Description:
                    items = items.OrderBy(st => st.Description);
                    break;
                case StandardTemplateOrderType.Status:
                    items = items.OrderBy(st => st.Status);
                    break;
                case StandardTemplateOrderType.Update:
                    items = items.OrderBy(st => st.Updated);
                    break;
                case StandardTemplateOrderType.DescriptionDesc:
                    items = items.OrderByDescending(st => st.Description);
                    break;
                case StandardTemplateOrderType.StatusDesc:
                    items = items.OrderByDescending(st => st.Status);
                    break;
                case StandardTemplateOrderType.UpdateDesc:
                    items = items.OrderByDescending(st => st.Updated);
                    break;
            }

            // Paging

            var pagedItems = PagedList<StandardTemplate>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<StandardTemplate> GetAsync(Guid id)
        {
            return await _standardTemplateRepository.GetAsync(id);
        } // GetAsync

        public async Task<StandardTemplate> CreateAsync(StandardTemplate item)
        {
            // - Validar que sea un standard valido
            if (!await _standardTemplateRepository.IsValidStandard(item.StandardID))
                throw new BusinessException("The standard is not valid or does not exist.");
            
            // Assigning values

            item.ID = Guid.NewGuid();
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;
            item.Status = StatusType.Nothing;

            // Excecute queries

            try 
            { 
                await _standardTemplateRepository.DeleteTmpByUserAsync(item.UpdatedUser);
                _standardTemplateRepository.Add(item);
                await _standardTemplateRepository.SaveChangesAsync();
            }
            catch (Exception ex) 
            { 
                throw new BusinessException($"StandardTemplateService.CreateAsync: {ex.Message}");
            }

            return item;
        } // CreateAsync

        public async Task<StandardTemplate> UpdateAsync(StandardTemplate item)
        {
            var foundItem = await _standardTemplateRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record does not exist.");

            // Validations goes here

            // Assigning values

            foundItem.Description = item.Description;
            foundItem.Version = item.Version;
            foundItem.Filename = item.Filename;
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
                await _standardTemplateRepository.DeleteTmpByUserAsync(item.UpdatedUser);
                _standardTemplateRepository.Update(foundItem);
                await _standardTemplateRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"StandardTemplateService.UpdateAsync: {ex.Message}");
            }
            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(StandardTemplate item)
        {
            var foundItem = await _standardTemplateRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record does not exist.");

            if (foundItem.Status == StatusType.Deleted)
            {
                _standardTemplateRepository.Delete(foundItem);
            }
            else
            { 
                foundItem.Status = foundItem.Status == StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _standardTemplateRepository.Update(foundItem);
            }

            try
            {   
                await _standardTemplateRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"StandardTemplateService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}