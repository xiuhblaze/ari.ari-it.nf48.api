using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using Arysoft.ARI.NF48.Api.Tools;
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

                // Solo si es PreAudit, permitir crearlo si es que no existe
                if (foundItem == null) 
                {
                    if (item.ID == Guid.Empty 
                        && item.AuditStep == AuditStepType.PreAudit 
                        && item.Status == StatusType.Nothing)
                    {
                        await ValidateCreateItemAtList(item);
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

        /// <summary>
        /// Genera los registros de ADCSiteAudit para un ADCSite, de acuerdo al tipo de 
        /// ciclo de auditoría del AppForm
        /// </summary>
        /// <param name="adcSite">ADCSite al cual asociar los ADCSiteAudits</param>
        /// <param name="appForm">AppForm con la información del ciclo de auditoría</param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task AddADCSiteAuditsAsync(ADCSite adcSite, AppForm appForm)
        {
            if (adcSite == null) throw new BusinessException("The ADCSite is required.");
            if (appForm == null) throw new BusinessException("The AppForm is required.");

            bool isMultiSite = appForm.Sites.Count > 1;
            var cycleType = appForm.AuditCycle.CycleType ?? AuditCycleType.Nothing;
            var initialStep = appForm.AuditCycle.InitialStep ?? AuditStepType.Nothing;
            var periodicity = appForm.AuditCycle.Periodicity ?? AuditCyclePeriodicityType.Nothing;

            if (cycleType == AuditCycleType.Nothing
                || (cycleType == AuditCycleType.Transfer && initialStep == AuditStepType.Nothing))
                throw new BusinessException("The Audit Cycle Type or Initial Step are not valid, can't be generate the ADCSiteAudits.");

            if (periodicity == AuditCyclePeriodicityType.Nothing)
                throw new BusinessException("The Audit Cycle Periodicity is not valid, can't be generate the ADCSiteAudits.");
                        
            var stepList = AuditCycleCalculations.GetStepList(cycleType, initialStep, periodicity);

            if (stepList.Count > 0)
            {
                var currentSite = appForm.Sites
                    .Where(s => s.ID == adcSite.SiteID)
                    .FirstOrDefault() ?? new Site();
                bool isOneOrMainSite = !isMultiSite || currentSite.IsMainSite;

                foreach (AuditStepType step in stepList)
                {
                    var adcStepAudit = CreateTmpItem("system");

                    adcStepAudit.ADCSiteID = adcSite.ID;
                    adcStepAudit.Value = isOneOrMainSite; // si es un solo sitio o es el principal, por default en true (el sitio recibe todas las auditorias)
                    adcStepAudit.AuditStep = step;
                    adcStepAudit.Days = isOneOrMainSite && step == AuditStepType.Stage1
                        ? (decimal?)1
                        : null;

                    _repository.Add(adcStepAudit);
                }

                try
                {
                    await _repository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"ADCSiteAuditService.AddADCSiteAuditsToADCSiteAsync: {ex.Message}");
                }
            }
        } // AddADCSiteAuditsAsync

        public async Task SyncADCSiteAuditsAsync(ADCSite adcSite, AppForm appForm)
        {
            if (adcSite == null) throw new BusinessException("The ADCSite is required.");
            if (appForm == null) throw new BusinessException("The AppForm is required.");

            bool isMultiSite = appForm.Sites.Count > 1;
            var cycleType = appForm.AuditCycle.CycleType ?? AuditCycleType.Nothing;
            var initialStep = appForm.AuditCycle.InitialStep ?? AuditStepType.Nothing;
            var periodicity = appForm.AuditCycle.Periodicity ?? AuditCyclePeriodicityType.Nothing;

            if (cycleType == AuditCycleType.Nothing
                || (cycleType == AuditCycleType.Transfer && initialStep == AuditStepType.Nothing))
                throw new BusinessException("The Audit Cycle Type or Initial Step are not valid, can't be generate the ADCSiteAudits.");

            if (periodicity == AuditCyclePeriodicityType.Nothing)
                throw new BusinessException("The Audit Cycle Periodicity is not valid, can't be generate the ADCSiteAudits.");

            var stepList = AuditCycleCalculations
                .GetStepList(cycleType, initialStep, periodicity);

            // Agregar los Steps que no existan
            var currentSite = appForm.Sites
                .Where(s => s.ID == adcSite.SiteID)
                .FirstOrDefault() ?? new Site();
            bool isOneOrMainSite = !isMultiSite || currentSite.IsMainSite;
            var existingSteps = _repository.Gets()
                .Where(x => x.ADCSiteID == adcSite.ID)
                .Select(x => x.AuditStep)
                .ToList();
            foreach (AuditStepType step in stepList)
            {
                if (!existingSteps.Contains(step))
                {   
                    var adcStepAudit = CreateTmpItem("system");

                    adcStepAudit.ADCSiteID = adcSite.ID;
                    adcStepAudit.Value = isOneOrMainSite; // si es un solo sitio o es el principal, por default en true (el sitio recibe todas las auditorias)
                    adcStepAudit.AuditStep = step;
                    adcStepAudit.Days = isOneOrMainSite && step == AuditStepType.Stage1
                        ? (decimal?)1
                        : null;

                    _repository.Add(adcStepAudit);
                }
            }

            // Eliminar los Steps que ya no existan
            foreach (var existingStep in existingSteps)
            {
                if (!stepList.Contains(existingStep ?? AuditStepType.Nothing))
                {
                    var adcStepAuditToDelete = _repository.Gets()
                        .Where(x => x.ADCSiteID == adcSite.ID && x.AuditStep == existingStep)
                        .FirstOrDefault();
                    if (adcStepAuditToDelete != null)
                    {
                        _repository.Delete(adcStepAuditToDelete);
                    }
                }
            }

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCSiteAuditService.SyncADCSiteAuditsAsync: {ex.Message}");
            }
        } // SyncADCSiteAuditsAsync

        // PRIVATE

        private async Task ValidateCreateItemAtList(ADCSiteAudit item)
        {
            // Validations

            if (item.ADCSiteID == Guid.Empty)
                throw new BusinessException("The ADCSite is required.");

            // - Validar que tenga el AuditStep
            if (item.AuditStep == null || item.AuditStep == AuditStepType.Nothing)
                throw new BusinessException("The Audit Step is required.");

            // - Validar que no esté para ese ADCSite en mismo AuditStep
            if (_repository.ExistsAuditStep(
                item.ADCSiteID,
                item.AuditStep ?? AuditStepType.Nothing,
                Guid.Empty))
                throw new BusinessException("The Audit Step already exists for that ADCSite");

            if (!await IsValidAuditStepAsync(item.AuditStep ?? AuditStepType.Nothing, item.ADCSiteID))
                throw new BusinessException("The Audit Step is not valid for the Audit Cycle Type of that ADCSite.");

            // - Si es PreAudit, validar que tenga PreAuditDays
            if (item.AuditStep == AuditStepType.PreAudit)
            {
                if (item.Days == null || item.Days < 0)
                    throw new BusinessException("The PreAudit Days is required for this Audit Step.");
            }
        } // ValidateCreateItemAtList

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
            //var _adcRepository = new ADCRepository();

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

            // - Validar que el AuditStep sea válido para el tipo de AuditCycle del ADCSite
            if (!await IsValidAuditStepAsync(item.AuditStep ?? AuditStepType.Nothing, foundItem.ADCSiteID))
                throw new BusinessException("The Audit Step is not valid for the Audit Cycle Type of that ADCSite.");

            // xBlaze: Por borrar, se mandó al metodo IsValidAuditStepAsync
            //var auditCycleType = await _adcRepository
            //    .GetAuditCycleTypeByADCSiteAuditIDAsync(item.ID);
            //switch (auditCycleType)
            //{
            //    case AuditCycleType.Initial:
            //        if (item.AuditStep != AuditStepType.PreAudit &&
            //            item.AuditStep != AuditStepType.Stage1 &&
            //            item.AuditStep != AuditStepType.Stage2 &&
            //            item.AuditStep != AuditStepType.Surveillance1 &&
            //            item.AuditStep != AuditStepType.Surveillance2 &&
            //            item.AuditStep != AuditStepType.Surveillance3 &&
            //            item.AuditStep != AuditStepType.Surveillance4 &&
            //            item.AuditStep != AuditStepType.Surveillance5)
            //        {
            //            throw new BusinessException("The Audit Step is not valid for the Initial Audit Cycle.");
            //        }
            //        break;
            //    case AuditCycleType.Recertification:
            //        if (item.AuditStep != AuditStepType.Recertification &&
            //            item.AuditStep != AuditStepType.Surveillance1 &&
            //            item.AuditStep != AuditStepType.Surveillance2 &&
            //            item.AuditStep != AuditStepType.Surveillance3 &&
            //            item.AuditStep != AuditStepType.Surveillance4 &&
            //            item.AuditStep != AuditStepType.Surveillance5)
            //        {
            //            throw new BusinessException("The Audit Step is not valid for the Recertification Audit Cycle.");
            //        }
            //        break;
            //    case AuditCycleType.Transfer:
            //        if (item.AuditStep != AuditStepType.Transfer &&
            //            item.AuditStep != AuditStepType.Recertification &&
            //            item.AuditStep != AuditStepType.Surveillance1 &&
            //            item.AuditStep != AuditStepType.Surveillance2 &&
            //            item.AuditStep != AuditStepType.Surveillance3 &&
            //            item.AuditStep != AuditStepType.Surveillance4 &&
            //            item.AuditStep != AuditStepType.Surveillance5)
            //        {
            //            throw new BusinessException("The Audit Step is not valid for the Transfer Audit Cycle.");
            //        }
            //        break;
            //    default:
            //        throw new BusinessException("The Audit Cycle Type is not valid.");
            //}

        } // validateUpdateItem 

        private void SetValuesUpdateItem(ADCSiteAudit item, ADCSiteAudit foundItem)
        {
            foundItem.Value = item.Value;
            foundItem.AuditStep = item.AuditStep;
            foundItem.Days = item.Days ?? 0;
            foundItem.Status = foundItem.Status == StatusType.Nothing && item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status != StatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

        } // SetValuesUpdateItem

        /// <summary>
        /// Determina si el auditStep es válido para el tipo de AuditCycle del ADCSite
        ///  - De acuerdo al tipo de AuditCycle, ver si es valido el AuditStep
        ///    Initial: PreAudit, Stage1, Stage2, Surveillance1-5 (aquí se valida que PreAudit sea solo en Initial)
        ///    Recertificacion: Recertification, Surveillance1-5
        ///    Transfer: Transfer, Recertification, Surveillance1-5
        /// </summary>
        /// <param name="auditStep">Audit step a validar</param>
        /// <param name="adcSiteID">
        /// Identificador del ADCSite para obtener el ADC   con ello el tipo 
        /// de ciclo.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// Autor: xBlaze
        /// Creacion: 19-01-2026
        /// Ultima Modificacion: 19-01-2026
        /// </remarks>
        private async Task<bool> IsValidAuditStepAsync(AuditStepType auditStep, Guid adcSiteID)
        {
            var _adcRepository = new ADCRepository();

            var auditCycleType = await _adcRepository
                .GetAuditCycleTypeByADCSiteIDAsync(adcSiteID);

            switch (auditCycleType)
            { 
                case AuditCycleType.Nothing:
                    throw new BusinessException("The Audit Cycle Type is not valid or was not found.");

                case AuditCycleType.Initial:
                    if (auditStep != AuditStepType.PreAudit &&
                        auditStep != AuditStepType.Stage1 &&
                        auditStep != AuditStepType.Stage2 &&
                        auditStep != AuditStepType.Surveillance1 &&
                        auditStep != AuditStepType.Surveillance2 &&
                        auditStep != AuditStepType.Surveillance3 &&
                        auditStep != AuditStepType.Surveillance4 &&
                        auditStep != AuditStepType.Surveillance5)
                    {
                        return false;
                    }
                    break;
                case AuditCycleType.Recertification:
                    if (auditStep != AuditStepType.Recertification &&
                        auditStep != AuditStepType.Surveillance1 &&
                        auditStep != AuditStepType.Surveillance2 &&
                        auditStep != AuditStepType.Surveillance3 &&
                        auditStep != AuditStepType.Surveillance4 &&
                        auditStep != AuditStepType.Surveillance5)
                    {
                        return false;
                    }
                    break;
                case AuditCycleType.Transfer:
                    if (auditStep != AuditStepType.Transfer &&
                        auditStep != AuditStepType.Recertification &&
                        auditStep != AuditStepType.Surveillance1 &&
                        auditStep != AuditStepType.Surveillance2 &&
                        auditStep != AuditStepType.Surveillance3 &&
                        auditStep != AuditStepType.Surveillance4 &&
                        auditStep != AuditStepType.Surveillance5)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        } // IsValidAuditStepAsync

        #region SHARED HELPERS

        public static ADCSiteAudit CreateTmpItem(string user)
        {
            return new ADCSiteAudit()
            {
                ID = Guid.NewGuid(),
                Status = StatusType.Nothing,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                UpdatedUser = user
            };
        } // CreateTmpItem

        #endregion
    }
}