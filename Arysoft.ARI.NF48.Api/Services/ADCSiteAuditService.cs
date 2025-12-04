using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class ADCSiteAuditService
    {
        public readonly ADCSiteAuditRepository _repository;

        // CONSTRUCTOR

        public ADCSiteAuditService()
        { 
            _repository = new ADCSiteAuditRepository();
        }

        // METHODS

        public PagedList<ADCSiteAudit> Gets(ADCSiteAuditQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.ADCSiteID.HasValue)
                items = items.Where(x => x.ADCSiteID == filters.ADCSiteID);

            if (filters.Status.HasValue && filters.Status != StatusType.Nothing)
            {
                items = items.Where(x => x.Status == filters.Status);
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
                case ADCSiteAuditOrderType.AuditStep:  
                    items = items.OrderBy(x => x.AuditStep);
                    break;
                case ADCSiteAuditOrderType.Value:
                    items = items.OrderBy(x => x.Value);
                    break;
                case ADCSiteAuditOrderType.Status:
                    items = items.OrderBy(x => x.Status);
                    break;
                case ADCSiteAuditOrderType.AuditStepDesc:
                    items = items.OrderByDescending(x => x.AuditStep);
                    break;
                case ADCSiteAuditOrderType.ValueDesc:
                    items = items.OrderByDescending(x => x.Value);
                    break;
                case ADCSiteAuditOrderType.StatusDesc:
                    items = items.OrderByDescending(x => x.Status);
                    break;
                default:
                    items = items.OrderBy(x => x.AuditStep);
                    break;
            }
            
            var pagedItems = PagedList<ADCSiteAudit>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<ADCSiteAudit> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync    

        public async Task<ADCSiteAudit> CreateAsync(ADCSiteAudit item)
        {
            // Validations

            if (item.ADCSiteID == Guid.Empty)
                throw new ArgumentException("The ADCSite is required.");

            // Set values

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
                throw new BusinessException($"ADCSiteAuditService.CreateAsync: {ex.Message}");
            }

            return item;
        } // CreateAsync

        public async Task<ADCSiteAudit> UpdateAsync(ADCSiteAudit item)
        { 
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to Update was not found");

            await ValidateUpdateItemAsync(item, foundItem);
            SetValuesUpdateItem(item, foundItem);

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();                
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCSiteAuditService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task<List<ADCSiteAudit>> UpdateListAsync(List<ADCSiteAudit> items)
        {
            if (!(items?.Any() ?? false)) // Valida si la lista es nula o vacía
                throw new BusinessException("The list of ADC Site Audits to Update is empty");

            var areUpdatedItems = false;
            var updatedItems = new List<ADCSiteAudit>();

            foreach (var item in items)
            {
                ADCSiteAudit foundItem;

                foundItem = await _repository.GetAsync(item.ID);

                // Solo si es PreAudit, permitir crearlo sino existe
                if (foundItem == null)
                {
                    if (item.AuditStep == AuditStepType.PreAudit)
                    {
                        await ValidateUpdateItemAsync(item, new ADCSiteAudit());

                        foundItem = CreatePreAuditItem(item);
                        _repository.Add(foundItem);
                        areUpdatedItems = true;
                    }
                    else throw new BusinessException($"One of the records (ADC Site Audit) to Update was not found: {item.ID}");
                }
                else
                {
                    await ValidateUpdateItemAsync(item, foundItem);
                    SetValuesUpdateItem(item, foundItem);
                    _repository.Update(foundItem);
                    areUpdatedItems = true;
                    updatedItems.Add(foundItem);
                }

            }

            if (areUpdatedItems)
            { 
                try
                {
                    await _repository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"ADCSiteAuditService.UpdateListAsync: {ex.Message}");
                }
            }

            return updatedItems;
        } // UpdateListAsync

        public async Task DeleteAsync(ADCSiteAudit item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to Delete was not found");

            // Validations
            // - aun no tengo validaciones

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
                throw new BusinessException($"ADCSiteAuditService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync

        // PRIVATE

        private ADCSiteAudit CreatePreAuditItem(ADCSiteAudit item)
        {
            item.ID = Guid.NewGuid();
            item.Status = StatusType.Active;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            return item;
        } // CreatePreAuditItem

        private async Task ValidateUpdateItemAsync(ADCSiteAudit item, ADCSiteAudit foundItem)
        {
            var _adcRepository = new ADCRepository();

            // Validations

            // - Validar que tenga el AuditStep
            if (item.AuditStep == null || item.AuditStep == AuditStepType.Nothing)
                throw new BusinessException("The Audit Step is required.");

            // - Validar que no esté para ese ADCSite en mismo AuditStep
            if (_repository.ExistsAuditStep(
                item.ADCSiteID, 
                item.AuditStep ?? AuditStepType.Nothing, 
                item.ID))
                throw new BusinessException("The Audit Step already exists for that ADCSite");

            // - De acuerdo al tipo de AuditCycle, ver si es valido el AuditStep
            //   Initial: PreAudit, Stage1, Stage2, Surveillance1-5 (aquí se valida que PreAudit sea solo en Initial)
            //   Recertificacion: Recertification, Surveillance1-5
            //   Transfer: Transfer, Recertification, Surveillance1-5

            var auditCycleType = await _adcRepository
                .GetAuditCycleTypeByADCSiteAuditIDAsync(item.ID);
            switch (auditCycleType)
            {
                case AuditCycleType.Initial:
                    if (item.AuditStep != AuditStepType.PreAudit &&
                        item.AuditStep != AuditStepType.Stage1 &&
                        item.AuditStep != AuditStepType.Stage2 &&
                        item.AuditStep != AuditStepType.Surveillance1 &&
                        item.AuditStep != AuditStepType.Surveillance2 &&
                        item.AuditStep != AuditStepType.Surveillance3 &&
                        item.AuditStep != AuditStepType.Surveillance4 &&
                        item.AuditStep != AuditStepType.Surveillance5)
                    {
                        throw new BusinessException("The Audit Step is not valid for the Initial Audit Cycle.");
                    }
                    break;
                case AuditCycleType.Recertificacion:
                    if (item.AuditStep != AuditStepType.Recertification &&
                        item.AuditStep != AuditStepType.Surveillance1 &&
                        item.AuditStep != AuditStepType.Surveillance2 &&
                        item.AuditStep != AuditStepType.Surveillance3 &&
                        item.AuditStep != AuditStepType.Surveillance4 &&
                        item.AuditStep != AuditStepType.Surveillance5)
                    {
                        throw new BusinessException("The Audit Step is not valid for the Recertification Audit Cycle.");
                    }
                    break;
                case AuditCycleType.Transfer:
                    if (item.AuditStep != AuditStepType.Transfer &&
                        item.AuditStep != AuditStepType.Recertification &&
                        item.AuditStep != AuditStepType.Surveillance1 &&
                        item.AuditStep != AuditStepType.Surveillance2 &&
                        item.AuditStep != AuditStepType.Surveillance3 &&
                        item.AuditStep != AuditStepType.Surveillance4 &&
                        item.AuditStep != AuditStepType.Surveillance5)
                    {
                        throw new BusinessException("The Audit Step is not valid for the Transfer Audit Cycle.");
                    }
                    break;
                default:
                    throw new BusinessException("The Audit Cycle Type is not valid.");
            }

        } // validateUpdateItem 

        private void SetValuesUpdateItem(ADCSiteAudit item, ADCSiteAudit foundItem)
        {
            foundItem.Value = item.Value;
            foundItem.AuditStep = item.AuditStep;
            if (item.AuditStep == AuditStepType.PreAudit)
                foundItem.PreAuditDays = item.PreAuditDays;
            if (item.AuditStep == AuditStepType.Stage1)
                foundItem.Stage1Days = item.Stage1Days;
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