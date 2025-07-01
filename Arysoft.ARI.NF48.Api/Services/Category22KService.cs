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
    public class Category22KService
    {
        private readonly Category22KRepository _category22KRepository;

        // CONSTRUCTOR

        public Category22KService()
        {
            _category22KRepository = new Category22KRepository();
        }

        // METHODS

        public PagedList<Category22K> Gets(Category22KQueryFilters filters)
        { 
            var items = _category22KRepository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            { 
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => 
                    (e.Cluster != null && e.Cluster.ToLower().Contains(filters.Text))
                    || (e.Category != null && e.Category.ToLower().Contains(filters.Text))
                    || (e.CategoryDescription != null && e.CategoryDescription.ToLower().Contains(filters.Text))
                    || (e.SubCategory != null && e.SubCategory.ToLower().Contains(filters.Text))
                    || (e.SubCategoryDescription != null && e.SubCategoryDescription.ToLower().Contains(filters.Text))
                    || (e.Examples != null && e.Examples.ToLower().Contains(filters.Text))
                );
            }

            if (filters.Accredited != null && filters.Accredited != Category22KAccreditedType.Nothing)
            {
                items = items.Where(e =>
                    (filters.Accredited == Category22KAccreditedType.Accredited && e.IsAccredited)
                    || (filters.Accredited == Category22KAccreditedType.NotAccredited && !e.IsAccredited)
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

            // Order by

            switch (filters.Order)
            {
                case Category22KOrderType.Cluster:
                    items = items.OrderBy(e => e.Cluster)
                        .ThenBy(e => e.Category);
                    break;
                case Category22KOrderType.Category:
                    items = items.OrderBy(e => e.Category)
                        .ThenBy(e => e.SubCategory);
                    break;
                case Category22KOrderType.ClusterDesc:
                    items = items.OrderByDescending(e => e.Cluster)
                        .ThenByDescending(e => e.Category);
                    break;
                case Category22KOrderType.CategoryDesc:
                    items = items.OrderByDescending(e => e.Category)
                        .ThenByDescending(e => e.SubCategory);
                    break;
                default:
                    items = items.OrderBy(e => e.Category)
                        .ThenBy(e => e.SubCategory); 
                    break;
            }

            // Paging

            var pagedItems = PagedList<Category22K>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Category22K> GetAsync(Guid id)
        { 
            return await _category22KRepository.GetAsync(id);
        } // GetAsync

        public async Task<Category22K> AddAsync(Category22K item)
        { 
            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            await _category22KRepository.DeleteTmpByUserAsync(item.UpdatedUser);
            _category22KRepository.Add(item);
            await _category22KRepository.SaveChangesAsync();

            return item;
        } // AddAsync

        public async Task<Category22K> UpdateAsync(Category22K item)
        {
            var foundItem = await _category22KRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - Que no haya duplicados
            if (await _category22KRepository.ExistByCategorySubCategoryAsync(item.Category, item.SubCategory))
                throw new BusinessException("The Category and sub category already exist");

            // Assigning values

            foundItem.Cluster = item.Cluster;
            foundItem.Category = item.Category;
            foundItem.CategoryDescription = item.CategoryDescription;
            foundItem.SubCategory = item.SubCategory;
            foundItem.SubCategoryDescription = item.SubCategoryDescription;
            foundItem.Examples = item.Examples;
            foundItem.IsAccredited = item.IsAccredited;
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
                _category22KRepository.Update(foundItem);
                await _category22KRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Category22K item)
        {
            var foundItem = await _category22KRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            // - Que no tenga certificados activos, who knows

            if (foundItem.Status == StatusType.Deleted)
            {
                //! Considerar eliminar todas las asociaciones al registro antes de su eliminación tales como
                //  applications, ...
                _category22KRepository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status < StatusType.Inactive
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _category22KRepository.Update(foundItem);
            }

            _category22KRepository.SaveChanges();
        } // DeleteAsync
    }
}