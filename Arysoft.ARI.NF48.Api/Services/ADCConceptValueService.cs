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

            ValidateUpdateItem(item, foundItem);
            SetValuesUpdateItem(item, foundItem);

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

        /// <summary>
        /// Obtiene una lista de items a actualizar, valida cada uno de ellos
        /// y los actualiza en la base de datos.        
        /// </summary>
        /// <param name="items">Lista de items a actualizar</param>
        /// <returns>Lista de elementos actualizados incluyendo todos los valores de la bdd</returns>        
        public async Task<List<ADCConceptValue>> UpdateListAsync(List<ADCConceptValue> items)
        {
            if (!(items?.Any() ?? false)) // Valida si la lista es nula o vacía
                throw new BusinessException("The list of ADC Concept Values to Update is empty");

            var areUpdatedItems = false;
            var updatedItems = new List<ADCConceptValue>();

            foreach (var item in items)
            {
                var foundItem = await _repository.GetAsync(item.ID)
                    ?? throw new BusinessException($"One of the records (ADC Concept Value) to Update was not found: { item.ID }");

                ValidateUpdateItem(item, foundItem);
                SetValuesUpdateItem(item, foundItem);
                _repository.Update(foundItem);
                areUpdatedItems = true;
                updatedItems.Add(foundItem);
            }

            if (areUpdatedItems)
            {
                try
                {
                    await _repository.SaveChangesAsync();                    
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"ADCConceptValueService.UpdateListAsync: {ex.Message}");
                }
            }

            return updatedItems;
        } // UpdateListAsync

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
        } // DeleteAsync

        // PRIVATE

        private void ValidateUpdateItem(ADCConceptValue item,ADCConceptValue foundItem)
        {
            // - validar contra ADCConcept, que no se salga de los rangos indicados UPDATE: Validarlo junto con CheckValue
            //if (foundItem.ADCConcept.WhenTrue ?? false && foundItem.ADCConcept.Increase != null)
            //{
            //    // Ver que no se pase del incremento maximo permitido
            //    if (item.Value > foundItem.ADCConcept.Increase)
            //        throw new BusinessException("The Value exceeds the maximum allowed for this Concept");
            //}
            //else if (!foundItem.ADCConcept.WhenTrue ?? false && foundItem.ADCConcept.Decrease != null)
            //{
            //    // HACK: Update (xBlaze:20250710) - El decremento se va a medir en valores de 5, 10, 15 y 20% para todos!
            //    // Ver que no se pase del decremento maximo permitido
            //    if (item.Value > foundItem.ADCConcept.Decrease)
            //        throw new BusinessException("The Value exceeds the minimum allowed for this Concept");
            //}

            // TODO: - validar si el checkValue coincida con el incremento o decremento correspondiente

        } // ValidateUpdateItem

        /// <summary>
        /// Asigna los valores que se van a actualizar en el item encontrado
        /// </summary>
        /// <param name="item">Nuevos valores</param>
        /// <param name="foundItem">Item encontrado</param>
        private void SetValuesUpdateItem(ADCConceptValue item, ADCConceptValue foundItem)
        {

            foundItem.CheckValue = item.CheckValue;
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
        } // SetValuesUpdateItem
    }
}