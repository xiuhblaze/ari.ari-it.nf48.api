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
    public class AuditCycleDocumentService
    {
        private readonly AuditCycleDocumentRepository _repository;

        // CONSTRUCTOR

        public AuditCycleDocumentService()
        {
            _repository = new AuditCycleDocumentRepository();
        } // AuditCycleDocumentService

        // METHODS

        public PagedList<AuditCycleDocument> Gets(AuditCycleDocumentQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower();
                items = items.Where(e => 
                    (e.Version != null && e.Version.ToLower().Contains(filters.Text))
                    || (e.Comments != null && e.Comments.ToLower().Contains(filters.Text))
                    || (e.OtherDescription != null && e.OtherDescription.ToLower().Contains(filters.Text))
                );
            }

            if (filters.AuditCycleID != null)
            {
                items = items.Where(e => e.AuditCycleID == filters.AuditCycleID);
            }

            if (filters.StandardID != null)
            {
                items = items.Where(e => e.StandardID != null && e.StandardID == filters.StandardID);
            }

            if (filters.DocumentType != null && filters.DocumentType != Enumerations.AuditCycleDocumentType.Nothing)
            {
                items = items.Where(e => e.DocumentType == filters.DocumentType);
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
                case AuditCycleDocumentOrderType.Version:
                    items = items.OrderBy(e => e.Version);
                    break;
                case AuditCycleDocumentOrderType.DocumentType:
                    items = items.OrderBy(e => e.DocumentType); 
                    break;
                case AuditCycleDocumentOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case AuditCycleDocumentOrderType.VersionDesc:
                    items = items.OrderByDescending(e => e.Version);
                    break;
                case AuditCycleDocumentOrderType.DocumentTypeDesc:
                    items = items.OrderByDescending(e => e.DocumentType);
                    break;
                case AuditCycleDocumentOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
            }

            var pagedItems = PagedList<AuditCycleDocument>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<AuditCycleDocument> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<AuditCycleDocument> AddAsync(AuditCycleDocument item)
        {
            // Validations

            if (item.AuditCycleID == Guid.Empty)
                throw new BusinessException("Audit cycle is required");

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries

            try
            {
                await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
                _repository.Add(item);
                await _repository.SaveChangesAsync(); // Async para esperar aquí a ver si sucede un error
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditCycleDocument.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<AuditCycleDocument> UpdateAsync(AuditCycleDocument item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            if (item.DocumentType == AuditCycleDocumentType.Nothing)
                throw new BusinessException("Document type is required");

            if (item.DocumentType == AuditCycleDocumentType.Other && string.IsNullOrEmpty(item.OtherDescription))
                throw new BusinessException("Other description is required");

            // Assigning values

            foundItem.Version = item.Version;
            foundItem.DocumentType = item.DocumentType;
            foundItem.Comments = item.Comments;
            foundItem.OtherDescription = item.OtherDescription;
            foundItem.Updated = DateTime.UtcNow;

            // Execute queries

            try
            {
                await _repository.DeleteTmpByUserAsync(foundItem.UpdatedUser);
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync(); // Async para esperar aquí a ver si sucede un error
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditCycleDocument.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task<bool> DeleteAsync(AuditCycleDocument item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

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
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditCycleDocument.DeleteAsync: {ex.Message}");
            }

            return true;
        } // DeleteAsync
    }
}