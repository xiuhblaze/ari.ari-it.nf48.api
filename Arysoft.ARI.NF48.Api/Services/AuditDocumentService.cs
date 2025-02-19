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
    public class AuditDocumentService
    {
        public readonly AuditDocumentRepository _repository;

        // CONSTRUCTOR

        public AuditDocumentService()
        {
            _repository = new AuditDocumentRepository();
        }

        // METHODS

        public PagedList<AuditDocument> Gets(AuditDocumentQueryFilters filters)
        { 
            var items = _repository.Gets();

            // Filters

            if (filters.AuditID != null && filters.AuditID != Guid.Empty)
            {
                items = items.Where(e => e.AuditID == filters.AuditID);
            }

            if (filters.StandardID != null && filters.StandardID != Guid.Empty)
            {
                items = items.Where(e => e.StandardID == filters.StandardID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items
                    .Where(e =>
                        (e.Comments != null && e.Comments.ToLower().Contains(filters.Text))
                        || (e.OtherDescription != null && e.OtherDescription.ToLower().Contains(filters.Text))
                    );
            }

            if (filters.DocumentType != null && filters.DocumentType != AuditDocumentType.Nothing)
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
                case AuditDocumentOrderType.DocumentType:
                    items = items.OrderBy(e => e.DocumentType);
                    break;
                case AuditDocumentOrderType.Standard:
                    items = items.OrderBy(e => e.Standard.Name);
                    break;
                case AuditDocumentOrderType.DocumentTypeDesc:
                    items = items.OrderByDescending(e => e.DocumentType);
                    break;
                case AuditDocumentOrderType.StandardDesc:
                    items = items.OrderByDescending(e => e.Standard.Name);
                    break;
                default:
                    items = items.OrderBy(e => e.DocumentType);
                    break;
            }

            // Paging

            var pagedItems = PagedList<AuditDocument>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<AuditDocument> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // Get

        public async Task<AuditDocument> AddAsync(AuditDocument item)
        {
            // Validations

            if (item.AuditID == Guid.Empty)
                throw new BusinessException("Must first assign an audit");

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
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditDocumentService.AddAsync: {ex.Message}");
            }

            return item;
        } // Create

        public async Task<AuditDocument> UpdateAsync(AuditDocument item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - Si es la primera vez, debe de traer el StandardID
            if (foundItem.Status == StatusType.Nothing)
            { 
                if (item.StandardID == null || item.StandardID == Guid.Empty)
                    throw new BusinessException("Must first assign a standard");

                // - El Standard debe de coincidir con el alguno de la auditoria

                foundItem.StandardID = item.StandardID;
            }

            if (item.DocumentType == AuditDocumentType.Other)
            {
                if (string.IsNullOrEmpty(item.OtherDescription))
                    throw new BusinessException("Must provide a description for the document type 'Other'");
            }

            // - Que respete el orden de los documentos, no permitir agregar uno que
            //   provoque el brinco de un paso, salvo si es auditoria especial

            // Assigning values

            if (item.Status == StatusType.Nothing)
                item.Status = StatusType.Active;

            foundItem.Filename = item.Filename;
            foundItem.Comments = item.Comments;
            foundItem.DocumentType = item.DocumentType;
            foundItem.OtherDescription = item.OtherDescription;
            foundItem.IsWitnessIncluded = item.IsWitnessIncluded;
            foundItem.Status = foundItem.Status == StatusType.Nothing
                ? StatusType.Active
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
                throw new BusinessException($"AuditDocumentService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(AuditDocument item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

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
                throw new BusinessException($"AuditDocumentService.DeleteAsync: {ex.Message}");
            }
        }
    }
}