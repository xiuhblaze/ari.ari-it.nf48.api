using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class CompanyService : IDisposable
    {
        public readonly CompanyRepository _repository;

        // CONSTRUCTOR 

        public CompanyService()
        {
            _repository = new CompanyRepository();
        }

        // METHODS

        public PagedList<Company> Gets(CompanyQueryFilters filters)
        {
            // Query sin ejecutar: todo lo siguiente se traduce a SQL
            var items = _repository.Query();
            //var items = _repository.Gets();

            // Filters

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty)
            {
                var organizationID = filters.OrganizationID.Value;
                items = items.Where(e => e.OrganizationID == organizationID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            { 
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e =>
                    (e.Name != null && e.Name.ToLower().Contains(filters.Text))
                    || (e.LegalEntity != null && e.LegalEntity.ToLower().Contains(filters.Text))
                );
            }

            if (filters.Status != null && filters.Status != StatusType.Nothing)
            {
                var status = filters.Status.Value;
                items = items.Where(e => e.Status == status);
            }
            else
            {
                var includeDeleted = filters.IncludeDeleted ?? false;
                items = includeDeleted
                    ? items.Where(e => e.Status != StatusType.Nothing)
                    : items.Where(e => e.Status != StatusType.Nothing && e.Status != StatusType.Deleted);
            }

            // Order (siempre debe haber OrderBy antes de Skip/Take en EF6)

            switch (filters.Order)
            {
                case CompanyOrderType.Name:
                    items = items.OrderBy(e => e.Name); 
                    break;
                case CompanyOrderType.LegalEntity:
                    items = items.OrderBy(e => e.LegalEntity);
                    break;
                case CompanyOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case CompanyOrderType.NameDesc:
                    items = items.OrderByDescending(e => e.Name);
                    break;
                case CompanyOrderType.LegalEntityDesc:
                    items = items.OrderByDescending(e => e.LegalEntity);
                    break;
                case CompanyOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.Name);
                    break;
            }

            // Paging

            var pagedItems = PagedList<Company>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Company> GetAsync(Guid id)
        { 
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<Company> CreateAsync(Company item)
        {
            // Validations

            if (item.OrganizationID == null || item.OrganizationID == Guid.Empty)
                throw new BusinessException("Must first assign Organization");

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

                // item es un "new Company" (no un proxy), el lazy loading no aplica:
                // se carga explícitamente para que el DTO de detalle traiga Organization
                await _repository.LoadReferenceAsync(item, c => c.Organization);
            }
            catch (Exception ex)
            {
                throw new BusinessException(
                    $"CompanyService.CreateAsync: {ex.Message}", 
                    HttpStatusCode.InternalServerError, 
                    ex);
            }

            return item;
        } // CreateAsync

        public async Task<Company> UpdateAsync(Company item)
        { 
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("Company to update not found");

            // Validations

            // - No duplicar el nombre de la compañia en la misma organización
            if (await _repository.ExistCompanyNameAsync(item.Name, foundItem.OrganizationID, item.ID))
                throw new BusinessException("The name already exists");

            // - No duplicar el Legal Entity en cualquier organización
            //if (await _repository.ExistLegalEntityAsync(item.LegalEntity, item.ID))
            //    throw new BusinessException("The Legal Entity already exists");

            // Assigning values

            if (item.Status == StatusType.Nothing)
                item.Status = StatusType.Active;

            foundItem.Name = item.Name;
            foundItem.LegalEntity = item.LegalEntity;
            //foundItem.COID = item.COID;
            foundItem.Status = foundItem.Status == StatusType.Nothing && item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status != StatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            try {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException(
                    $"CompanyService.UpdateAsync: {ex.Message}",
                    HttpStatusCode.InternalServerError,
                    ex);
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Company item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("Company to delete not found");

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
                throw new BusinessException(
                    $"CompanyService.DeleteAsync: {ex.Message}",
                    HttpStatusCode.InternalServerError,
                    ex);
            }
        } // DeleteAsync

        // DISPOSE

        public void Dispose()
        {
            _repository.Dispose();
        } // Dispose
    }
}