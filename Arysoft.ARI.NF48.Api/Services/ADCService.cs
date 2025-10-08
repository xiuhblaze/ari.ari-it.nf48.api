using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
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

        // CONSTRUCTOR

        public ADCService()
        {
            _repository = new ADCRepository();
        }

        // METHODS

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
                item.Alerts = GetAlertsAsync(item).GetAwaiter().GetResult();
            }

            // Order

            switch (filters.Order)
            {
                case ADCOrderType.Description:
                    items = items.OrderBy(e => e.Description);
                    break;
                case ADCOrderType.Created:
                    items = items.OrderBy(e => e.Created);
                    break;
                case ADCOrderType.DescriptionDesc:
                    items = items.OrderBy(e => e.Description);
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

        public async Task<ADC> GetAsync(Guid id)
        {
            var item = await _repository.GetAsync(id)
                ?? throw new BusinessException("The ADC was not found.");

            if (item.Status < ADCStatusType.Inactive)
            { 
                var alerts = await GetAlertsAsync(item);

                if (alerts.Count > 0)
                {
                    if (alerts.Contains(ADCAlertType.SitesMistmatch))
                    { 
                        await UpdateSitesToExistingADCAsync(item);
                        _repository.DetachAllEntities();
                        item = await _repository.GetAsync(item.ID)
                            ?? throw new BusinessException("The ADC was not found after update sites.");
                    }

                    await RecalcularTotalesAsync(item);

                    try
                    {
                        _repository.UpdateValues(item);
                        await _repository.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new BusinessException($"ADCService.GetAsync.Update.RecalcularTotales: {ex.Message}");
                    }

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

            // Validations

            if (item.AppFormID == null || item.AppFormID == Guid.Empty)            
                throw new BusinessException("The Application Form ID is required.");

            // Validar que el AppForm no tenga un ADC

            if (await _repository.AppFormHasValidADCAsync(item.AppFormID))
                throw new BusinessException("The Application Form already has a valid ADC");
            
            // Set default values
            
            item.ID = Guid.NewGuid();
            item.AuditCycleID = await _appFormRepository.GetAuditCycleIDAsync(item.AppFormID);
            item.UserCreates = item.UpdatedUser;
            item.Status = ADCStatusType.New;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            try
            { 
            // Procesar ADC
                await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
                _repository.Add(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCService.AddAsync: {ex.Message}");
            }

            await AddSitesToNewADCAsync(item);

            //await ProcesarADCAsync(item);

            try 
            { 
                _repository.UpdateValues(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)

            {
                throw new BusinessException($"ADCService.AddAsync.Update.ProcesarADCAsync: {ex.Message}");
            }

            item = await _repository.GetAsync(item.ID, asNoTracking: true)
                ?? throw new BusinessException("The ADC was not found after creation.");

            await RecalcularTotalesAsync(item);

            try
            {
                _repository.UpdateValues(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)

            {
                throw new BusinessException($"ADCService.AddAsync.Update.RecalcularTotales: {ex.Message}");
            }

            item = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The ADC was not found after and recalculation.");

            return item;
        } // AddAsync

        public async Task<ADC> UpdateAsync(ADC item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            ValidateUpdateItem(item, foundItem);
            SetValuesUpdateItem(item, foundItem);

            foundItem.Alerts = await GetAlertsAsync(foundItem);

            //if (foundItem.Alerts.Contains(ADCAlertType.SitesMistmatch))
            //{
            //    await UpdateSitesToExistingADCAsync(foundItem);
            //}

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task<ADC> UpdateCompleteADCAsync(ADC item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            var _adcSitesService = new ADCSiteService();

            ValidateUpdateItem(item, foundItem);
            SetValuesUpdateItem(item, foundItem);

            foundItem.Alerts = await GetAlertsAsync(foundItem);

            var listSites = new List<ADCSite>();

            if (item.ADCSites?.Any() ?? false) // en item.ADCSites traigo los nuevos valores
            {
                listSites = await _adcSitesService
                    .UpdateListAsync(item.ADCSites.ToList());
            }

            // Creo que no lo necesita pues en Get ya se actualizan los sites
            // y debe de traer ya los nuevos datos
            //if (item.Status < ADCStatusType.Inactive)
            //{
            //    //await ProcesarADCAsync(foundItem);
            //    if (foundItem.Alerts.Contains(ADCAlertType.SitesMistmatch))
            //        await UpdateSitesToExistingADCAsync(foundItem);
            //}

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCService.UpdateCompleteADCAsync: {ex.Message}");
            }

            foundItem.ADCSites = listSites;            
            
            return foundItem;
        } // UpdateListAsync

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
                    foundItem.HistoricalDataJSON = GetHistoricalDataJSON(foundItem);

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

        // PRIVATE

        /// <summary>
        /// Procesa las validaciones necesarias para actualizar un ADC
        /// </summary>
        /// <param name="item"></param>
        /// <param name="foundItem"></param>
        /// <exception cref="BusinessException"></exception>
        private void ValidateUpdateItem(ADC item, ADC foundItem)
        {
            // Validations

            // - Si cambia el status, realizar diferentes validaciones
            if (foundItem.Status != item.Status)
            { 
                switch (item.Status) // Si el nuevo status es...
                {
                    case ADCStatusType.Review:
                        if (string.IsNullOrEmpty(item.ReviewComments))
                            throw new BusinessException("Comments are required when send to Review.");
                        break;

                    case ADCStatusType.Rejected:
                        if (string.IsNullOrEmpty(item.ReviewComments))
                            throw new BusinessException("Comments are required when rejected.");
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

        } // ValidateUpdateItem

        /// <summary>
        /// Establece los valores para actualizar un ADC
        /// </summary>
        /// <param name="item"></param>
        /// <param name="foundItem"></param>
        private void SetValuesUpdateItem(ADC item, ADC foundItem)
        {
            // - Si hay cambios en el status, realizar diferentes asignaciones
            if (foundItem.Status != item.Status)
            {
                switch (item.Status) // Si el nuevo status es...
                {
                    case ADCStatusType.Review:
                        foundItem.ReviewDate = DateTime.UtcNow;
                        foundItem.ReviewComments = item.ReviewComments;
                        break;

                    case ADCStatusType.Rejected:                        
                        foundItem.ReviewDate = DateTime.UtcNow;
                        foundItem.ReviewComments = item.ReviewComments;
                        foundItem.UserReview = item.UpdatedUser;
                        break;

                    case ADCStatusType.Active:
                        foundItem.UserReview = item.UpdatedUser;
                        foundItem.ActiveDate = DateTime.UtcNow;
                        break;

                    case ADCStatusType.Inactive:
                        foundItem.HistoricalDataJSON = GetHistoricalDataJSON(foundItem);
                        break;

                    case ADCStatusType.Cancel:
                        if (foundItem.Status <= ADCStatusType.Active)
                            foundItem.HistoricalDataJSON = GetHistoricalDataJSON(foundItem);
                        break;
                }
            } // Si cambia el status

            // Assigning values

            foundItem.Description = item.Description;
            foundItem.TotalInitial = item.TotalInitial;
            foundItem.TotalMD11 = item.TotalMD11;
            foundItem.TotalSurveillance = item.TotalSurveillance;
            foundItem.TotalRR = item.TotalRR;
            foundItem.ExtraInfo = item.ExtraInfo;
            foundItem.Status = foundItem.Status == ADCStatusType.Nothing && item.Status == ADCStatusType.Nothing
                ? ADCStatusType.New
                : item.Status != ADCStatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;
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

            // - Obtener los Sites del AppForm y agregarlos al ADC

            foreach(var site in appForm.Sites.Where(s => s.Status == StatusType.Active))
            {
                var employeesMD5 = await ADCSiteService.GetEmployeesMD5Async(site.ID);
                var adcSite = new ADCSite
                {
                    ID = Guid.NewGuid(),
                    ADCID = item.ID,
                    SiteID = site.ID,
                    InitialMD5 = employeesMD5.InitialMD5,
                    NoEmployees = employeesMD5.NoEmployees,
                    TotalInitial = employeesMD5.InitialMD5,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    UpdatedUser = item.UpdatedUser,
                    Status = StatusType.Active
                };
                adcSiteRepository.Add(adcSite);

                // Agregar los ADCConceptValues si no existen
                await RegisterADCConceptsAsync(adcSite, appForm.StandardID ?? Guid.Empty);

                // Agregar los ADCSiteAudits si no existen
                await AddADCSiteAuditsAsync(adcSite, appForm, appForm.Sites.Count > 1);
            } // foreach site

            await adcSiteRepository.SaveChangesAsync();
        } // AddSitesToNewADCAsync

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

            // - Obtener los Sites del AppForm y agregar solo los que no existen al ADC
            foreach (var site in appForm.Sites
                .Where(s => s.Status == StatusType.Active))
            {
                if (adcSiteRepository.Gets().Any(s => s.SiteID == site.ID && s.ADCID == item.ID))
                    continue; // El Site ya existe en el ADC, saltar al siguiente

                var adcSite = new ADCSite();
                var employeesMD5 = await ADCSiteService.GetEmployeesMD5Async(site.ID);

                adcSite.ID = Guid.NewGuid();
                adcSite.ADCID = item.ID;
                adcSite.SiteID = site.ID;
                adcSite.InitialMD5 = employeesMD5.InitialMD5;
                adcSite.NoEmployees = employeesMD5.NoEmployees;
                adcSite.Status = StatusType.Active;
                adcSite.Created = DateTime.UtcNow;
                adcSite.Updated = DateTime.UtcNow;
                adcSite.UpdatedUser = item.UpdatedUser;
        
                adcSiteRepository.Add(adcSite);

                // Agregar los ADCConceptValues si no existen
                await RegisterADCConceptsAsync(adcSite, appForm.StandardID ?? Guid.Empty);
                // Agrega los ADCSiteAudits si no existen
                await AddADCSiteAuditsAsync(adcSite, appForm, appForm.Sites.Count > 1);

            } // foreach site

            await adcSiteRepository.SaveChangesAsync();

            // Segundo foreach para eliminar los Sites que no están en el AppForm
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

        } // UpdateSitesToExistingADCAsync

        private async Task ProcesarADCAsync(ADC item)
        {
            var appFormRepository = new AppFormRepository();
            var adcSiteRepository = new ADCSiteRepository();
            var md5Repository = new MD5Repository();

            var appForm = await appFormRepository.GetAsync(item.AppFormID)
                ?? throw new BusinessException("The AppForm was not found.");

            if (appForm.Sites == null || !appForm.Sites.Any())
                throw new BusinessException("The AppForm does not have any Sites.");

            //var adcSiteList = new List<ADCSite>();

            // - Obtener los Sites del AppForm y agregarlos al ADC
            //HACK: Que pasa si se quita algun site del AppForm?
            foreach (var site in appForm.Sites
                .Where(s => s.Status == StatusType.Active))
            {
                //// - Obtener los Empleados de cada turno y sumarlos
                //var noEmployees = site.Shifts
                //    .Where(s => s.Status == StatusType.Active)
                //    .Sum(s => s.NoEmployees) ?? 0;
                
                //// - Obtener el MD5
                //var initialMd5 = await md5Repository.GetDaysAsync(noEmployees);
                ////var adcSite = item.ADCSites != null
                ////    ? item.ADCSites.FirstOrDefault(s => s.SiteID == site.ID) ?? new ADCSite()
                ////    : new ADCSite();

                var employeesMD5 = await ADCSiteService.GetEmployeesMD5Async(site.ID);

                var adcSite = adcSiteRepository.Gets()
                    .FirstOrDefault(s => s.SiteID == site.ID && s.ADCID == item.ID)
                    ?? new ADCSite();

                adcSite.InitialMD5 = employeesMD5.InitialMD5;
                adcSite.NoEmployees = employeesMD5.NoEmployees;
                adcSite.Updated = DateTime.UtcNow;
                adcSite.UpdatedUser = item.UpdatedUser;

                if (adcSite.ID == Guid.Empty)
                {
                    adcSite.ID = Guid.NewGuid();
                    adcSite.ADCID = item.ID;
                    adcSite.SiteID = site.ID;
                    adcSite.Created = DateTime.UtcNow;
                    adcSite.Status = StatusType.Active;

                    adcSiteRepository.Add(adcSite);
                    //adcSiteList.Add(adcSite);
                }
                else 
                { 
                    adcSiteRepository.Update(adcSite);
                    //adcSiteList.Add(adcSite);
                }

                // Agregar los ADCConceptValues si no existen
                await RegisterADCConceptsAsync(adcSite, appForm.StandardID ?? Guid.Empty);
            } // foreach site

            // HACK: Hacer un segundo foreach para eliminar los Sites que no están en el AppForm
            var adcSitesFromBDD = adcSiteRepository.Gets()
                .Where(s => s.ADCID == item.ID);

            if (adcSitesFromBDD != null) // Esto aun no funciona, pues no se han subido a la bdd los nuevos sites en este momento
            {
                // - Eliminar los Sites que no están en el AppForm
                var sitesToRemove = adcSitesFromBDD
                    .Where(s => !appForm.Sites.Any(a => a.ID == s.SiteID))
                    .ToList();
                foreach (var siteToRemove in sitesToRemove)
                {
                    adcSiteRepository.Delete(siteToRemove);
                }
            }

            await adcSiteRepository.SaveChangesAsync();
        } // ProcesarADC

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

        private async Task<List<ADCSiteAudit>> AddADCSiteAuditsAsync(ADCSite adcSite, AppForm appForm, bool isMultisite)
        {
            var currentAuditCycleStandard = appForm.AuditCycle.AuditCycleStandards
                .FirstOrDefault(s => s.StandardID == appForm.StandardID)
                ?? throw new BusinessException("The current standard was not found in audit cycle");

            var cycleType = currentAuditCycleStandard.CycleType ?? AuditCycleType.Nothing;
            var initialStep = currentAuditCycleStandard.InitialStep ?? AuditStepType.Nothing;
            var periodicity = appForm.AuditCycle.Periodicity ?? AuditCyclePeriodicityType.Nothing;

            if (cycleType == AuditCycleType.Nothing 
                || (cycleType == AuditCycleType.Transfer && initialStep == AuditStepType.Nothing))
                throw new BusinessException("The Audit Cycle Type or Initial Step are not valid, can't be generate the ADCSiteAudits.");

            if (periodicity == AuditCyclePeriodicityType.Nothing)
                throw new BusinessException("The Audit Cycle Periodicity is not valid, can't be generate the ADCSiteAudits.");

            var adcSiteAuditRepository = new ADCSiteAuditRepository();
            var listADCSiteAudits = new List<ADCSiteAudit>();
            var stepList = new List<AuditStepType>();
            var hasChanges = false;

            switch (cycleType)
            {
                case AuditCycleType.Initial:
                    // stepList.Add(AuditStepType.Stage1);
                    stepList.Add(AuditStepType.Stage2);
                    stepList.Add(AuditStepType.Surveillance1);
                    stepList.Add(AuditStepType.Surveillance2);
                    if (periodicity == AuditCyclePeriodicityType.Biannual)
                    {
                        stepList.Add(AuditStepType.Surveillance3);
                        stepList.Add(AuditStepType.Surveillance4);
                        stepList.Add(AuditStepType.Surveillance5);
                    }
                    break;
                case AuditCycleType.Recertificacion:
                    stepList.Add(AuditStepType.Recertification);
                    stepList.Add(AuditStepType.Surveillance1);
                    stepList.Add(AuditStepType.Surveillance2);
                    if (periodicity == AuditCyclePeriodicityType.Biannual)
                    {
                        stepList.Add(AuditStepType.Surveillance3);
                        stepList.Add(AuditStepType.Surveillance4);
                        stepList.Add(AuditStepType.Surveillance5);
                    }
                    break;
                case AuditCycleType.Transfer:
                    switch (initialStep)
                    {
                        case AuditStepType.Recertification:
                            stepList.Add(AuditStepType.Recertification);
                            stepList.Add(AuditStepType.Surveillance1);
                            stepList.Add(AuditStepType.Surveillance2);
                            if (periodicity == AuditCyclePeriodicityType.Biannual)
                            {
                                stepList.Add(AuditStepType.Surveillance3);
                                stepList.Add(AuditStepType.Surveillance4);
                                stepList.Add(AuditStepType.Surveillance5);
                            }
                            break;
                        case AuditStepType.Surveillance1:
                            stepList.Add(AuditStepType.Surveillance1);
                            stepList.Add(AuditStepType.Surveillance2);
                            if (periodicity == AuditCyclePeriodicityType.Biannual)
                            {
                                stepList.Add(AuditStepType.Surveillance3);
                                stepList.Add(AuditStepType.Surveillance4);
                                stepList.Add(AuditStepType.Surveillance5);
                            }
                            break;
                        case AuditStepType.Surveillance2:
                            stepList.Add(AuditStepType.Surveillance2);
                            if (periodicity == AuditCyclePeriodicityType.Biannual)
                            {
                                stepList.Add(AuditStepType.Surveillance3);
                                stepList.Add(AuditStepType.Surveillance4);
                                stepList.Add(AuditStepType.Surveillance5);
                            }
                            break;
                    }
                    break;
            }

            foreach (AuditStepType step in stepList)
            {
                var currentSite = appForm.Sites
                    .Where(s => s.ID == adcSite.SiteID)
                    .FirstOrDefault() ?? new Site();

                var adcStepAudit = new ADCSiteAudit()
                {
                    ID = Guid.NewGuid(),
                    ADCSiteID = adcSite.ID,
                    Value = !isMultisite || currentSite.IsMainSite, // si es un solo sitio o es el principal, por default en true (el sitio recibe todas las auditorias)
                    AuditStep = step,
                    Status = StatusType.Active,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    UpdatedUser = "system",
                };
                adcSiteAuditRepository.Add(adcStepAudit);
                listADCSiteAudits.Add(adcStepAudit);
                hasChanges = true;
            }

            //foreach (AuditStepType step in Enum.GetValues(typeof(AuditStepType)))
            //{
            //    bool addStep = false;

            //    switch (cycleType)
            //    {
            //        case AuditCycleType.Initial:
            //            if (step == AuditStepType.Stage1 
            //                || step == AuditStepType.Stage2
            //                || step == AuditStepType.Surveillance1
            //                || step == AuditStepType.Surveillance2
            //                || (step == AuditStepType.Surveillance3 
            //                    && periodicity == AuditCyclePeriodicityType.Biannual)
            //                || (step == AuditStepType.Surveillance4
            //                    && periodicity == AuditCyclePeriodicityType.Biannual)
            //                || (step == AuditStepType.Surveillance5
            //                    && periodicity == AuditCyclePeriodicityType.Biannual))
            //                addStep = true;
            //            break;
            //        case AuditCycleType.Recertificacion:
            //            if (step == AuditStepType.Recertification
            //                || step == AuditStepType.Surveillance1
            //                || step == AuditStepType.Surveillance2
            //                || (step == AuditStepType.Surveillance3
            //                    && periodicity == AuditCyclePeriodicityType.Biannual)
            //                || (step == AuditStepType.Surveillance4
            //                    && periodicity == AuditCyclePeriodicityType.Biannual)
            //                || (step == AuditStepType.Surveillance5
            //                    && periodicity == AuditCyclePeriodicityType.Biannual))
            //                addStep = true;
            //            break;
            //        case AuditCycleType.Transfer:
            //            if ((step == AuditStepType.Transfer
            //                || step == AuditStepType.Recertification
            //                || step == AuditStepType.Surveillance1
            //                || step == AuditStepType.Surveillance2
            //                || (step == AuditStepType.Surveillance3
            //                    && periodicity == AuditCyclePeriodicityType.Biannual)
            //                || (step == AuditStepType.Surveillance4
            //                    && periodicity == AuditCyclePeriodicityType.Biannual)
            //                || (step == AuditStepType.Surveillance5
            //                    && periodicity == AuditCyclePeriodicityType.Biannual))
            //                && (step >= initialStep 
            //                    || initialStep == AuditStepType.Surveillance1 
            //                    || initialStep == AuditStepType.Surveillance2))
            //                addStep = true;
            //            break;
            //    }

            //    if (addStep)
            //    {
            //        var adcStepAudit = new ADCSiteAudit()
            //        {
            //            ID = Guid.NewGuid(),
            //            ADCSiteID = adcSite.ID,
            //            Value = false,
            //            AuditStep = step,
            //            Status = StatusType.Active,
            //            Created = DateTime.UtcNow,
            //            Updated = DateTime.UtcNow,
            //            UpdatedUser = "system",
            //        };

            //        adcSiteAuditRepository.Add(adcStepAudit);
            //        listADCSiteAudits.Add(adcStepAudit);
            //        hasChanges = true;
            //    }
            //}

            if (hasChanges)
            {
                try
                {
                    await adcSiteAuditRepository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"ADCService.AddADCSiteAuditsAsync: {ex.Message}");
                }
            }

            return listADCSiteAudits;
        } // AddADCSiteAuditsAsync

        /// <summary>
        /// Calcula los valores para un ADCSite tanto su numero de empleados como
        /// el numero de dias en base a MD5, así como el Total de Empleados del ADC
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task RecalcularTotalesAsync(ADC item) 
        {
            // HACK: Buscar los ADCSites de forma manual primero

            if (item.ADCSites != null && item.ADCSites.Any())
            {
                var totalEmployees = 0;
                
                foreach (var adcSite in item.ADCSites
                    .Where(adcsite => adcsite.Status == StatusType.Active))
                {
                    adcSite.TotalInitial = adcSite.InitialMD5 ?? 0;
                    var employeesMD5 = await ADCSiteService.GetEmployeesMD5Async(adcSite.SiteID ?? Guid.Empty);

                    if (employeesMD5.NoEmployees != adcSite.NoEmployees)
                    { 
                        var adcSiteService = new ADCSiteService();

                        await adcSiteService.UpdateEmployeesMD5Async(adcSite.ID);

                        adcSite.NoEmployees = employeesMD5.NoEmployees;
                        adcSite.InitialMD5 = employeesMD5.InitialMD5;
                    }

                    totalEmployees += employeesMD5.NoEmployees;
                }

                item.TotalEmployees = totalEmployees;
            }
            else 
            {
                item.TotalEmployees = 0;
            }
        } // RecalcularTotales

        private string GetHistoricalDataJSON(ADC item)
        {
            var firstSite = item.ADCSites?
                .FirstOrDefault(s => s.Status == StatusType.Active);

            var historicalData = new 
            { 
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
                        acv.ADCConcept.ExtraInfo
                    })
            };

            return JsonSerializer.Serialize(historicalData);
        } // GetHistoricalDataJSON

        // STATIC METHODS

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
                    var noEmployees = item.ADCSites
                        .Where(adcsite => adcsite.Status == StatusType.Active)
                        .Sum(adcsite => adcsite.NoEmployees) ?? 0;

                    foreach (var adcSite in item.ADCSites
                        .Where(adcsite => adcsite.Status == StatusType.Active))
                    {
                        adcSite.Alerts = await ADCSiteService.GetAlertsAsync(adcSite);

                        if (adcSite.Alerts != null && adcSite.Alerts.Any())
                        {   
                            alerts.Add(ADCAlertType.EmployeesMistmatch);
                        }
                    }

                    // Si el total de empleados del ADC no coincide con la suma de empleados de los ADCSites
                    if (item.TotalEmployees != noEmployees 
                        && !alerts.Contains(ADCAlertType.EmployeesMistmatch))
                    {
                        alerts.Add(ADCAlertType.EmployeesMistmatch);
                    }
                } // Validando si cambia el numero de empleados

                // Si el número de ADCSites no coincide con el número de Sites del AppForm
                if (item.AppForm != null
                    && item.AppForm.Sites != null
                    && item.ADCSites != null)
                {
                    if (!SitesMistmatch(item) && !alerts.Contains(ADCAlertType.SitesMistmatch))
                        alerts.Add(ADCAlertType.SitesMistmatch);

                    //var noADCSites = item.ADCSites?.Count(adcsite => adcsite.Status == StatusType.Active) ?? 0;
                    //var noAppFormSites = item.AppForm.Sites.Count(site => site.Status == StatusType.Active);

                    //if (noADCSites != noAppFormSites)
                    //{
                    //    if (!alerts.Contains(ADCAlertType.SitesMistmatch))
                    //        alerts.Add(ADCAlertType.SitesMistmatch);
                    //}
                    //else 
                    //{
                    //    // verificar que sean los mismos Sites
                    //    var sameSites = true;
                    //    foreach (var site in item.AppForm.Sites
                    //        .Where(site => site.Status == StatusType.Active))
                    //    {
                    //        if (!item.ADCSites.Any(adcsite => adcsite.SiteID == site.ID))                        
                    //        {
                    //            sameSites = false;
                    //            break;
                    //        }
                    //    }

                    //    if (!sameSites && !alerts.Contains(ADCAlertType.SitesMistmatch))
                    //    {
                    //        alerts.Add(ADCAlertType.SitesMistmatch);
                    //    }
                    //}
                }
                else
                {
                    // TODO: Realmente no se cargó la información.
                    // Ver otra forma de informarlo sin interrumpir el flujo.
                    if (!alerts.Contains(ADCAlertType.SitesMistmatch))
                        alerts.Add(ADCAlertType.SitesMistmatch);
                }
                // Validando si cambia el numero de ADCSites vs el AppForm
            } // if status < Inactive

            // Otras alertas...

            return alerts;
        } // GetAlertsAsync

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
    }
}