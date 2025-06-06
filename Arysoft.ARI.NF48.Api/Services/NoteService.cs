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
    public class NoteService
    {
        private readonly BaseRepository<Note> _repository;

        // CONSTRUCTOR

        public NoteService()
        {
            _repository = new BaseRepository<Note>();
        } // NoteService

        // METHODS

        public PagedList<Note> Gets(NoteQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.OwnerID != null)
            {
                items = items.Where(e => e.OwnerID == filters.OwnerID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(e =>
                    (e.Text != null && e.Text.ToLower().Contains(filters.Text))
                    || e.UpdatedUser.ToLower().Contains(filters.Text)
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
                case NoteOrderType.Text:
                    items = items.OrderBy(e => e.Text);
                    break;
                case NoteOrderType.Created:
                    items = items.OrderBy(e => e.Created);
                    break;
                case NoteOrderType.TextDesc:
                    items = items.OrderByDescending(e => e.Text);
                    break;
                case NoteOrderType.CreatedDesc:
                    items = items.OrderByDescending(e => e.Created);
                    break;
                default:
                    items = items.OrderByDescending(e => e.Created);
                    break;
            }

            // Paging

            var pagedItems = PagedList<Note>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Note> GetAsync(Guid id)
        { 
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<Note> AddAsync(Note item)
        {
            // Validations


            if (item.OwnerID == null)
                throw new BusinessException("The Owner is required");

            if (!string.IsNullOrEmpty(item.Text))
            {            
                item.Status = item.Status == StatusType.Nothing
                    ? StatusType.Active
                    : item.Status;
            }
            else
            { 
                item.Status = StatusType.Nothing;
            }

            // Assigning values

            item.ID = Guid.NewGuid();
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
                throw new BusinessException($"Note.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<Note> UpdateAsync(Note item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            //if (item.Status == StatusType.Nothing)
            //    throw new BusinessException("The status is required");

            if (string.IsNullOrEmpty(item.Text))
                throw new BusinessException("The note is empty");

            // Assigning values

            foundItem.Text = item.Text;
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
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Note.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Note item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            if (foundItem.Status == StatusType.Deleted)
            {
                // FileRepository.DeleteDirectory($"~/files/auditors/{foundItem.ID}");
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
                throw new BusinessException($"NoteService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}