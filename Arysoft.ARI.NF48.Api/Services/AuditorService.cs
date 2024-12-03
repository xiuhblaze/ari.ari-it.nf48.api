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
    public class AuditorService
    {
        private readonly AuditorRepository _auditorRepository;

        // CONSTRUCTOR

        public AuditorService()
        {
            _auditorRepository = new AuditorRepository();
        }

        // METHODS

        public PagedList<Auditor> Gets(AuditorQueryFilters filters)
        {
            var items = _auditorRepository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(e => 
                    e.FirstName.ToLower().Contains(filters.Text)
                    || e.MiddleName.ToLower().Contains(filters.Text)
                    || e.LastName.ToLower().Contains(filters.Text)
                    || e.Email.ToLower().Contains(filters.Text)
                    || e.Phone.ToLower().Contains(filters.Text)
                    || e.Address.ToLower().Contains(filters.Text)
                );
            }

            if (filters.IsLeader != null && filters.IsLeader != AuditorLeaderType.Nothing)
            {
                switch (filters.IsLeader)
                {
                    case AuditorLeaderType.Leader:
                        items = items.Where(e => e.IsLeadAuditor);
                        break;
                    case AuditorLeaderType.Regular:
                        items = items.Where(e => !e.IsLeadAuditor);
                        break;
                }
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
                case AuditorOrderType.FirstName:
                    items = items.OrderBy(e => e.FirstName)
                        .ThenBy(e => e.MiddleName)
                        .ThenBy(e => e.LastName);
                    break;
                case AuditorOrderType.IsLeader:
                    items = items.OrderBy(e => e.IsLeadAuditor);
                    break;
                case AuditorOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case AuditorOrderType.FirstNameDesc:
                    items = items.OrderByDescending(e => e.FirstName)
                        .ThenByDescending(e => e.MiddleName)
                        .ThenByDescending(e => e.LastName);
                    break;
                case AuditorOrderType.IsLeaderDesc:
                    items = items.OrderByDescending(e => e.IsLeadAuditor);
                    break;
                case AuditorOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
            }

            // Paging

            var pagedItems = PagedList<Auditor>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Auditor> GetAsync(Guid id)
        {
            return await _auditorRepository.GetAsync(id);
        } // GetAsync

        public async Task<Auditor> AddAsync(Auditor item)
        {
            // Validations

            if (string.IsNullOrEmpty(item.UpdatedUser))
            {
                throw new BusinessException("Must specify a username");
            }

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries

            await _auditorRepository.DeleteTmpByUser(item.UpdatedUser);
            _auditorRepository.Add(item);
            _auditorRepository.SaveChanges();

            return item;
        } // AddAsync

        public async Task<Auditor> UpdateAsync(Auditor item)
        {
            // Validations

            if (await _auditorRepository.GetByFullNameAsync(
                item.FirstName, item.MiddleName, item.LastName, item.ID) != null) 
                throw new BusinessException("The name already exist");

            var foundItem = await _auditorRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (item.Status == StatusType.Nothing) item.Status = StatusType.Active;

            // Assigning values

            foundItem.FirstName = item.FirstName;
            foundItem.MiddleName = item.MiddleName;
            foundItem.LastName = item.LastName;
            foundItem.Email = item.Email;
            foundItem.Phone = item.Phone;
            foundItem.Address = item.Address;
            foundItem.PhotoFilename = item.PhotoFilename;
            foundItem.FeePayment = item.FeePayment;
            foundItem.IsLeadAuditor = item.IsLeadAuditor;
            foundItem.Status = foundItem.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Execute queries

            try
            {
                _auditorRepository.Update(foundItem);
                _auditorRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Auditor item)
        {
            var foundItem = await _auditorRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            // ( no hay validaciones por el momento )

            if (foundItem.Status == StatusType.Deleted)
            {
                _auditorRepository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status == StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _auditorRepository.Update(foundItem);
            }

            _auditorRepository.SaveChanges();
        } // DeleteAsync
    }
}