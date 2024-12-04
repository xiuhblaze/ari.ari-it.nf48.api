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
    public class CatAuditorDocumentService
    {
        private readonly CatAuditorDocumentRepository _repository;

        // CONSTRUCTOR

        public CatAuditorDocumentService()
        {
            _repository = new CatAuditorDocumentRepository();
        }

        // METHODS

        public PagedList<CatAuditorDocument> Gets(CatAuditorDocumentQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => 
                    e.Name.ToLower().Contains(filters.Text)
                    || e.Description.ToLower().Contains(filters.Text)
                );
            }

            if (filters.DocumentType != null && filters.DocumentType != CatAuditorDocumentType.Nothing)
            {
                items = items.Where(e => e.DocumentType == filters.DocumentType);
            }

            if (filters.SubCategory != null && filters.SubCategory != CatAuditorDocumentSubCategoryType.Nothing)
            {
                items = items.Where(e => e.SubCategory == filters.SubCategory);
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
                case CatAuditorDocumentOrderType.DocumentType:
                    items = items.OrderBy(e => e.DocumentType)
                        .ThenBy(e => e.Order);
                    break;
                case CatAuditorDocumentOrderType.Order:
                    items = items.OrderBy(e => e.Order);
                    break;
                case CatAuditorDocumentOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case CatAuditorDocumentOrderType.DocumentTypeDesc:
                    items = items.OrderByDescending(e => e.DocumentType)
                        .ThenByDescending(e => e.Order);
                    break;
                case CatAuditorDocumentOrderType.OrderDesc:
                    items = items.OrderByDescending(e => e.Order);
                    break;
                case CatAuditorDocumentOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
            }

            // Paging

            var pagedItems = PagedList<CatAuditorDocument>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<CatAuditorDocument> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<CatAuditorDocument> AddAsync(CatAuditorDocument item)
        {
            // Assigning values
            
            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries

            await _repository.DeleteTmpByUser(item.UpdatedUser);
            _repository.Add(item);
            _repository.SaveChanges();

            return item;
        } // AddAsync

        public async Task<CatAuditorDocument> UpdateAsync(CatAuditorDocument item)
        {
            // Validations

            if (item.Status == StatusType.Nothing) item.Status = StatusType.Active;

            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (item.DocumentType == CatAuditorDocumentType.Nothing)
                throw new BusinessException("Must specify the document type");

            if (item.UpdateEvery < 0)
                throw new BusinessException("Update every cannot be negative");

            if (item.UpdatePeriodicity == CatAuditorDocumentPeriodicityType.Nothing)
                throw new BusinessException("Must select the update periodicity");

            if (item.WarningEvery < 0)
                throw new BusinessException("Warning every cannot be negative");

            if (item.WarningPeriodicity == CatAuditorDocumentPeriodicityType.Nothing)
                throw new BusinessException("Must select the warning periodicity");

            // Assigning values

            foundItem.Name = item.Name;
            foundItem.Description = item.Description;
            foundItem.DocumentType = item.DocumentType;
            foundItem.SubCategory = item.SubCategory;
            foundItem.UpdateEvery = item.UpdateEvery;
            foundItem.UpdatePeriodicity = item.UpdatePeriodicity;
            foundItem.WarningEvery = item.WarningEvery;
            foundItem.WarningPeriodicity = item.WarningPeriodicity;
            foundItem.IsRequired = item.IsRequired;
            foundItem.Order = item.Order;
            foundItem.Status = foundItem.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Execute queries

            try
            {
                _repository.Update(foundItem);
                _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"CatAuditorDocumentService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(CatAuditorDocument item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - no validations yet

            // Execute queries

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

            _repository.SaveChanges();
        } // DeleteAsync
    }
}