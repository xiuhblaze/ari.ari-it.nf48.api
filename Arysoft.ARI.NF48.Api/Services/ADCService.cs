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
                // TODO: Verificar si esto si jala :/
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
            
            item.Alerts = await GetAlertsAsync(item);

            if (item.Alerts.Count > 0)
            {
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
            }

            return item;
        } // GetAsync

        public async Task<ADC> AddAsync(ADC item)
        {
            // Validations

            if (item.AppFormID == null || item.AppFormID == Guid.Empty)            
                throw new BusinessException("The AppForm ID is required.");

            // Validar que el AppForm no tenga un ADC

            if (await _repository.AppFormHasValidADCAsync(item.AppFormID))
                throw new BusinessException("The Application Form already has a valid ADC");
            
            // Set default values
            
            item.ID = Guid.NewGuid();
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

            await ProcesarADCAsync(item);

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

            //// Validations

            //// - no se me ocurre que ahorita

            //// - Dependiendo del status, realizar diferentes acciones
            //if (foundItem.Status != item.Status)
            //{
            //    // - 'orita no me acuerdo...
            //    switch (item.Status) // Si el nuevo status es...
            //    {
            //        case ADCStatusType.Review:

            //            if (string.IsNullOrEmpty(item.ReviewComments))
            //                throw new BusinessException("Comments are required when send to Review.");

            //            foundItem.ReviewDate = DateTime.UtcNow;
            //            foundItem.ReviewComments = item.ReviewComments;
            //            break;

            //        case ADCStatusType.Rejected:
            //            if (string.IsNullOrEmpty(item.ReviewComments))
            //                throw new BusinessException("Comments are required when rejected.");

            //            foundItem.ReviewDate = DateTime.UtcNow;
            //            foundItem.ReviewComments = item.ReviewComments;
            //            foundItem.UserReview = item.UpdatedUser;
            //            break;

            //        case ADCStatusType.Active:
            //            if (foundItem.Status != ADCStatusType.Review)
            //                throw new BusinessException("Only items in Review can be set to Active.");
            //            foundItem.UserReview = item.UpdatedUser;
            //            foundItem.ActiveDate = DateTime.UtcNow;
            //            break;

            //        case ADCStatusType.Inactive:

            //            break;
            //    }
            //}

            //// Assigning values

            //foundItem.Description = item.Description;
            //foundItem.TotalInitial = item.TotalInitial;
            //foundItem.TotalMD11 = item.TotalMD11;
            //foundItem.TotalSurveillance = item.TotalSurveillance;
            //foundItem.TotalRR = item.TotalRR;
            //foundItem.ExtraInfo = item.ExtraInfo;
            //foundItem.Status = foundItem.Status == ADCStatusType.Nothing && item.Status == ADCStatusType.Nothing
            //    ? ADCStatusType.New
            //    : item.Status != ADCStatusType.Nothing
            //        ? item.Status
            //        : foundItem.Status;
            //foundItem.Updated = DateTime.UtcNow;
            //foundItem.UpdatedUser = item.UpdatedUser;

            ValidateUpdateItem(item, foundItem);
            SetValuesUpdateItem(item, foundItem);

            //if (item.Status < ADCStatusType.Inactive)
            //{
            //    await ProcesarADCAsync(foundItem);
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

            //if (item.Status < ADCStatusType.Inactive) // Al parecer lo wa hacer en el frontend
            //    RecalcularTotales(foundItem);

            foundItem.Alerts = await GetAlertsAsync(foundItem);

            return foundItem;
        } // UpdateAsync


        public async Task<ADC> UpdateCompleteADCAsync(ADC item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            var _adcSitesService = new ADCSiteService();

            ValidateUpdateItem(item, foundItem);
            SetValuesUpdateItem(item, foundItem);

            var listSites = new List<ADCSite>();

            if (item.ADCSites?.Any() ?? false) // en item.ADCSites traigo los nuevos valores
            {
                listSites = await _adcSitesService
                    .UpdateListAsync(item.ADCSites.ToList());
            }

            if (item.Status < ADCStatusType.Inactive)
            {
                await ProcesarADCAsync(foundItem);
            }

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
            foundItem.Alerts = await GetAlertsAsync(foundItem);

            return foundItem;
        } // UpdateListAsync

        public async Task DeleteAsync(ADC item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            if (item.Status == ADCStatusType.Deleted) // Eliminación física
            {
                _repository.Delete(item);
            }
            else // Eliminación lógica
            {
                item.Status = foundItem.Status < ADCStatusType.Cancel
                    ? ADCStatusType.Cancel
                    : ADCStatusType.Deleted;
                item.Updated = DateTime.UtcNow;
                item.UpdatedUser = item.UpdatedUser;

                _repository.Update(item);
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
                }
            }

        } // ValidateUpdateItem

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

        private async Task RegisterADCConceptsAsync(ADCSite adcSite, Guid standardID)
        { 
            var adcConceptRepository = new ADCConceptRepository();
            var adcConceptValueRepository = new ADCConceptValueRepository();

            var concepts = adcConceptRepository
                .Gets()
                .Where(c => c.StandardID == standardID
                    && c.Status == StatusType.Active)
                .ToList() ?? throw new BusinessException("No ADC Concepts found for the Standard.");
            var hasChanges = false;

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
        } // RegisterADCConceptsAsync

        private async Task RecalcularTotalesAsync(ADC item) 
        {
            // HACK: Buscar los ADCSites de forma manual primero

            if (item.ADCSites != null && item.ADCSites.Any())
            {
                var totalEmployees = 0;
                
                foreach (var adcSite in item.ADCSites
                    .Where(adcsite => adcsite.Status == StatusType.Active))
                {
                    var totalInitial = adcSite.InitialMD5 ?? 0;

                    adcSite.TotalInitial = totalInitial;

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

        public static async Task<List<ADCAlertType>> GetAlertsAsync(ADC item)
        { 
            var alerts = new List<ADCAlertType>();
            
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
            }

            // Si el número de ADCSites no coincide con el número de Sites del AppForm
            if (item.AppForm != null && item.AppForm.Sites != null)
            {
                var noADCSites = item.ADCSites?.Count(adcsite => adcsite.Status == StatusType.Active) ?? 0;
                var noAppFormSites = item.AppForm.Sites.Count(site => site.Status == StatusType.Active);

                if (noADCSites != noAppFormSites)
                {
                    if (!alerts.Contains(ADCAlertType.SitesMistmatch))
                        alerts.Add(ADCAlertType.SitesMistmatch);
                }
                else 
                {
                    // verificar que sean los mismos Sites
                    var sameSites = true;
                    foreach (var site in item.AppForm.Sites
                        .Where(site => site.Status == StatusType.Active))
                    {
                        if (!item.ADCSites.Any(adcsite => adcsite.SiteID == site.ID))                        
                        {
                            sameSites = false;
                            break;
                        }
                    }

                    if (!sameSites && !alerts.Contains(ADCAlertType.SitesMistmatch))
                    {
                        alerts.Add(ADCAlertType.SitesMistmatch);
                    }
                }
            }

            // Otras alertas...

            return alerts;
        } // GetAlertsAsync
    }
}