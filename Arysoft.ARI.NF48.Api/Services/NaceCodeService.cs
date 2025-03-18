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
    public class NaceCodeService
    {
        private readonly BaseRepository<NaceCode> _naceCodeRepository;

        // CONSTRUCTOR

        public NaceCodeService()
        {
            _naceCodeRepository = new BaseRepository<NaceCode>();
        }

        // METHODS

        public PagedList<NaceCode> Gets(NaceCodeQueryFilters filters)
        {
            var items = _naceCodeRepository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => 
                    e.Description != null && e.Description.ToLower().Contains(filters.Text)
                );
            }

            if (filters.Sector != null)
            {
                items = items.Where(e => e.Sector == filters.Sector);
            }

            if (filters.Division != null)
            {
                items = items.Where(e => e.Division == filters.Division);
            }

            if (filters.Group != null)
            {
                items = items.Where(e => e.Group == filters.Group);
            }

            if (filters.Class != null)
            {
                items = items.Where(e => e.Class == filters.Class);
            }

            if (filters.OnlyOption != null)
            {
                switch (filters.OnlyOption)
                {
                    case NaceCodeOnlyOptionType.Sectors:
                        items = items.Where(e => e.Sector != null && e.Division == null && e.Group == null && e.Class == null);
                        break;
                    case NaceCodeOnlyOptionType.Divisions:
                        items = items.Where(e => e.Sector != null && e.Division != null && e.Group == null && e.Class == null);
                        break;
                    case NaceCodeOnlyOptionType.Groups:
                        items = items.Where(e => e.Sector != null && e.Division != null && e.Group != null && e.Class == null);
                        break;
                    case NaceCodeOnlyOptionType.Classes:
                        items = items.Where(e => e.Sector != null && e.Division != null && e.Group != null && e.Class != null);
                        break;
                }
            }

            //if (filters.OnlySectors != null && (bool)filters.OnlySectors)
            //{
            //    items = items.Where(e => e.Sector != null && e.Division == null && e.Group == null && e.Class == null);
            //}

            //if (filters.OnlyDivisions != null && (bool)filters.OnlyDivisions)
            //{
            //    items = items.Where(e => e.Sector != null && e.Division != null && e.Group == null && e.Class == null);
            //}

            //if (filters.OnlyGroups != null && (bool)filters.OnlyGroups)
            //{
            //    items = items.Where(e => e.Sector != null && e.Division != null && e.Group != null && e.Class == null);
            //}

            //if (filters.OnlyClasses != null && (bool)filters.OnlyClasses)
            //{
            //    items = items.Where(e => e.Sector != null && e.Division != null && e.Group != null && e.Class != null);
            //}

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
                case NaceCodeOrderType.Description:
                    items = items.OrderBy(e => e.Description);
                    break;
                case NaceCodeOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case NaceCodeOrderType.SectorDesc:
                    items = items.OrderByDescending(e => e.Sector)
                        .ThenByDescending(e => e.Division)
                        .ThenByDescending(e => e.Group)
                        .ThenByDescending(e => e.Class);
                    break;
                case NaceCodeOrderType.DescriptionDesc:
                    items = items.OrderByDescending(e => e.Description);
                    break;
                case NaceCodeOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default: // NaceCodeOrderType.Sector
                    items = items.OrderBy(e => e.Sector)
                        .ThenBy(e => e.Division)
                        .ThenBy(e => e.Group)
                        .ThenBy(e => e.Class);
                    break;
            }

            // Paging

            var pagedItems = PagedList<NaceCode>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<NaceCode> GetAsync(Guid id)
        { 
            return await _naceCodeRepository.GetAsync(id);
        } // GetAsync

        public async Task<NaceCode> AddAsync(NaceCode item)
        {
            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries

            await _naceCodeRepository.DeleteTmpByUserAsync(item.UpdatedUser);
            _naceCodeRepository.Add(item);
            await _naceCodeRepository.SaveChangesAsync();

            return item;
        } // AddAsync

        public async Task<NaceCode> UpdateAsync(NaceCode item)
        {
            // Validations

            // -- validate if not exist a duplicate item with Sector, Division, Group and Class

            var foundItem = await _naceCodeRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Assigning values

            foundItem.Sector = item.Sector;
            foundItem.Division = item.Division;
            foundItem.Group = item.Group;
            foundItem.Class = item.Class;
            foundItem.Description = item.Description;
            foundItem.Status = item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Execute queries

            try
            { 
                _naceCodeRepository.Update(foundItem);
                await _naceCodeRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(NaceCode item)
        {
            var foundItem = await _naceCodeRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            if (foundItem.Status == StatusType.Deleted)
            {
                // Hacer validaciones previas
                _naceCodeRepository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status == StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _naceCodeRepository.Update(foundItem);
            }

            _naceCodeRepository.SaveChanges();
        } // DeleteAsync
    }
}