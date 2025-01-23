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
                    (e.FirstName != null && e.FirstName.ToLower().Contains(filters.Text))
                    || (e.MiddleName != null && e.MiddleName.ToLower().Contains(filters.Text))
                    || (e.LastName != null && e.LastName.ToLower().Contains(filters.Text))
                    || (e.Phone != null && e.Phone.ToLower().Contains(filters.Text))
                    || (e.PhoneAlt != null && e.PhoneAlt.ToLower().Contains(filters.Text))
                    || (e.Email != null && e.Email.ToLower().Contains(filters.Text))
                    || (e.Address != null && e.Address.ToLower().Contains(filters.Text))
                    || (e.Position != null && e.Position.ToLower().Contains(filters.Text))
                    || (e.Organization != null && e.Organization.Name.ToLower().Contains(filters.Text))
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

            // Order

            switch (filters.Order)
            {
                case ContactOrderType.FirstName:
                    items = items.OrderBy(e => e.FirstName)
                        .ThenBy(e => e.MiddleName)
                        .ThenBy(e => e.LastName);
                    break;
                case ContactOrderType.IsMainContact:
                    items = items.OrderBy(e => e.IsMainContact)
                        .ThenBy(e => e.FirstName);
                    break;
                case ContactOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case ContactOrderType.FirstNameDesc:
                    items = items.OrderByDescending(e => e.FirstName)
                        .ThenByDescending(e => e.MiddleName)
                        .ThenByDescending(e => e.LastName);
                    break;
                case ContactOrderType.IsMainContactDesc:
                    items = items.OrderByDescending(e => e.IsMainContact)
                        .ThenByDescending(e => e.FirstName);
                    break;
                case ContactOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.FirstName)
                        .ThenBy(e => e.MiddleName)
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
            item.Created = DateTime.UtcNow; 
            item.Updated = DateTime.UtcNow;

            // Execute queries
            try
            {
                await _contactRepository.DeleteTmpByUserAsync(item.UpdatedUser);
                _contactRepository.Add(item);
                await _contactRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Contact.AddAsync: {ex.Message}");
            }
            return item;
        } // AddAsync

        public async Task<Contact> UpdateAsync(Contact item)
        {
            // Validations

            if (item.Status == StatusType.Nothing) item.Status = StatusType.Active;

            // HACK: - Que el nombre no se repita
            // - Que al menos traiga el First name y el Last name
            // - Que el correo sea válido y requerido

            var foundItem = await _contactRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (item.IsMainContact)
            {
                // Poner los demas contactos de la organización en false
                await _contactRepository.SetToNotContactMainAsync(foundItem.OrganizationID);
            }

            // Assigning values

            foundItem.FirstName = item.FirstName;
            foundItem.MiddleName = item.MiddleName;
            foundItem.LastName = item.LastName;
            foundItem.Email = item.Email;
            foundItem.Phone = item.Phone;
            foundItem.PhoneAlt = item.PhoneAlt;
            foundItem.Address = item.Address;
            foundItem.Position = item.Position;
            foundItem.PhotoFilename = item.PhotoFilename;
            foundItem.IsMainContact = item.IsMainContact;
            foundItem.Status = foundItem.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Execute queries

            try
            { 
                _contactRepository.Update(foundItem);
                await _contactRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Contact.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Contact item)
        {
            var foundItem = await _contactRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            //TODO: Que no sea el último contacto

            // Excecute queries

            if (foundItem.Status == StatusType.Deleted)
            {
                _contactRepository.Delete(foundItem);
            }
            else 
            {
                foundItem.Status = foundItem.Status == StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _contactRepository.Update(foundItem);
            }

            try
            {
                await _contactRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Contact.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}