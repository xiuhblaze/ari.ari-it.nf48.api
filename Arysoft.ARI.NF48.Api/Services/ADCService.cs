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
using System.Text.Json;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class ADCService
    {
        public readonly ADCRepository _repository;
        public readonly ADCSiteAuditService _adcSiteAuditService;

        // CONSTRUCTOR

        public ADCService()
        {
            _repository = new ADCRepository();
            _adcSiteAuditService = new ADCSiteAuditService();
        }

        // METHODS

        #region " CRUD "

        public PagedList<ADC> Gets(ADCQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty)
            { 
                items = items.Where(e => e.AppForm.AuditCycle.OrganizationID == filters.OrganizationID);
            }

            if (filters.AuditCycleID != null && filters.AuditCycleID != Guid.Empty)
            { 
                items = items.Where(e => e.AppForm.AuditCycleID == filters.AuditCycleID);
            }

            if (filters.AppFormID != null && filters.AppFormID != Guid.Empty)
            {
                items = items.Where(e => e.AppFormID == filters.AppFormID);
            }

            if (filters.Status.HasValue && filters.Status != ADCStatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != ADCStatusType.Nothing)
                    : items.Where(e => e.Status != ADCStatusType.Nothing
                        && e.Status != ADCStatusType.Deleted);
            }

            foreach (var item in items)
            {
                if (item.Status < ADCStatusType.Inactive)
                { 
                    item.Alerts = GetAlertsAsync(item)
                        .GetAwaiter()
                        .GetResult();
                }
            }

            // Order

            switch (filters.Order)
            {
                case ADCOrderType.Created:
                    items = items.OrderBy(e => e.Created);
                    break;
                case ADCOrderType.CreatedDesc:
                    items = items.OrderByDescending(e => e.Created);
                    break;
                default:
                    items = items.OrderByDescending(e => e.Created);
                    break;
            }
            
            var pagedItems = PagedList<ADC>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<ADC> GetAsync(Guid id, bool asNoTracking = false)
        {
            var item = await _repository.GetAsync(id, asNoTracking)
                ?? throw new BusinessException("The ADC was not found.");

            if (item.Status < ADCStatusType.Inactive)
            { 
                var alerts = await GetAlertsAsync(item);

                if (alerts.Count > 0)
                {
                    if (alerts.Contains(ADCAlertType.SitesMistmatch)
                        ||alerts.Contains(ADCAlertType.CycleTypeMistmatch))
                    { 
                        await UpdateSitesToExistingADCAsync(item);
                        _repository.DetachAllEntities();
                        item = await _repository.GetAsync(item.ID)
                            ?? throw new BusinessException("The ADC was not found after update sites.");
                    }

                    //NOTA: Aun para las otras alertas no necesita algun proceso en
                    //      particular, con hacer RefreshInitialData... es suficiente

                    if (item.AppForm.Standard.StandardBase == StandardBaseType.ISO22K)
                        item = await RefreshInitialDataISO22KAsync(item);
                    else
                        item = await RefreshInitialDataAsync(item);

                    try
                    {
                        _repository.UpdateValues(item);
                        await _repository.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new BusinessException($"ADCService.GetAsync.Update.RecalcularTotales: {ex.Message}");
                    }

                    _repository.DetachAllEntities();
                    item = await _repository.GetAsync(item.ID)
                        ?? throw new BusinessException("The ADC was not found after and recalculation.");
                }

                item.Alerts = alerts;
            }

            return item;
        } // GetAsync

        public async Task<ADC> AddAsync(ADC item)
        {
            var _appFormRepository = new AppFormRepository();
            var _tmpADCRepository = new ADCRepository();

            // Validations

            if (item.AppFormID == null || item.AppFormID == Guid.Empty)            
                throw new BusinessException("The Application Form ID is required.");

            var appForm = await _appFormRepository.GetAsync(item.AppFormID)
                ?? throw new BusinessException("The Application Form was not found.");

            await ValidateCreateItemAsync(item, appForm);
            item = await SetValuesCreateItemAsync(item, appForm);

            try
            { 
                await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
                _repository.Add(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCService.AddAsync: {ex.Message}");
            }

            // Agregar los Sites del AppForm al ADC en ADCSites
            if (appForm.Standard.StandardBase == StandardBaseType.ISO22K)
            {
                await ADCSitesToNewISO22KADCAsync(item);
            }
            else
            {
                await AddSitesToNewADCAsync(item); 
            }

            // Recargar todo el item para recalcular los totales y MD5,
            // pues los ADCSites se agregaron en la bdd
            var tmpItem = await _tmpADCRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The ADC was not found after creation.");

            if (appForm.Standard.StandardBase == StandardBaseType.ISO22K)
                tmpItem = await RefreshInitialDataISO22KAsync(tmpItem);
            else
                tmpItem = await RefreshInitialDataAsync(tmpItem);
            _tmpADCRepository.Update(tmpItem);

            try
            {
                // _tmpADCRepository.UpdateValues(tmpItem); // No se necesita
                await _tmpADCRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCService.AddAsync.Update.RecalcularTotales: {ex.Message}");
            }

            return tmpItem;
        } // AddAsync

        public async Task<ADC> UpdateAsync(ADC item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            await ValidateUpdateItemAsync(item, foundItem);
            var toUpdateItem = await SetValuesUpdateItemAsync(item, foundItem);

            toUpdateItem.Alerts = await GetAlertsAsync(toUpdateItem);

            try
            {
                _repository.Update(toUpdateItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCService.UpdateAsync: {ex.Message}");
            }

            return toUpdateItem;
        } // UpdateAsync

        public async Task DeleteAsync(ADC item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            if (foundItem.Status == ADCStatusType.Deleted) // Eliminación física
            {
                _repository.Delete(foundItem);
            }
            else // Eliminación lógica
            {
                if (string.IsNullOrEmpty(foundItem.HistoricalDataJSON))
                    foundItem.HistoricalDataJSON = await GetHistoricalDataJSONAsync(foundItem);

                foundItem.Status = foundItem.Status < ADCStatusType.Cancel
                    ? ADCStatusType.Cancel
                    : ADCStatusType.Deleted;
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
                throw new BusinessException($"ADCService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync

        #endregion

        #region " CASOS DE USO "

        /// <summary>
        /// Establece el status de un ADC a Inactive
        /// </summary>
        /// <param name="appFormID">Identificador del AppForm</param>
        /// <param name="updaterUser">Usuario que realiza la actualización</param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task SetToInactiveFromAppFormAsync(Guid appFormID, string updaterUser)
        {
            var foundItem = _repository.Gets()
                .Where(a => a.AppFormID == appFormID)
                .FirstOrDefault();

            if (foundItem != null) 
            {
                foundItem.HistoricalDataJSON = await GetHistoricalDataJSONAsync(foundItem);
                foundItem.Status = ADCStatusType.Inactive;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = updaterUser;

                _repository.Update(foundItem);

                try
                {
                    await _repository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"ADCService.SetToInactiveFromAppFormAsync: {ex.Message}");
                }
            }
        } // SetToInactiveFromAppFormAsync

        // PROPOSAL

        /// <summary>
        /// Actualiza el ADC con el valor del ID de la Propuesta creando la relación
        /// </summary>
        /// <param name="adcID"></param>
        /// <param name="proposalID"></param>
        /// <param name="updatedUser"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task UpdateProposalIDAsync(Guid adcID, Guid proposalID, string updatedUser)
        {
            var foundItem = await _repository.GetAsync(adcID)
                ?? throw new BusinessException("The ADC to update was not found.");

            if (proposalID == Guid.Empty)
                throw new BusinessException("The Proposal ID is required.");


            if (foundItem.Status != ADCStatusType.Active)
                throw new BusinessException("Only Active ADCs can be linked to a Proposal.");

            foundItem.ProposalID = proposalID;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = updatedUser;

            try
            {
                _repository.UpdateValues(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCService.UpdateProposalIDAsync: {ex.Message}");
            }
        } // UpdateProposalIDAsync

        /// <summary>
        /// Remueve el ID de la propuesta de un ADC eliminando la relación.
        /// </summary>
        /// <param name="adcID"></param>
        /// <param name="updatedUser"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task RemoveProposalIDAsync(Guid adcID, string updatedUser)
        {
            var foundItem = await _repository.GetAsync(adcID)
                ?? throw new BusinessException("The ADC to update was not found.");

            if (foundItem.Status > ADCStatusType.Active)
                throw new BusinessException("Only active ADCs can be unliked from a proposal.");

            foundItem.ProposalID = null;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = updatedUser;

            try
            {
                _repository.UpdateValues(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCService.RemoveProposalIDAsync: {ex.Message}");
            }
        } // RemoveProposalIDAsync

        #endregion // CASOS DE USO

        #region " PRIVATE "

        // - Create Item

        /// <summary>
        /// Validaciones para crear un nuevo ADC, solo genera excepciones,
        /// no necesita devolver valor alguno
        /// </summary>
        /// <param name="item">item ADC que traer los valores minimos para crear el ADC</param>
        /// <param name="appForm">item AppForm del que depende el ADC</param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        /// <remarks>
        /// Autor: xBlaze
        /// Creacion: 21-01-2026
        /// Ultima Modificacion: 2026-07-31
        /// </remarks>
        private async Task ValidateCreateItemAsync(ADC item, AppForm appForm)
        {
            // Validations

            // Validar que el AppForm sea active

            if (appForm.Status != AppFormStatusType.Active)
                throw new BusinessException("The Application Form must be Active to create an ADC.");

            // Validar que el AppForm no tenga un ADC
            if (appForm.ADCs.Any(adc => adc.Status > ADCStatusType.Nothing
                && adc.Status < ADCStatusType.Cancel))
                throw new BusinessException("The Application Form already has a valid ADC");

            // Validar que el AppForm no tenga un CycleYear igual al del appForm valido
            if (appForm.ADCs.Any(adc => adc.Status > ADCStatusType.Nothing
                && adc.Status < ADCStatusType.Cancel
                && adc.CycleYear == appForm.CycleYear))
                throw new BusinessException("The Application Form already has a ADC with the same Cycle Year");

            // Validar que sea de un Standard valido, por lo pronto:
            // ISO 9001, ISO 14001, ISO 22000, ISO 45001
            if (appForm.Standard.StandardBase != StandardBaseType.ISO9K 
                && appForm.Standard.StandardBase != StandardBaseType.ISO14K
                && appForm.Standard.StandardBase != StandardBaseType.ISO22K
                && appForm.Standard.StandardBase != StandardBaseType.ISO45K)
                throw new BusinessException("The Application Form Standard is not valid for creating an ADC.");

            // Validar que el AppForm tenga un Sitio principal activo
            if (!appForm.Sites.Any(s => s.IsMainSite && s.Status == StatusType.Active))
                throw new BusinessException("The Application Form must have an active main Site to create an ADC.");

            // Validaciones por Standard
            if (appForm.Standard.StandardBase == StandardBaseType.ISO22K)
            {
                // Validaciones específicas para ISO 22000
                if (appForm.Category22KID == null || appForm.Category22KID == Guid.Empty)
                    throw new BusinessException("The Application Form must have a Category 22K to create an ADC.");

                if (appForm.Category22K == null || appForm.Category22K.Status != StatusType.Active)
                    throw new BusinessException("The Application Form must have an active Category 22K to create an ADC.");

                if (appForm.HACCPCount == null || appForm.HACCPCount <= 0)
                    throw new BusinessException("The application form must include at least one HACCP process for creating an ADC.");

            }

        } // ValidateCreateItemAsync

        /// <summary>
        /// Establece los valores minimos requeridos para crear un nuevo ADC
        /// </summary>
        /// <param name="item"></param>
        /// <param name="appForm"></param>
        /// <returns></returns>
        private async Task<ADC> SetValuesCreateItemAsync(ADC item, AppForm appForm)
        {
            var riskLevelCategory = AuditCycleCalculations.GetMaxRiskLevelCategory(appForm);

            item.ID = Guid.NewGuid();
            item.AuditCycleID = appForm.AuditCycleID; 
            item.StandardID = appForm.StandardID.Value;
            item.RiskLevelCategory = riskLevelCategory;
            item.CycleYear = appForm.CycleYear;
            item.Status = ADCStatusType.New;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            if (appForm.Standard.StandardBase == StandardBaseType.ISO22K)
            {
                var extraInfoData = new
                {
                    appForm.Category22KID,
                    appForm.HACCPCount,
                    CycleType = appForm.AuditCycle.CycleType ?? AuditCycleType.Nothing,
                };
                item.ExtraInfoJSON = JsonSerializer.Serialize(extraInfoData);
            }
            else
            {
                var extraInfoData = new
                {
                    CycleType = appForm.AuditCycle.CycleType ?? AuditCycleType.Nothing,
                };
                item.ExtraInfoJSON = JsonSerializer.Serialize(extraInfoData);
            }

            return item;
        } // SetValuesCreateItem

        // - Update Item

        /// <summary>
        /// Procesa las validaciones necesarias para actualizar un ADC
        /// </summary>
        /// <param name="item"></param>
        /// <param name="foundItem"></param>
        /// <exception cref="BusinessException"></exception>
        private async Task ValidateUpdateItemAsync(ADC item, ADC foundItem)
        {
            // Validations

            // - Si cambia el status, realizar diferentes validaciones
            if (foundItem.Status != item.Status)
            {
                switch (item.Status) // Si el nuevo status es...
                {
                    case ADCStatusType.Review:
                        //if (string.IsNullOrEmpty(item.ReviewComments))
                        //    throw new BusinessException("Comments are required when send to Review.");
                        break;

                    case ADCStatusType.Rejected:
                        //if (string.IsNullOrEmpty(item.ReviewComments))
                        //    throw new BusinessException("Comments are required when rejected.");
                        break;

                    case ADCStatusType.Active:
                        if (foundItem.Status != ADCStatusType.Review)
                            throw new BusinessException("Only items after Review can be set to Active.");
                        break;

                    case ADCStatusType.Inactive:
                        // No hay validaciones para Inactive aun
                        break;

                    case ADCStatusType.Deleted:
                        throw new BusinessException("To delete an ADC, use the Delete method.");
                }
            }
            else // El status no ha cambiado
            {
                if (foundItem.Status < ADCStatusType.Inactive) // Si sigue en proceso...
                {
                    // Validar que tenga el sitio principal
                    if (!foundItem.ADCSites.Any(adcs =>
                        adcs.Site.IsMainSite
                        && adcs.Status == StatusType.Active
                        && adcs.Site.Status == StatusType.Active))
                        throw new BusinessException("The ADC must have an active main Site.");
                }
            }

            // Validar que si se marca IncludePreAudit, el ciclo de auditoria sea Initial
            // y no tenga un registro de Pre-Audit
            if (item.IncludePreAudit ?? false) {

                if (!await _repository.IsAuditCycleTypeByADCID(foundItem.ID, AuditCycleType.Initial))
                    throw new BusinessException("The Audit Cycle Type must be Initial to include Pre-Audit.");
            }

            if (foundItem.IncludePreAudit.HasValue && foundItem.IncludePreAudit.Value
                && (!item.IncludePreAudit.HasValue || !item.IncludePreAudit.Value))
            {
                // Se quito pre-audit, eliminar los ADCSiteAudits de pre-audit
                var adcSiteAuditRepository = new ADCSiteAuditRepository();

                try
                {
                    await adcSiteAuditRepository.DeleteByADCIDAndAuditStepAsync(foundItem.ID, AuditStepType.PreAudit);
                    await adcSiteAuditRepository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"ADCService.ValidateUpdateItemAsync.DeletePreAudit: {ex.Message}");
                }
            }
        } // ValidateUpdateItem

        /// <summary>
        /// Establece los valores para actualizar un ADC
        /// </summary>
        /// <param name="item">Item con los datos a actualizar</param>
        /// <param name="foundItem">Item encontrado en la BDD con los datos actuales</param>
        /// <returns>Retorna un objeto ADC con los valores actualizados</returns>
        private async Task<ADC> SetValuesUpdateItemAsync(ADC item, ADC foundItem)
        {
            // - Si hay cambios en el status, realizar diferentes asignaciones
            if (foundItem.Status != item.Status)
            {
                switch (item.Status) // Si el nuevo status es...
                {
                    case ADCStatusType.Review:
                        foundItem.ReviewDate = DateTime.UtcNow;
                        // foundItem.ReviewComments = item.ReviewComments;
                        break;

                    case ADCStatusType.Rejected:                        
                        foundItem.ReviewDate = DateTime.UtcNow;
                        //foundItem.ReviewComments = item.ReviewComments;
                        //foundItem.UserReview = item.UpdatedUser;
                        break;

                    case ADCStatusType.Active:
                        // foundItem.UserReview = item.UpdatedUser;
                        foundItem.ActiveDate = DateTime.UtcNow;
                        break;

                    case ADCStatusType.Inactive:
                        throw new BusinessException("To set an ADC to Inactive, deactivate the associated Application Form.");
                        //foundItem.HistoricalDataJSON = GetHistoricalDataJSON(foundItem);
                        //break;

                    case ADCStatusType.Cancel:
                        if (foundItem.Status <= ADCStatusType.Active)
                            foundItem.HistoricalDataJSON = await GetHistoricalDataJSONAsync(foundItem);
                        break;
                }
            } // Si cambia el status

            // Assigning values

            foundItem.IncludePreAudit = item.IncludePreAudit ?? false;
            foundItem.TotalInitial = item.TotalInitial;
            foundItem.TotalMD11 = item.TotalMD11;
            foundItem.TotalSurveillance = item.TotalSurveillance;
            foundItem.TotalRecertification = item.TotalRecertification;
            foundItem.ExtraInfo = item.ExtraInfo;
            foundItem.Status = foundItem.Status == ADCStatusType.Nothing && item.Status == ADCStatusType.Nothing
                ? ADCStatusType.New
                : item.Status != ADCStatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            if (foundItem.AppForm.Standard.StandardBase == StandardBaseType.ISO22K)
            {
                var extraInfoData = new
                {
                    foundItem.AppForm.Category22KID,
                    foundItem.AppForm.HACCPCount,
                    CycleType = foundItem.AppForm.AuditCycle.CycleType ?? AuditCycleType.Nothing,
                };
                foundItem.ExtraInfoJSON = JsonSerializer.Serialize(extraInfoData);
            }
            else {
                var extraInfoData = new
                {
                    CycleType = foundItem.AppForm.AuditCycle.CycleType ?? AuditCycleType.Nothing,
                };
                foundItem.ExtraInfoJSON = JsonSerializer.Serialize(extraInfoData);
            }

            return foundItem;
        } // SetValuesUpdateItem

        /// <summary>
        /// Agrega los sites para un nuevo ADC en base a los sites del AppForm
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        private async Task AddSitesToNewADCAsync(ADC item)
        {
            var appFormRepository = new AppFormRepository();
            var adcSiteRepository = new ADCSiteRepository();
            var md5Repository = new MD5Repository();

            var appForm = await appFormRepository.GetAsync(item.AppFormID)
                ?? throw new BusinessException("The AppForm was not found.");

            if (appForm.Sites == null || !appForm.Sites.Any())
                throw new BusinessException("The AppForm does not have any Sites.");

            item.AppForm = appForm; // item no trae el AppForm porque es nuevo y se necesita en CreateInitialDataAsync

            // - Obtener los Sites del AppForm y agregarlos al ADC
            foreach (var site in appForm.Sites.Where(s => s.Status == StatusType.Active))
            {   
                var adcSite = await ADCSiteService.CreateInitialDataAsync(item, site);
                adcSite.UpdatedUser = item.UpdatedUser;

                adcSiteRepository.Add(adcSite);

                if (site.IsMainSite)
                {
                    // Agregar los ADCConceptValues si no existen, solo al sitio principal
                    await RegisterADCConceptsAsync(adcSite, appForm.StandardID ?? Guid.Empty);
                }

                // Agregar los ADCSiteAudits si no existen
                await _adcSiteAuditService.AddADCSiteAuditsAsync(adcSite, appForm);
            } // foreach site

            await adcSiteRepository.SaveChangesAsync();
        } // AddSitesToNewADCAsync

        /// <summary>
        /// Agrega los sites para un nuevo ADC en base a los sites del AppForm, 
        /// pero con reglas especiales para ISO 22000
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        private async Task ADCSitesToNewISO22KADCAsync(ADC item)
        {
            var appFormRepository = new AppFormRepository();
            var adcSiteRepository = new ADCSiteRepository();
            var md5Repository = new MD5Repository();
            
            var appForm = await appFormRepository.GetAsync(item.AppFormID)
                ?? throw new BusinessException("The AppForm was not found.");

            if (appForm.Sites == null || !appForm.Sites.Any())
                throw new BusinessException("The AppForm does not have any Sites.");

            var tableType = AuditCycleCalculations
                .GetMD5TableType(appForm.Standard?.StandardBase ?? StandardBaseType.Nothing);
            //var maxRiskLevelCategory = AuditCycleCalculations // Pos es solo para 22K - 
            //    .GetMaxRiskLevelCategory(appForm);

            // 1. Obtener el total de empleados de todos los Sites
            // 2. Calcular los valores de TD y TH, es en general para todos los sites
            // 3. Entregar el calculo de empleados TFTE para registrarlo en el sitio main
            // 4. Para el resto de sitios, el numero de dias es el 50% de TD + TH + TFTE
            var totalEmployeesAllSites = OrganizationCalculations
                .GetTotalWorkers(appForm.Sites.ToList());
            var md5ItemAllSites = await md5Repository
                .GetItemByEmployeesAsync(totalEmployeesAllSites, tableType);

            item.AppForm = appForm; // item no trae el AppForm porque es nuevo y se necesita en CreateInitialDataAsync

            foreach (var site in appForm.Sites.Where(s => s.Status == StatusType.Active))
            {   
                var adcSite = await ADCSiteService.CreateISO22KInitialDataAsync(item, site, md5ItemAllSites);
                adcSite.UpdatedUser = item.UpdatedUser;
                adcSiteRepository.Add(adcSite);

                if (site.IsMainSite)
                {
                    await RegisterADCConceptsAsync(adcSite, appForm.StandardID ?? Guid.Empty);
                }

                // Agregar los ADCSiteAudits si no existen
                await _adcSiteAuditService.AddADCSiteAuditsAsync(adcSite, appForm);
            } // foreach site

            await adcSiteRepository.SaveChangesAsync();
        } // ADCSitesToNewISO22KADCAsync

        /// <summary>
        /// Revisa un ADC existente y actualiza sus sites en base a los sites del AppForm
        /// agregando los que no existen y eliminando los que ya no están en el AppForm
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        private async Task UpdateSitesToExistingADCAsync(ADC item)
        {
            var appFormRepository = new AppFormRepository();
            var adcSiteRepository = new ADCSiteRepository();
            var md5Repository = new MD5Repository();
            var appForm = await appFormRepository.GetAsync(item.AppFormID)
                ?? throw new BusinessException($"The AppForm was not found: {item.AppFormID}.");

            if (appForm.Sites == null || !appForm.Sites.Any())
                throw new BusinessException("The AppForm does not have any Sites.");

            // Obtener el nivel de riesgo máximo del AppForm
            var maxRiskLevel = AuditCycleCalculations
                .GetMaxRiskLevelCategory(appForm);
            // Obtener de acuerdo con el standard, el tipo de tabla MD5 a consultar
            var tableType = AuditCycleCalculations
                .GetMD5TableType(appForm.Standard?.StandardBase ?? StandardBaseType.Nothing);
            // 1. Obtener los Sites del AppForm y agregar solo los que no existen al ADC
            foreach (var site in appForm.Sites
                .Where(s => s.Status == StatusType.Active))
            {
                if (adcSiteRepository.Gets()
                    .Any(s => s.SiteID == site.ID && s.ADCID == item.ID)
                ) continue; // El Site ya existe en el ADC, saltar al siguiente

                var adcSite = await ADCSiteService.CreateInitialDataAsync(item, site);
                adcSite.UpdatedUser = item.UpdatedUser;

                adcSiteRepository.Add(adcSite);

                // Agregar los ADCConceptValues si no existen
                if (site.IsMainSite)
                {
                    await RegisterADCConceptsAsync(adcSite, appForm.StandardID ?? Guid.Empty);
                }
                // Agrega los ADCSiteAudits si no existen - NOTE: Puede que lo tenga que quitar y solo utilizar el de SyncADCSiteAuditsAsync
                //await _adcSiteAuditService.AddADCSiteAuditsAsync(adcSite, appForm);

            } // foreach site

            await adcSiteRepository.SaveChangesAsync();

            // 2. Eliminar los Sites que no están en el AppForm
            var sitesFromBDD = adcSiteRepository.Gets()
                .Where(s => s.ADCID == item.ID)
                .ToList();

            if (sitesFromBDD != null && sitesFromBDD.Any())
            {   
                var sitesToRemove = sitesFromBDD
                    .Where(s => !appForm.Sites.Any(a => a.ID == s.SiteID))
                    .Select(s => s.ID)
                    .ToList();

                await adcSiteRepository.DeleteByListToRemoveAsync(sitesToRemove);
            }

            // 3. Actualizar los InitialMD5 de los sites que siguen en el ADC,
            //    en caso de que haya cambiado la categoria de Risk Level, asi como
            //    los ADCSiteAudits de cada site por si cambió el CycleType del AppForm
            List<ADCSite> adcSitesToUpdate = adcSiteRepository.Gets()
                .Where(s => s.ADCID == item.ID
                    && appForm.Sites.Any(a => a.ID == s.SiteID))
                .ToList();

            foreach (var adcSite in adcSitesToUpdate)
            {   
                var _adcSite = await ADCSiteService.RefreshInitialDataAsync(adcSite);

                adcSite.MD5ID = _adcSite.MD5ID;
                adcSite.InitialMD5 = _adcSite.InitialMD5;
                adcSite.TotalWorkers = _adcSite.TotalWorkers;
                adcSite.WorkersOnSite = _adcSite.WorkersOnSite;
                adcSite.WorkersOffSite = _adcSite.WorkersOffSite;
                adcSite.TotalInitial = _adcSite.TotalInitial;
                adcSite.UpdatedUser = item.UpdatedUser;

                // Actualizar los ADCSiteAudits de cada site por si cambió el CycleType
                await _adcSiteAuditService.SyncADCSiteAuditsAsync(adcSite, appForm);

                adcSiteRepository.Update(adcSite); // Ver sino se necesita UpdateValues, pues ya se tiene el objeto completo
            }

            await adcSiteRepository.SaveChangesAsync();

        } // UpdateSitesToExistingADCAsync

        /// <summary>
        /// Agrega los ADCConceptValues a un ADCSite en base a los ADCConcepts del Standard
        /// </summary>
        /// <param name="adcSite"></param>
        /// <param name="standardID"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        private async Task<List<ADCConceptValue>> RegisterADCConceptsAsync(ADCSite adcSite, Guid standardID)
        { 
            var adcConceptRepository = new ADCConceptRepository();
            var adcConceptValueRepository = new ADCConceptValueRepository();

            var concepts = adcConceptRepository
                .Gets()
                .Where(c => c.StandardID == standardID
                    && c.Status == StatusType.Active)
                .ToList() ?? throw new BusinessException("No ADC Concepts found for the Standard.");
            var hasChanges = false;
            var listADCConceptValues = new List<ADCConceptValue>();

            foreach (var concept in concepts)
            { 
                if (adcSite.ADCConceptValues == null
                    || !adcSite.ADCConceptValues.Any(acv => acv.ADCConceptID == concept.ID))
                {
                    var adcConceptValue = new ADCConceptValue
                    {
                        ID = Guid.NewGuid(),
                        ADCConceptID = concept.ID,
                        ADCSiteID = adcSite.ID,
                        CheckValue = false,
                        Value = 0, // Inicializar en 0 o el valor que corresponda
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        Status = StatusType.Active,
                        UpdatedUser = adcSite.UpdatedUser
                    };
                    
                    adcConceptValueRepository.Add(adcConceptValue);
                    listADCConceptValues.Add(adcConceptValue);
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                try
                {
                    await adcConceptValueRepository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"ADCService.RegisterADCConceptsAsync: {ex.Message}");
                }
            }

            return listADCConceptValues;
        } // RegisterADCConceptsAsync

        //private async Task<List<ADCSiteAudit>> AddADCSiteAuditsAsync(ADCSite adcSite, AppForm appForm, bool isMultisite)
        //{
        //    var cycleType = appForm.AuditCycle.CycleType ?? AuditCycleType.Nothing;
        //    var initialStep = appForm.AuditCycle.InitialStep ?? AuditStepType.Nothing;
        //    var periodicity = appForm.AuditCycle.Periodicity ?? AuditCyclePeriodicityType.Nothing;

        //    if (cycleType == AuditCycleType.Nothing
        //        || (cycleType == AuditCycleType.Transfer && initialStep == AuditStepType.Nothing))
        //        throw new BusinessException("The Audit Cycle Type or Initial Step are not valid, can't be generate the ADCSiteAudits.");

        //    if (periodicity == AuditCyclePeriodicityType.Nothing)
        //        throw new BusinessException("The Audit Cycle Periodicity is not valid, can't be generate the ADCSiteAudits.");

        //    var adcSiteAuditRepository = new ADCSiteAuditRepository();
        //    var listADCSiteAudits = new List<ADCSiteAudit>();
        //    var stepList = AuditCycleCalculations.GetStepList(cycleType, initialStep, periodicity);
        //    var hasChanges = false;

        //    foreach (AuditStepType step in stepList)
        //    {
        //        var currentSite = appForm.Sites
        //            .Where(s => s.ID == adcSite.SiteID)
        //            .FirstOrDefault() ?? new Site();
        //        bool isOneOrMainSite = !isMultisite || currentSite.IsMainSite;

        //        var adcStepAudit = new ADCSiteAudit()
        //        {
        //            ID = Guid.NewGuid(),
        //            ADCSiteID = adcSite.ID,
        //            Value = isOneOrMainSite, // si es un solo sitio o es el principal, por default en true (el sitio recibe todas las auditorias)
        //            AuditStep = step,
        //            Days = isOneOrMainSite && step == AuditStepType.Stage1
        //                ? (decimal?)1
        //                : null,
        //            Status = StatusType.Active,
        //            Created = DateTime.UtcNow,
        //            Updated = DateTime.UtcNow,
        //            UpdatedUser = "system",
        //        };
        //        adcSiteAuditRepository.Add(adcStepAudit);
        //        listADCSiteAudits.Add(adcStepAudit);
        //        hasChanges = true;
        //    }

        //    if (hasChanges)
        //    {
        //        try
        //        {
        //            await adcSiteAuditRepository.SaveChangesAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new BusinessException($"ADCService.AddADCSiteAuditsAsync: {ex.Message}");
        //        }
        //    }

        //    return listADCSiteAudits;
        //} // AddADCSiteAuditsAsync

        /// <summary>
        /// Actualiza los valores para un ADCSite tanto su numero de empleados como
        /// el numero de dias en base a Employees Range (antes MD5), así como el 
        /// Total de Empleados del ADC
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task<ADC> RefreshInitialDataAsync(ADC item) 
        {
            var maxRiskLevelCategory = AuditCycleCalculations
                .GetMaxRiskLevelCategory(item.AppForm);

            if (item.ADCSites != null && item.ADCSites.Any())
            {
                var totalWorkers = 0;

                foreach (var adcSite in item.ADCSites
                    .Where(adcsite => adcsite.Status == StatusType.Active))
                {
                    var _totalWorkers = OrganizationCalculations.GetTotalWorkers(adcSite.Site);
         
                    if (_totalWorkers != adcSite.TotalWorkers) // Si el total de empleados del site ha cambiado
                    { 
                        var adcSiteService = new ADCSiteService();
                        await adcSiteService.UpdateInitialDataAsync(adcSite.ID); // Aquí se actualiza el total de empleados y el total de dias iniciales (InitialMD5)
                    }

                    totalWorkers += _totalWorkers;
                }

                item.TotalWorkers = totalWorkers;
            }
            else 
            {
                item.TotalWorkers = 0;
            }

            item.RiskLevelCategory = maxRiskLevelCategory;

            return item;
        } // RefreshInitialDataAsync

        private async Task<ADC> RefreshInitialDataISO22KAsync(ADC item)
        {
            var md5Repository = new MD5Repository();

            var appForm = item.AppForm 
                ?? throw new BusinessException("The AppForm is required to recalculate totals for ISO 22000.");
            var totalEmployeesAllSites = OrganizationCalculations
                .GetTotalWorkers(appForm.Sites.ToList());

            // Haya cambiado algo o no, recalcular los valores de los ADCSites
            if (item.ADCSites != null && item.ADCSites.Any())
            {
                foreach (var adcSite in item.ADCSites
                    .Where(adcsite => adcsite.Status == StatusType.Active))
                {
                    var adcSiteService = new ADCSiteService();
                    await adcSiteService.UpdateISO22KInitialDataAsync(adcSite.ID);
                }
            }
            
            item.TotalWorkers = totalEmployeesAllSites;

            return item;
        } // RefreshInitialDataISO22KAsync

        private async Task<string> GetHistoricalDataJSONAsync(ADC item)
        {
            var _organizationRepository = new OrganizationRepository();
            var firstSite = item.ADCSites?
                .FirstOrDefault(s => s.Status == StatusType.Active);
            bool isMultiStandard = await _organizationRepository.IsMultiStandardAsync(item.AuditCycle.OrganizationID);

            var historicalData = new
            {
                IsMultiStandard = isMultiStandard,
                Organization = new
                {
                    Name = item.AppForm?.Organization?.Name ?? string.Empty,
                    Status = item.AppForm?.Organization?.Status ?? OrganizationStatusType.Nothing,
                },
                AuditCycle = new 
                {
                    item.AuditCycle?.CycleType,
                    item.AuditCycle?.InitialStep,
                    item.AuditCycle?.Periodicity
                }, 
                Standard = new {
                    item.Standard.Name,
                    item.Standard?.StandardBase,
                    item.Standard?.Description
                },
                Sites = item.ADCSites?
                    .Where(s => s.Status == StatusType.Active)
                    .Select(s => new {
                        s.SiteID,
                        s.Site.Description,
                        s.Site.IsMainSite,
                        s.Site.Address,
                        s.Site.Country,
                        s.Site.LocationURL
                    }),
                ADCConcepts = firstSite.ADCConceptValues
                    .Select(acv => new {
                        acv.ADCConceptID,
                        acv.ADCConcept.StandardID,
                        acv.ADCConcept.IndexSort,
                        acv.ADCConcept.Description,
                        acv.ADCConcept.WhenTrue,
                        acv.ADCConcept.Increase,
                        acv.ADCConcept.Decrease,
                        acv.ADCConcept.IncreaseUnit,
                        acv.ADCConcept.DecreaseUnit,
                        acv.ADCConcept.ExtraInfo
                    })
            };

            return JsonSerializer.Serialize(historicalData);
        } // GetHistoricalDataJSON

        #endregion // PRIVATE

        #region STATIC METHODS

        /// <summary>
        /// Revisa de un ADC si tiene alertas y cuáles son
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<List<ADCAlertType>> GetAlertsAsync(ADC item)
        { 
            var alerts = new List<ADCAlertType>();

            if (item.Status < ADCStatusType.Inactive)
            { 
                // Obtener alertas de ADCSites
                if (item.ADCSites != null && item.ADCSites.Any())
                {
                    var totalWorkers = item.ADCSites
                        .Where(adcsite => adcsite.Status == StatusType.Active)
                        .Sum(adcsite => adcsite.TotalWorkers) ?? 0;

                    foreach (var adcSite in item.ADCSites
                        .Where(adcsite => adcsite.Status == StatusType.Active))
                    {
                        adcSite.Alerts = ADCSiteService.GetAlerts(adcSite);

                        if (adcSite.Alerts != null && adcSite.Alerts.Any())
                        {
                            if (adcSite.Alerts.Any(a => a == ADCSiteAlertType.EmployeesMistmatch)
                                && !alerts.Contains(ADCAlertType.EmployeesMistmatch))
                                alerts.Add(ADCAlertType.EmployeesMistmatch);
                        }
                    }

                    // Si el total de empleados del ADC no coincide con la suma de empleados de los ADCSites
                    if (item.TotalWorkers != totalWorkers
                        && !alerts.Contains(ADCAlertType.EmployeesMistmatch))
                    {
                        alerts.Add(ADCAlertType.EmployeesMistmatch);
                    }
                } // Validando si cambia el numero de empleados

                // 1. Validar si cambió el tipo de CertificateCycle y lanzar
                //    una advertencia asi como actualizar los ADCConceptValues 
                if (!string.IsNullOrEmpty(item.ExtraInfoJSON))
                {
                    using (var doc = JsonDocument.Parse(item.ExtraInfoJSON))
                    {
                        var root = doc.RootElement;

                        var cycleType = root.GetNullableInt("CycleType");
                        if (cycleType.HasValue)
                        {
                            var cycleTypeEnum = (AuditCycleType)cycleType.Value;
                            if (cycleTypeEnum != item.AuditCycle.CycleType
                                && !alerts.Contains(ADCAlertType.CycleTypeMistmatch))
                            {
                                alerts.Add(ADCAlertType.CycleTypeMistmatch);
                            }
                        }
                    }
                }

                // Si falta asignar el sitio principal (que hayan
                // actualizado los sitios y no esté el principal) o algo así
                if (!item.ADCSites.Any(adcs => adcs.Site.IsMainSite
                    && adcs.Status == StatusType.Active
                    && adcs.Site.Status == StatusType.Active))
                {
                    if (!alerts.Contains(ADCAlertType.MainSiteMissing))
                        alerts.Add(ADCAlertType.MainSiteMissing);
                }

                // Si el número de ADCSites no coincide con el número de
                // sites o los sites del AppForm: SitesMistmatch
                if (item.AppForm != null
                    && item.AppForm.Sites != null
                    && item.ADCSites != null)
                {
                    if (!SitesMistmatch(item) && !alerts.Contains(ADCAlertType.SitesMistmatch))
                        alerts.Add(ADCAlertType.SitesMistmatch);
                }
                else
                {
                    // TODO: Realmente no se cargó la información.
                    // Ver otra forma de informarlo sin interrumpir el flujo.
                    if (!alerts.Contains(ADCAlertType.SitesMistmatch))
                        alerts.Add(ADCAlertType.SitesMistmatch);
                }

                // Validar si el nivel de riesgo del ADC no coincide con el nivel de
                // riesgo del AppForm. 
                var maxRiskLevelCategory = AuditCycleCalculations.GetMaxRiskLevelCategory(item.AppForm);
                if (maxRiskLevelCategory != item.RiskLevelCategory)
                {
                    if (!alerts.Contains(ADCAlertType.RiskLevelMistmatch))
                        alerts.Add(ADCAlertType.RiskLevelMistmatch);
                }

                // Si es Standard es ISO 22K y cambió en el AppForm la categoría de
                // ISO 22K o el número de planes HACCP, cualquiera de estas dos
                // situaciones puede cambiar el número de días de auditoría, por
                // lo que se debe alertar al usuario
                if (item.Standard?.StandardBase == StandardBaseType.ISO22K)
                {
                    Guid? categoryID = null;
                    int? haccpCount = null;

                    if (!string.IsNullOrEmpty(item.ExtraInfoJSON))
                    {
                        using (var doc = JsonDocument.Parse(item.ExtraInfoJSON))
                        {
                            var root = doc.RootElement;
                            categoryID = root.GetNullableGuid("Category22KID");
                            haccpCount = root.GetNullableInt("HACCPCount");
                        }
                    }

                    // Cambió la categoría de ISO 22K
                    if (categoryID.HasValue && item.AppForm.Category22KID != categoryID.Value)
                    {
                        if (!alerts.Contains(ADCAlertType.Category22KMistmatch))
                            alerts.Add(ADCAlertType.Category22KMistmatch);
                    }

                    // Cambió el número de planes HACCP
                    if (haccpCount.HasValue && item.AppForm.HACCPCount != haccpCount.Value)
                    {
                        if (!alerts.Contains(ADCAlertType.HACCPCountMistmatch))
                            alerts.Add(ADCAlertType.HACCPCountMistmatch);
                    }
                } // Si es ISO 22K
            } // if status < Inactive

            return alerts;
        } // GetAlertsAsync

        /// <summary>
        /// Compara el numero de sitios y si son los mismos sitios, devuelve true 
        /// si son los mismos, de lo contrario false.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool SitesMistmatch(ADC item)
        {
            var noADCSites = item.ADCSites?.Count(adcsite => adcsite.Status == StatusType.Active) ?? 0;
            var noAppFormSites = item.AppForm?.Sites?.Count(site => site.Status == StatusType.Active) ?? 0;

            if (noADCSites != noAppFormSites)
                return false;

            // verificar que sean los mismos Sites
            
            foreach (var site in item.AppForm.Sites
                .Where(site => site.Status == StatusType.Active))
            {
                if (!item.ADCSites.Any(adcsite => adcsite.SiteID == site.ID))
                {
                    return false;
                }
            }

            return true;
        } // SitesMistmatch

        #endregion // STATIC METHODS
    }
}