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
    public class PersonService
    {
        private readonly PersonRepository _personRepository;

        // CONSTRUCTOR

        public PersonService()
        {
            _personRepository = new PersonRepository();
        }

        // METHODS

        public PagedList<Person> Gets(PersonQueryFilters filters)
        {
            var items = _personRepository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e =>
                    (e.FirstName != null && e.FirstName.ToLower().Contains(filters.Text))
                    || (e.LastName != null && e.LastName.ToLower().Contains(filters.Text))
                    || (e.Email != null && e.Email.ToLower().Contains(filters.Text))
                    || (e.Phone != null && e.Phone.ToLower().Contains(filters.Text))
                    || (e.LocationDescription != null && e.LocationDescription.ToLower().Contains(filters.Text))
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
                case PersonOrderType.Name:
                    items = items.OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName);
                    break;
                case PersonOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case PersonOrderType.NameDesc:
                    items = items.OrderByDescending(e => e.FirstName)
                        .ThenByDescending(e => e.LastName);
                    break;
                case PersonOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName);
                    break;
            }
        
            // Paging

            var pagedItems = PagedList<Person>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Person> GetAsync(Guid id)
        {
            return await _personRepository.GetAsync(id);
        } // GetAsync

        public async Task<Person> AddAsync(Person item)
        { 

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries

            await _personRepository.DeleteTmpByUser(item.UpdatedUser);
            _personRepository.Add(item);
            await _personRepository.SaveChangesAsync();

            return item;
        } // AddAsync

        public async Task<Person> UpdateAsync(Person item)
        {
            // Validations

            // - Que la persona ya exista - creo que va a ser solo un warning en el frontend

            var foundItem = await _personRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Assigning values

            foundItem.FirstName = item.FirstName;
            foundItem.LastName = item.LastName;
            foundItem.Email = item.Email;
            foundItem.Phone = item.Phone;
            foundItem.PhoneAlt = item.PhoneAlt;
            foundItem.LocationDescription = item.LocationDescription;
            foundItem.Status = foundItem.Status == StatusType.Nothing && item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status != StatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Excecute queries

            _personRepository.Update(foundItem);
            await _personRepository.SaveChangesAsync();

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Person item)
        {
            var foundItem = await _personRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            // - Que no tenga certificados activos, who knows

            if (foundItem.Status == StatusType.Deleted)
            {
                //! Considerar eliminar todas las asociaciones al registro antes de su eliminación tales como
                //  contacts, auditors, users, ...
                _personRepository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status < StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _personRepository.Update(foundItem);
            }
        } // DeleteAsync
    }
}