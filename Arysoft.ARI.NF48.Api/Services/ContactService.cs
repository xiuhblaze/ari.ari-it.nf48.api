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
    public class ContactService
    {
        private readonly ContactRepository _contactRepository;

        // CONSTRUCTOR

        public ContactService()
        {
            _contactRepository = new ContactRepository();
        }

        // METHODS

        public PagedList<Contact> Gets(ContactQueryFilters filters)
        {
            var items = _contactRepository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(e =>
                    e.FirstName.ToLower().Contains(filters.Text)
                    || e.LastName.ToLower().Contains(filters.Text)
                    || e.Phone.ToLower().Contains(filters.Text)
                    || e.Position.ToLower().Contains(filters.Text)
                );
            }

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty)
            {
                items = items.Where(e => e.OrganizationID == filters.OrganizationID);
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

            // Orden

            switch (filters.Order)
            {
                case ContactOrderType.FirstName:
                    items = items.OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName);
                    break;
                case ContactOrderType.LastName:
                    items = items.OrderBy(e => e.LastName)
                        .ThenBy(e => e.FirstName);
                    break;
                case ContactOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case ContactOrderType.FirstNameDesc:
                    items = items.OrderByDescending(e => e.FirstName)
                        .ThenByDescending(e => e.LastName);
                    break;
                case ContactOrderType.LastNameDesc:
                    items = items.OrderByDescending(e => e.LastName)
                        .ThenByDescending(e => e.FirstName);
                    break;
                case ContactOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName);
                    break;
            }

            // Paging

            var pagedItems = PagedList<Contact>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Contact> GetAsync(Guid id)
        { 
            return await _contactRepository.GetAsync(id);
        } // GetAsync

        public async Task<Contact> AddAsync(Contact item)
        {
            // Validations

            if (item.OrganizationID == Guid.Empty)
            {
                throw new BusinessException("Must first assign Organization");
            }

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.Now; 
            item.Updated = DateTime.Now;

            // Execute queries

            await _contactRepository.DeleteTmpByUser(item.UpdatedUser);
            _contactRepository.Add(item);
            await _contactRepository.SaveChangesAsync();

            return item;
        } // AddAsync

        public async Task<Contact> UpdateAsync(Contact item)
        {
            // Validations

            // HACK: - Que el nombre no se repita

            var foundItem = await _contactRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Assigning values

            foundItem.FirstName = item.FirstName;
            foundItem.LastName = item.LastName;
            foundItem.Phone = item.Phone;
            foundItem.PhoneExtensions = item.PhoneExtensions;
            foundItem.Email = item.Email;
            foundItem.Position = item.Position;
            foundItem.Status = foundItem.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;
            foundItem.Updated = DateTime.Now;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Execute queries

            _contactRepository.Update(foundItem);
            await _contactRepository.SaveChangesAsync();

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Contact item)
        {
            var foundItem = await _contactRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - Que no sea el único contacto

            if (foundItem.Status == StatusType.Deleted)
            {
                _contactRepository.Delete(foundItem);
            }
            else 
            {
                foundItem.Status = foundItem.Status == StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.Now;
                foundItem.UpdatedUser = item.UpdatedUser;

                _contactRepository.Update(foundItem);
            }
        } // DeleteAsync
    }
}