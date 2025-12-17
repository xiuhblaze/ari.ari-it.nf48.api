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
                items = items.Where(e => e.AuditCycles != null 
                    && e.AuditCycles
                        .Where(ac => ac.ID == filters.AuditCycleID)
                        .Any()
                );
            }

            if (filters.StandardID != null)
            {
                items = items.Where(e => e.AuditCycles != null 
                    && e.AuditCycles
                        .Where(ac => ac.StandardID == filters.StandardID)
                        .Any()
                );
            }

            if (filters.DocumentType != null && filters.DocumentType != AuditCycleDocumentType.Nothing)
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

            //if (item.AuditCycleID == Guid.Empty)
            //    throw new BusinessException("Audit cycle is required");
            if (item.OrganizationID == Guid.Empty)
                throw new BusinessException("Organization is required");

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

            // - validar que el documento sea de alguno de los standares activos en el ciclo

            // Assigning values

            // foundItem.StandardID = item.StandardID;
            foundItem.Filename = item.Filename;
            foundItem.Version = item.Version;
            foundItem.DocumentType = item.DocumentType;
            foundItem.Comments = item.Comments;
            foundItem.OtherDescription = item.OtherDescription;
            foundItem.UploadedBy = item.UploadedBy;
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

        // AUDIT CYCLES

        public async Task AddAuditCycleAsync(Guid id, Guid auditCycleID)
        {
            var _auditCycleRepository = new AuditCycleRepository();

            // - Validar que el ciclo de auditoría exista
            // - Validar que todos los AuditCycles sean de la misma organización
            // - Validar que no se agregue otro ciclo con el mismo standard

            var auditCycle = await _auditCycleRepository.GetAsync(auditCycleID)
                ?? throw new BusinessException("The audit cycle to assign was not found");

            if (await _repository.IsAnyAuditCycleStandardAsync(id, auditCycle.StandardID ?? Guid.Empty))            
                throw new BusinessException("The document already has assigned a cycle with the same standard");

            try
            {
                await _repository.AddAuditCycleAsync(id, auditCycleID);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditCycleDocumentService.AddAuditCycleAsync: {ex.Message}");
            }
        } // AddAuditCycleAsync

        public async Task DelAuditCycleAsync(Guid id, Guid auditCycleID)
        {
            // - Validar que el documento se quede con al menos un ciclo de auditoría

            try
            {
                await _repository.DelAuditCycleAsync(id, auditCycleID);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditCycleDocumentService.DelAuditCycleAsync: {ex.Message}");
            }
        } // DelAuditCycleAsync
    }
}