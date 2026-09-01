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
    public class ADCSiteService
    {
        public readonly ADCSiteRepository _repository;

        // CONSTRUCTOR

        public ADCSiteService()
        {
            _repository = new ADCSiteRepository();
        }

        // METHODS

        public PagedList<ADCSite> Gets(ADCSiteQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.ADCID != null && filters.ADCID != Guid.Empty)
            {
                items = items.Where(e => e.ADCID == filters.ADCID);
            }

            if (filters.SiteID != null && filters.SiteID != Guid.Empty)
            {
                items = items.Where(e => e.SiteID == filters.SiteID);
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
                case ADCSiteOrderType.SiteDescription:
                    items = items.OrderBy(e => e.Site.Description);
                    break;
                case ADCSiteOrderType.IsMainSite:
                    items = items.OrderByDescending(e => e.Site.IsMainSite)
                        .ThenByDescending(e => e.Site.Description);
                    break;
                case ADCSiteOrderType.SiteDescriptionDesc:
                    items = items.OrderBy(e => e.Site.Description);
                    break;
                case ADCSiteOrderType.IsMainSiteDesc:
                    items = items.OrderByDescending(e => e.Site.IsMainSite)
                        .ThenByDescending(e => e.Site.Description);
                    break;
                default:
                    items = items.OrderByDescending(e => e.Site.IsMainSite)
                        .ThenByDescending(e => e.Site.Description);
                    break;
            }

            //foreach (var item in items)
            //{
            //    // Get alerts
            //    item.Alerts = GetAlertsAsync(item).GetAwaiter().GetResult();
            //    item.IsMultiStandard = IsMultiStandard(item.ID);
            //    // TODO: Verificar si esto si jala :/
            //}

            var pagedItems = PagedList<ADCSite>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        /// <summary>
        /// Obtiene un listado unico de todos los ADCSites indistintamente
        /// de los ADCs dado el ID de la propuesta.
        /// </summary>
        /// <param name="id">Identificador de la Propuesta</param>
        /// <returns></returns>
        public async Task<List<ADCSite>> GetsByProposalID(Guid id)
        {
            var proposalRepository = new ProposalRepository();
            var proposalItem = await proposalRepository.GetAsync(id)
                ?? throw new BusinessException("The Proposal item not found");

            var adcSites = proposalItem.ADCs != null
                ? proposalItem.ADCs.Where(adc => adc.ADCSites != null)
                    .SelectMany(adc => adc.ADCSites)
                    .ToList()
                : new List<ADCSite>();

            return adcSites;
        } // GetsByProposalID

        /// <summary>
        /// Obtiene un sitio ADC por su ID.
        /// </summary>
        /// <param name="id">Identificador del Sitio para el ADC</param>
        /// <returns></returns>
        public async Task<ADCSite> GetAsync(Guid id)
        {
            //var _md5Repository = new MD5Repository();
            var item = await _repository.GetAsync(id)
                ?? throw new BusinessException("The record was not found");

            // Get alerts
            var alerts = GetAlerts(item);

            if (alerts.Contains(ADCSiteAlertType.EmployeesMistmatch))
            {
                item = item.ADC.Standard.StandardBase == StandardBaseType.ISO22K
                    ? await RefreshISO22KInitialDataAsync(item)
                    : await RefreshInitialDataAsync(item);

                _repository.Update(item);
                try
                {
                    await _repository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"ADCSite.GetAsync: {ex.Message}");
                }
            }

            // item.IsMultiStandard = IsMultiStandard(item.ID);

            return item;
        } // GetAsync

        //public async Task<ADCSite> AddAsync(ADCSite item) // Se crean en el ADCService
        //{
        //    // Validations

        //    if (item.ADCID == null || item.ADCID == Guid.Empty)
        //        throw new BusinessException("The ADC ID is required.");

        //    // Assigning values

        //    item.ID = Guid.NewGuid();
        //    item.Status = StatusType.Nothing;
        //    item.Created = DateTime.UtcNow;
        //    item.Updated = DateTime.UtcNow;

        //    // Execute queries

        //    try
        //    {
        //        await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
        //        _repository.Add(item);
        //        await _repository.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessException($"ADCSite.AddAsync: {ex.Message}");
        //    } // AddAsync

        //    return item;
        //} // AddAsync

        public async Task<ADCSite> UpdateAsync(ADCSite item)
        { 
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");
            
            // HACK: IMPORTANTE Ver que realmente se va a seguir actualizando despues de que sea Inactive

            await ValidateUpdateItemAsync(item, foundItem);
            await SetValuesUpdateItemAsync(item, foundItem);
            
            // Execute queries

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCSite.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task UpdateInitialDataAsync(Guid adcSiteID)
        {
            var foundItem = await _repository.GetAsync(adcSiteID)
                ?? throw new BusinessException("The record to update was not found");

            var refreshedItem = await RefreshInitialDataAsync(foundItem);

            foundItem.MD5ID = refreshedItem.MD5ID;
            foundItem.InitialMD5 = refreshedItem.InitialMD5;
            foundItem.TotalWorkers = refreshedItem.TotalWorkers;
            foundItem.WorkersOnSite = refreshedItem.WorkersOnSite;
            foundItem.WorkersOffSite = refreshedItem.WorkersOffSite;
            
            // Execute queries
            
            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCSite.UpdateInitialDataAsync: {ex.Message}");
            }
        } // UpdateInitialDataAsync

        [Obsolete("Use RefreshInitialDataAsync instead", false)]
        public async Task UpdateEmployeesMD5Async(Guid adcSiteID)
        {
            var foundItem = await _repository.GetAsync(adcSiteID)
                ?? throw new BusinessException("The record to update was not found");
            var appFormItem = foundItem.ADC?.AppForm;

            if (appFormItem == null && foundItem.ADC != null)
            { 
                appFormItem = await new AppFormRepository().GetAsync(foundItem.ADC.AppFormID)
                    ?? throw new BusinessException("The Application Form associated with the ADC was not found");
            }
            
            var tableType = AuditCycleCalculations
                .GetMD5TableType(foundItem.ADC?.Standard?.StandardBase ?? StandardBaseType.Nothing);            
            var maxRiskLevelCategory = AuditCycleCalculations
                .GetMaxRiskLevelCategory(appFormItem);
            var _totalWorkers = OrganizationCalculations
                .GetTotalWorkers(foundItem.Site);
            var md5Item = await new MD5Repository()
                .GetItemByEmployeesAsync(_totalWorkers, tableType);
            var days = AuditCycleCalculations
                .GetInitialAuditDaysByRiskLevelCategory(md5Item, maxRiskLevelCategory);

            foundItem.MD5ID = md5Item.ID;
            foundItem.InitialMD5 = days;
            foundItem.WorkersOnSite = OrganizationCalculations
                .GetWorkersOnSite(foundItem.Site);
            foundItem.WorkersOffSite = OrganizationCalculations
                .GetWorkersOffSite(foundItem.Site);
            foundItem.TotalWorkers = _totalWorkers;

            // Execute queries

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCSite.UpdateEmployeesMD5Async: {ex.Message}");
            }
        } // UpdateEmployeesMD5Async

        public async Task UpdateISO22KInitialDataAsync(Guid adcSiteID)
        {
            var foundItem = await _repository.GetAsync(adcSiteID)
                ?? throw new BusinessException("The record to update was not found");

            var refreshedItem = await RefreshISO22KInitialDataAsync(foundItem);

            foundItem.MD5ID = refreshedItem.MD5ID;
            foundItem.InitialMD5 = refreshedItem.InitialMD5;
            foundItem.TotalWorkers = refreshedItem.TotalWorkers;
            foundItem.WorkersOnSite = refreshedItem.WorkersOnSite;
            foundItem.WorkersOffSite = refreshedItem.WorkersOffSite;
            //var appForm = foundItem.ADC?.AppForm;

            //if (appForm == null && foundItem.ADC != null)
            //{
            //    appForm = await new AppFormRepository().GetAsync(foundItem.ADC.AppFormID)
            //        ?? throw new BusinessException("The Application Form associated with the ADC was not found");
            //}

            //foundItem.MD5ID = md5ID;
            //foundItem.WorkersOnSite = OrganizationCalculations
            //    .GetWorkersOnSite(foundItem.Site);
            //foundItem.WorkersOffSite = OrganizationCalculations
            //    .GetWorkersOffSite(foundItem.Site);
            //foundItem.TotalWorkers = OrganizationCalculations
            //    .GetTotalWorkers(foundItem.WorkersOnSite, foundItem.WorkersOffSite);

            //if (foundItem.Site.IsMainSite)
            //{
            //    foundItem.InitialMD5 = mainDays;
            //    foundItem.TotalInitial = mainDays;
            //}
            //else
            //{
            //    var halfDays = mainDays / 2; // El 50% del sitio principal, para cualquier sitio secundario 
            //    foundItem.InitialMD5 = halfDays;
            //    foundItem.TotalInitial = halfDays;
            //}

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCSite.UpdateEmployeesDaysISO22KAsync: {ex.Message}");
            }
        } // UpdateEmployeesDaysISO22KAsync

        public async Task<List<ADCSite>> UpdateListAsync(List<ADCSite> adcSites)
        {
            if (!(adcSites?.Any() ?? false)) // Valida si la lista es nula o vacía
                throw new ArgumentException("The list of ADC Sites to update is empty.");

            //var _conceptValueService = new ADCConceptValueService();
            var areUpdatedItems = false;
            var updatedItems = new List<ADCSite>();

            foreach (var adcSite in adcSites)
            {
                var foundItem = await _repository.GetAsync(adcSite.ID)
                    ?? throw new BusinessException($"One of the records (ADC Site) to Update was not found: {adcSite.ID}");
                //var listConceptValues = new List<ADCConceptValue>();

                await ValidateUpdateItemAsync(adcSite, foundItem);               
                await SetValuesUpdateItemAsync(adcSite, foundItem);

                //if (adcSite.ADCConceptValues?.Any() ?? false) // en adcSite.ADCConceptValues traigo los nuevos valores
                //{
                //    listConceptValues = await _conceptValueService
                //        .UpdateListAsync(adcSite.ADCConceptValues.ToList());
                //}

                _repository.Update(foundItem);
                areUpdatedItems = true;

                //foundItem.ADCConceptValues = listConceptValues; // HACK: Ver si esto jala
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
                    throw new BusinessException($"ADCSiteService.UpdateListAsync: {ex.Message}");
                }
            }

            return updatedItems;
        } // UpdateListAsync

        public async Task DeleteAsync(ADCSite item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            if (foundItem.Status == StatusType.Deleted)
            {
                // TODO: Ver si se necesita alguna validación antes de eliminar
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
                throw new BusinessException($"ADCSite.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync

        #region " PRIVATE "

        private async Task ValidateUpdateItemAsync(ADCSite item, ADCSite foundItem)
        {   

            if (foundItem.Status == StatusType.Nothing) // Si es nuevo...
            {
                if (item.SiteID == null || item.SiteID == Guid.Empty)
                    throw new ArgumentException("The Site ID is required");
            } // xBlaze Update: Creo que no se va a utilizar, se genera automáticamente

            // 0. Validar que si no es multiestandard no maneje MD11 o
            //    ignorar los valores y archivos
            // 1. Validar que el descuento de totalInitial no sea mayor al 30% del InitialMD5
            // 2. Validar que TotalInitial no sea menor a 2 días
            // 3. Validar que si la organizacion maneja un solo standard no aplique el MD11
            //  3.1. Validar que el descuento de MD11 no sea mayor al 20% de TotalInitial
            
        } // ValidateUpdateItemAsync

        private async Task SetValuesUpdateItemAsync(ADCSite item, ADCSite foundItem)
        {   
            if (foundItem.Status == StatusType.Nothing) // Si es nuevo...
            {
                foundItem.SiteID = item.SiteID; // Solo se asigna si es nuevo
            }

            if (item.Status < StatusType.Inactive) // Si está activo o es nuevo, recalcular
            {
                // NOTA: La mayoria de calculos se va a realizar en el frontend para que se
                // aprueben en tiempo real
                // NOTA 2: En teoria no deberia necesitarse esto, pues al enviarlo al
                // frontend desde ADCService.GetAsync ya se calculan los valores de
                // InitialMD5, TotalWorkers, WorkersOnSite y WorkersOffSite y no cambian
                item = foundItem.ADC.Standard.StandardBase == StandardBaseType.ISO22K
                    ? await RefreshISO22KInitialDataAsync(item)
                    : await RefreshInitialDataAsync(item);

                foundItem.MD5ID = item.ID;
                foundItem.InitialMD5 = item.InitialMD5;
                foundItem.TotalWorkers = item.TotalWorkers;
                foundItem.WorkersOnSite = item.WorkersOnSite;
                foundItem.WorkersOffSite = item.WorkersOffSite;
            }

            foundItem.TotalInitial = item.TotalInitial;     // Se obtiene de la diferencia del InitialMD5 con la suma de todos los Concept Values, no debe reducirse más de un 30%
            foundItem.MD11 = item.MD11;                     // Por lo pronto manual hasta que entienda el MD11
            foundItem.MD11Filename = string.IsNullOrEmpty(item.MD11Filename)
                ? foundItem.MD11Filename
                : item.MD11Filename;                        // Nombre del archivo de evidencia del MD11
            foundItem.MD11UploadedBy = string.IsNullOrEmpty(item.MD11UploadedBy)
                ? foundItem.MD11UploadedBy
                : item.MD11UploadedBy;                      // Usuario que subió el archivo del MD11
            foundItem.Total = item.Total;                   // Total en días ya sea de TotalInitial o de MD11
            foundItem.Surveillance = item.Surveillance;     // Debe ser una tercera parte del TotalInitial (x)/3
            foundItem.Recertification = item.Recertification; // Debe reducirse un 33% del TotalInitial pero su reducción no puede ser menor al 50% de Initial MD5
            foundItem.ExtraInfo = item.ExtraInfo;
            foundItem.Status = foundItem.Status == StatusType.Nothing && item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status != StatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;
        } // SetValuesUpdateItemAsync

        #endregion " PRIVATE "

        #region " STATICS "

        /// <summary>
        /// Crea los datos iniciales de un ADCSite, recalculando los 
        /// valores de InitialMD5, TotalWorkers, WorkersOnSite y 
        /// WorkersOffSite.
        /// </summary>
        /// <param name="adc">El ADC a refrescar</param>
        /// <param name="site">El Site a refrescar</param>
        /// <returns></returns>
        public static async Task<ADCSite> CreateInitialDataAsync(ADC adc, Site site)
        {
            var _md5Repository = new MD5Repository();

            if (adc.AppForm == null)
                throw new BusinessException("The ADC's AppForm is required to create the initial data for ADCSite.");

            if (adc.Standard == null 
                || adc.Standard.StandardBase == null 
                || adc.Standard.StandardBase == StandardBaseType.Nothing)
                throw new BusinessException("The ADC's Standard is required to create the initial data for ADCSite.");

            // Obtener el nivel de riesgo máximo del AppForm
            var maxRiskLevelCategory = AuditCycleCalculations
                    .GetMaxRiskLevelCategory(adc.AppForm ?? new AppForm());
            // Obtener de acuerdo con el standard, el tipo de tabla MD5 a consultar
            var tableType = AuditCycleCalculations
                .GetMD5TableType(adc.Standard?.StandardBase ?? StandardBaseType.Nothing);
            var totalWorkers = OrganizationCalculations
                .GetTotalWorkers(site);
            var md5Item = await _md5Repository
                .GetItemByEmployeesAsync(totalWorkers, tableType);
            var days = AuditCycleCalculations
                .GetInitialAuditDaysByRiskLevelCategory(md5Item, maxRiskLevelCategory);

            var adcSite = new ADCSite
            {
                ID = Guid.NewGuid(),
                ADCID = adc.ID,
                SiteID = site.ID,
                MD5ID = md5Item.ID,
                InitialMD5 = days,
                TotalWorkers = totalWorkers,
                WorkersOnSite = OrganizationCalculations.GetWorkersOnSite(site),
                WorkersOffSite = OrganizationCalculations.GetWorkersOffSite(site),
                TotalInitial = days,
                Status = StatusType.Active,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            return adcSite;
        } // CreateInitialDataAsync

        public static async Task<ADCSite> CreateISO22KInitialDataAsync(ADC adc, Site site, MD5 md5)
        {
            if (adc.AppForm == null)
                throw new BusinessException("The ADC's AppForm is required to create the initial data for ADCSite.");

            var mainDays = AuditCycleCalculations
                .GetInitialAuditDaysByRiskLevelCategory(md5, RiskLevelCategoryType.Medium);
            var totalWorkers = OrganizationCalculations.GetTotalWorkers(site);
            mainDays = AuditCycleCalculations
                .GetInitialAuditDaysForISO22K(mainDays, adc.AppForm.Category22K, adc.AppForm.HACCPCount ?? 0);
            var halfDays = mainDays / 2; // El 50% del sitio principal, para cualquier sitio secundario

            var adcSite = new ADCSite { 
                ID = Guid.NewGuid(),
                ADCID = adc.ID,
                SiteID = site.ID,
                MD5ID = md5.ID,
                TotalWorkers = totalWorkers,
                WorkersOnSite = OrganizationCalculations.GetWorkersOnSite(site),
                WorkersOffSite = OrganizationCalculations.GetWorkersOffSite(site),
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                Status = StatusType.Active
            };

            if (site.IsMainSite)
            { 
                adcSite.InitialMD5 = mainDays;
                adcSite.TotalInitial = mainDays;
            }
            else
            {
                adcSite.InitialMD5 = halfDays;
                adcSite.TotalInitial = halfDays;
            }

            return adcSite;
        } // CreateISO22KInitialDataAsync

        /// <summary>
        /// Refresca los datos iniciales de un ADCSite, recalculando los 
        /// valores de InitialMD5, TotalWorkers, WorkersOnSite y 
        /// WorkersOffSite.
        /// </summary>
        /// <param name="adcSite"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public static async Task<ADCSite> RefreshInitialDataAsync(ADCSite adcSite)
        {
            var _md5Repository = new MD5Repository();
            var _adcSiteRepository = new ADCSiteRepository();

            var foundItem = await _adcSiteRepository.GetAsync(adcSite.ID, true)
                ?? throw new BusinessException("The record to refresh was not found");

            var maxRiskLevelCategory = AuditCycleCalculations
                    .GetMaxRiskLevelCategory(foundItem.ADC?.AppForm ?? new AppForm());
            var tableType = AuditCycleCalculations
                .GetMD5TableType(foundItem.ADC?.Standard?.StandardBase ?? StandardBaseType.Nothing);
            var totalWorkers = OrganizationCalculations
                .GetTotalWorkers(foundItem.Site);
            var md5Item = await _md5Repository
                .GetItemByEmployeesAsync(totalWorkers, tableType);
            var days = AuditCycleCalculations
                .GetInitialAuditDaysByRiskLevelCategory(md5Item, maxRiskLevelCategory);

            adcSite.MD5ID = md5Item.ID;
            adcSite.InitialMD5 = days;
            adcSite.TotalWorkers = totalWorkers;
            adcSite.WorkersOnSite = OrganizationCalculations.GetWorkersOnSite(foundItem.Site);
            adcSite.WorkersOffSite = OrganizationCalculations.GetWorkersOffSite(foundItem.Site);
            
            return adcSite;
        } // RefreshInitialDataAsync

        public static async Task<ADCSite> RefreshISO22KInitialDataAsync(ADCSite adcSite)
        {
            var appFormRepository = new AppFormRepository();
            var md5Repository = new MD5Repository();
            var adcSiteRepository = new ADCSiteRepository();

            var foundItem = await adcSiteRepository.GetAsync(adcSite.ID, true)
                ?? throw new BusinessException("The record to refresh was not found");

            var appForm = await appFormRepository.GetAsync(foundItem.ADC.AppFormID)
                ?? throw new BusinessException("The AppForm for refresh initial data was not found.");

            var totalEmployeesAllSites = OrganizationCalculations
                .GetTotalWorkers(appForm.Sites.ToList());
            var md5ItemAllSites = await md5Repository
                .GetItemByEmployeesAsync(totalEmployeesAllSites, MD5TableType.FTE);

            var mainDays = AuditCycleCalculations
                .GetInitialAuditDaysByRiskLevelCategory(md5ItemAllSites, RiskLevelCategoryType.Medium);
            mainDays = AuditCycleCalculations
                .GetInitialAuditDaysForISO22K(mainDays, appForm.Category22K, appForm.HACCPCount ?? 0);

            adcSite.MD5ID = md5ItemAllSites.ID;
            adcSite.WorkersOnSite = OrganizationCalculations
                .GetWorkersOnSite(foundItem.Site);
            adcSite.WorkersOffSite = OrganizationCalculations
                .GetWorkersOffSite(foundItem.Site);
            adcSite.TotalWorkers = OrganizationCalculations
                .GetTotalWorkers(adcSite.WorkersOnSite, adcSite.WorkersOffSite);

            if (foundItem.Site.IsMainSite)
            {
                adcSite.InitialMD5 = mainDays;
                //adcSite.TotalInitial = mainDays;
            }
            else
            {
                var halfDays = mainDays / 2; // El 50% del sitio principal, para cualquier sitio secundario 
                adcSite.InitialMD5 = halfDays;
                //adcSite.TotalInitial = halfDays;
            }

            return adcSite;
        } // RefreshISO22KInitialDataAsync

        //public static async Task<List<ADCSiteAlertType>> GetAlertsAsync(ADCSite item)
        //{
        //    var alerts = new List<ADCSiteAlertType>();

        //    var noEmployees = item.Site.Shifts
        //        .Where(s => s.Status == StatusType.Active)
        //        .Sum(s => s.NoEmployees) ?? 0;

        //    if (noEmployees != (item.NoEmployees ?? 0)) { 
        //        alerts.Add(ADCSiteAlertType.EmployeesMistmatch);
        //    }

        //    //// Concept value decrease exceeded
        //    //if (item.TotalInitial != null && item.TotalInitial > 0
        //    //    && item.MD11 != null && item.MD11 < 0.7m * item.TotalInitial)
        //    //{
        //    //    alerts.Add(ADCSiteAlertType.ConceptValueDecreaseExceeded);
        //    //}

        //    //// MD11 reduction exceeded
        //    //if (item.MD11 != null && item.MD11 < 0.7m * item.TotalInitial)
        //    //{
        //    //    alerts.Add(ADCSiteAlertType.MD11ReductionExceeded);
        //    //}

        //    return alerts;
        //} // GetAlertsAsync

        public static List<ADCSiteAlertType> GetAlerts(ADCSite item)
        {
            var alerts = new List<ADCSiteAlertType>();
            var totalWorkers = OrganizationCalculations.GetTotalWorkers(item.Site);

            if (totalWorkers != (item.TotalWorkers ?? 0))
            {
                alerts.Add(ADCSiteAlertType.EmployeesMistmatch);
            }

            //// Concept value decrease exceeded - xB: 20260824 Creo que no se van a necesitar
            //if (item.TotalInitial != null && item.TotalInitial > 0
            //    && item.MD11 != null && item.MD11 < 0.7m * item.TotalInitial)
            //{
            //    alerts.Add(ADCSiteAlertType.ConceptValueDecreaseExceeded);
            //}

            //// MD11 reduction exceeded
            //if (item.MD11 != null && item.MD11 < 0.7m * item.TotalInitial)
            //{
            //    alerts.Add(ADCSiteAlertType.MD11ReductionExceeded);
            //}

            return alerts;
        } // GetAlerts

        public static bool IsMultiStandard(Guid ADCSiteID)
        {
            if (ADCSiteID == Guid.Empty)
                throw new ArgumentException("The ADC Site ID is required.");

            var _repository = new ADCSiteRepository();

            return _repository.OrganizationStandardCount(ADCSiteID) > 1;
        } // IsMultiStandard

        #endregion // STATICS

    } // ADCSiteService
}