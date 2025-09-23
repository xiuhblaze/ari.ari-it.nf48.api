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
    public class ADCConceptService
    {
        public readonly ADCConceptRepository _repository;

        // CONSTRUCTOR

        public ADCConceptService()
        {
            _repository = new ADCConceptRepository();
        }

        // METHODS

        public PagedList<ADCConcept> Gets(ADCConceptQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.StandardID != null && filters.StandardID != Guid.Empty)
            {
                items = items.Where(e => e.StandardID == filters.StandardID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(e =>                    
                    (e.Description != null && e.Description.ToLower().Contains(filters.Text))
                    // || (e.UpdatedUser != null && e.UpdatedUser.ToLower().Contains(filters.Text))
                );
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
                case ADCConceptOrderType.IndexSort:
                    items = items.OrderBy(i => i.IndexSort);
                    break;
                case ADCConceptOrderType.Description:
                    items = items.OrderBy(i => i.Description);
                    break;
                case ADCConceptOrderType.Standard:
                    items = items.OrderBy(i => i.Standard.Name)
                        .ThenBy(i => i.IndexSort);
                    break;
                case ADCConceptOrderType.IndexSortDesc:
                    items = items.OrderByDescending(i => i.IndexSort);
                    break;
                case ADCConceptOrderType.DescriptionDesc:
                    items = items.OrderByDescending(i => i.Description);
                    break;
                case ADCConceptOrderType.StandardDesc:
                    items = items.OrderByDescending(i => i.Standard.Name)
                        .ThenByDescending(i => i.IndexSort);
                    break;
                default:
                    items = items.OrderBy(i => i.IndexSort)
                        .ThenBy(i => i.Description);
                    break;
            }

            var pagedItems = PagedList<ADCConcept>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        /// <summary>
        /// Obtiene un concepto de ADC por su ID
        /// </summary>
        /// <param name="id">Identificador del ADC Concept</param>
        /// <returns></returns>
        public async Task<ADCConcept> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<ADCConcept> AddAsync(ADCConcept item)
        {
            // Validations

            // Set Values

            item.ID = Guid.NewGuid();
            item.IndexSort = 1000; // Default value for new items
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
                throw new BusinessException($"ADCConceptService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<ADCConcept> UpdateAsync(ADCConcept item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            if (foundItem.Status == StatusType.Nothing) // Si es nuevo...
            {
                if (item.StandardID == null || item.StandardID == Guid.Empty)
                    throw new BusinessException("The Standard ID is required for a new ADC Concept");

                foundItem.StandardID = item.StandardID; // Solo cuando es nuevo, se asigna
            }

            // - Al menos debe de haber un incremento o decremento
            if (item.Increase == null && item.Decrease == null)
                throw new BusinessException("At least one of Increase or Decrease must be specified");


            // - Si tiene increase, debe de tener la unidad
            if (item.Increase != null && item.IncreaseUnit == null)
                throw new BusinessException("The Increase Unit is required when Increase is specified");

            // - Si tiene decrease, debe de tener la unidad
            if (item.Decrease != null && item.DecreaseUnit == null)
                throw new BusinessException("The Decrease Unit is required when Decrease is specified");

            // - El indice cambió, reordenar todos los Conceptos activos
            //   del mismo Standard
            if (item.IndexSort.HasValue && item.IndexSort != foundItem.IndexSort)
            {
                _repository.ReorderByIndex(foundItem.StandardID, item.IndexSort ?? 1000, item.ID);
                foundItem.IndexSort = item.IndexSort;
            }

            // Set Values

            foundItem.Description = item.Description;
            foundItem.WhenTrue = item.WhenTrue;
            foundItem.Increase = item.Increase;
            foundItem.Decrease = item.Decrease;
            foundItem.IncreaseUnit = item.IncreaseUnit; 
            foundItem.DecreaseUnit = item.DecreaseUnit;
            foundItem.ExtraInfo = item.ExtraInfo;
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
                throw new BusinessException($"ADCConceptService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(ADCConcept item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            if (foundItem.Status == StatusType.Active) // Va a cambiar a inactive
            {
                // - Rehacer el indice con el resto de elementos
                _repository.ReorderByIndex(foundItem.StandardID, 0, Guid.Empty);
            }

            if (foundItem.Status == StatusType.Deleted)
            {
                // TODO: Ver si se necesita alguna validación

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

            // Execute queries
            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCConceptService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}