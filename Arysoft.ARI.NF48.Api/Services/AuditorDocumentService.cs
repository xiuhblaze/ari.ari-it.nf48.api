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
    public class AuditorDocumentService
    {
        private readonly AuditorDocumentRepository _repository;

        // CONSTRUCTOR

        public AuditorDocumentService()
        {
            _repository = new AuditorDocumentRepository();
        } // AuditorDocumentService

        // METHODS

        public PagedList<AuditorDocument> Gets(AuditorDocumentQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.AuditorID != null)
            {
                items = items.Where(e => e.AuditorID == filters.AuditorID);
            }

            if (filters.CatAuditorDocumentID != null)
            {
                items = items.Where(e => e.CatAuditorDocumentID == filters.CatAuditorDocumentID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => 
                    e.Observations != null && e.Observations.ToLower().Contains(filters.Text)
                );
            }

            if (filters.DueDateStart != null)
            {
                items = items.Where(e => e.DueDate >= filters.DueDateStart);
            }

            if (filters.DueDateEnd != null)
            {
                items = items.Where(e => e.DueDate <= filters.DueDateEnd);
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

            switch(filters.Order)
            {
                case AuditorDocumentOrderType.StartDate:
                    items = items.OrderBy(e => e.StartDate); 
                    break;
                case AuditorDocumentOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case AuditorDocumentOrderType.StartDateDesc:
                    items = items.OrderByDescending(e => e.StartDate);
                    break;
                case AuditorDocumentOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.StartDate);
                    break;
            }

            // Paging

            var pagedItems = PagedList<AuditorDocument>
                .Create(items, filters.PageNumber, filters.PageSize);

            //// Valida si ya pasó su fecha de termino, para marcar el documento como inactivo
            //var hasChanges = false;
            //foreach (var item in pagedItems) 
            //{
            //    if (DateTime.Compare((DateTime)item.DueDate, DateTime.Today) < 0) 
            //    {
            //        item.Status = StatusType.Inactive;
            //        _repository.Update(item);
            //        hasChanges = true;
            //    }                
            //}
            //if (hasChanges) _repository.SaveChanges();

            return pagedItems;
        } // Gets

        public async Task<AuditorDocument> GetAsync(Guid id)
        {
            var item = await _repository.GetAsync(id);

            //if (item.DueDate != null && DateTime.Compare((DateTime)item.DueDate, DateTime.Today) < 0)
            //{ 
            //    item.Status = StatusType.Inactive;
            //    _repository.Update(item);
            //    _repository.SaveChanges();
            //}

            return item;
        } // GetAsync

        public async Task<AuditorDocument> AddAsync(AuditorDocument item)
        {
            // Validations

            if (item.AuditorID == null)
                throw new BusinessException("Must specify auditor");

            if (item.CatAuditorDocumentID == null)
                throw new BusinessException("Must specify document type");

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries

            try
            {
                await _repository.DeleteTmpByUser(item.UpdatedUser);
                _repository.Add(item);
                _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditorDocumentService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<AuditorDocument> UpdateAsync(AuditorDocument item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - Que la fecha de inicio (start) no sea mayor que la de termino (due)
            if (DateTime.Compare((DateTime)item.StartDate, (DateTime)item.DueDate) > 0)
                throw new BusinessException("Due date can't be before Start date");

            //// - Si la fecha de termino es menor a hoy, automáticamente el Status sea Inactive
            //if (DateTime.Compare((DateTime)item.DueDate, DateTime.Today) < 0)
            //{ 
            //    item.Status = StatusType.Inactive;
            //    foundItem.Status = StatusType.Inactive;
            //}

            // - Si el documento es el activo, inactiva los demas
            if (item.Status == StatusType.Nothing || item.Status == StatusType.Active) {
                item.Status = StatusType.Active;
                await _repository.SetToInactiveDocumentsAsync(foundItem.AuditorID, foundItem.CatAuditorDocumentID);
            }

            // Assigning values

            if (item.Status == StatusType.Nothing) item.Status = StatusType.Active;

            foundItem.Filename = item.Filename;
            foundItem.StartDate = item.StartDate;
            foundItem.DueDate = item.DueDate;
            foundItem.Observations = item.Observations;
            foundItem.Type = item.Type;
            foundItem.Status = item.Status;
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
                throw new BusinessException($"AuditorDocumentService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(AuditorDocument item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - no validations yet
            // - Considerar eliminar le archivo cuando se va a eliminar físicamente el registro

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