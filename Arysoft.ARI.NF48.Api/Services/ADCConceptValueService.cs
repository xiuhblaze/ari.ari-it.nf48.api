using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using Arysoft.ARI.NF48.Api.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class ADCConceptValueService
    {
        public readonly ADCConceptValueRepository _repository;

        // CONSTRUCTOR

        public ADCConceptValueService()
        {
            _repository = new ADCConceptValueRepository();
        }

        // METHODS

        public PagedList<ADCConceptValue> Gets(ADCConceptValueQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.ADCConceptID != null && filters.ADCConceptID != Guid.Empty)
            {
                items = items.Where(e => e.ADCConceptID == filters.ADCConceptID);
            }

            if (filters.ADCSiteID != null && filters.ADCSiteID != Guid.Empty)
            {
                items = items.Where(e => e.ADCSiteID == filters.ADCSiteID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(e =>
                    (e.Justification != null && e.Justification.ToLower().Contains(filters.Text))
                    || (e.JustificationApproved != null && e.JustificationApproved.ToLower().Contains(filters.Text)));
            }

            if (filters.Status.HasValue && filters.Status != StatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != StatusType.Nothing)
                    : items.Where(e => e.Status != StatusType.Nothing
                        && e.Status != StatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            {
                case ADCConceptValueOrderType.IndexSort:
                    items = items.OrderBy(e => e.ADCConcept.IndexSort);
                    break;
                case ADCConceptValueOrderType.Value:
                    items = items.OrderBy(e => e.Value);
                    break;
                case ADCConceptValueOrderType.IndexSortDesc:
                    items = items.OrderByDescending(e => e.ADCConcept.IndexSort);
                    break;
                case ADCConceptValueOrderType.ValueDesc:
                    items = items.OrderByDescending(e => e.Value);
                    break;
                default:
                    items = items.OrderBy(e => e.ADCConcept.IndexSort);
                    break;
            }

            var pagedItems = PagedList<ADCConceptValue>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<ADCConceptValue> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<ADCConceptValue> CreateAsync(ADCConceptValue item)
        {
            // Validations

            if (item.ADCConceptID == Guid.Empty)
                throw new BusinessException("The Concept is required");

            if (item.ADCSiteID == Guid.Empty)
                throw new BusinessException("The Site is required");

            // Set Values

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
                throw new BusinessException($"ADCConceptValueService.CreateAsync: {ex.Message}");
            }

            return item;
        } // CreateAsync

        public async Task<ADCConceptValue> UpdateAsync(ADCConceptValue item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to Update was not found");

            // Validations

            if (item.ID == null || item.ID == Guid.Empty)
                throw new BusinessException("The ID is required");

            // - validar contra ADCConcept, que no se salga de los rangos indicados
            if (foundItem.ADCConcept.WhenTrue ?? false && foundItem.ADCConcept.Increase != null)
            {
                // Ver que no se pase del incremento maximo permitido
                if (item.Value > foundItem.ADCConcept.Increase)
                    throw new BusinessException("The Value exceeds the maximum allowed for this Concept");
            }
            else if (!foundItem.ADCConcept.WhenTrue ?? false && foundItem.ADCConcept.Decrease != null)
            {
                // Ver que no se pase del decremento maximo permitido
                if (item.Value > foundItem.ADCConcept.Decrease)
                    throw new BusinessException("The Value exceeds the minimum allowed for this Concept");
            }

            // Set Values

            foundItem.Value = item.Value;
            foundItem.Justification = item.Justification;
            foundItem.ValueApproved = item.ValueApproved;
            foundItem.JustificationApproved = item.JustificationApproved;
            foundItem.ValueUnit = item.ValueUnit;
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
                throw new BusinessException($"ADCConceptValueService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(ADCConceptValue item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to Delete was not found");

            // Validations

            // - ...

            // Excecute queries

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
                throw new BusinessException($"ADCConceptValueService.DeleteAsync: {ex.Message}");
            }
        }
    }
}