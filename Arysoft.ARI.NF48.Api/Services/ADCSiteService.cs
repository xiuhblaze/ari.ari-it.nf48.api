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
        /// Obtiene un sitio ADC por su ID.
        /// </summary>
        /// <param name="id">Identificador del Sitio para el ADC</param>
        /// <returns></returns>
        public async Task<ADCSite> GetAsync(Guid id)
        {
            var item = await _repository.GetAsync(id)
                ?? throw new BusinessException("The record was not found");

            // Get alerts
            var alerts = await GetAlertsAsync(item);

            if (alerts.Contains(ADCSiteAlertType.EmployeesMistmatch))
            {
                // Volver a obtener el MD5 y guardar antes de enviar
                var employeesMD5 = await GetEmployeesMD5Async(item.SiteID ?? Guid.Empty);
                
                item.InitialMD5 = employeesMD5.InitialMD5;
                item.NoEmployees = employeesMD5.NoEmployees;

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

        //public bool IsMultiStandard(Guid ADCSiteID)
        //{
        //    if (ADCSiteID == Guid.Empty)
        //        throw new ArgumentException("The ADC Site ID is required.");

        //    return _repository.OrganizationStandardCount(ADCSiteID) > 1;
        //} // IsMultiStandard

        public async Task<ADCSite> AddAsync(ADCSite item)
        {
            // Validations

            if (item.ADCID == null || item.ADCID == Guid.Empty)
                throw new BusinessException("The ADC ID is required.");

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
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCSite.AddAsync: {ex.Message}");
            } // AddAsync

            return item;
        } // AddAsync

        public async Task<ADCSite> UpdateAsync(ADCSite item)
        { 
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");
            
            //var siteRepository = new SiteRepository();
            //var md5Repository = new MD5Repository();

            // Validations

            //if (foundItem.Status == StatusType.Nothing) // Si es nuevo...
            //{ 
            //    if (item.SiteID == null || item.SiteID == Guid.Empty)
            //    {
            //        throw new ArgumentException("The Site ID is required");
            //    }

            //    foundItem.SiteID = item.SiteID; // Solo se asigna si es nuevo
            //} // xBlaze Update: Creo que no se va a utilizar, se genera automáticamente

            //if (item.Status < StatusType.Inactive) // Si está activo o es nuevo, recalcular
            //{ 
            //    //var site = await siteRepository.GetAsync(item.SiteID ?? Guid.Empty)
            //    //    ?? throw new BusinessException("The Site ID does not exist");
            //    //var employeesCount = site.Shifts != null
            //    //    ? site.Shifts.Where(s => s.Status == StatusType.Active)
            //    //        .Sum(s => s.NoEmployees) ?? 0
            //    //    : 0;

            //    //if (foundItem.NoEmployees != employeesCount) // Cambió el número de empleados, volver a obtener InitialMD5
            //    //{ 
            //    //    item.InitialMD5 = await md5Repository.GetDaysAsync(employeesCount);
            //    //    item.NoEmployees = employeesCount;
            //    //}
            //    var employeesMD5 = await GetEmployeesMD5Async(item.SiteID ?? Guid.Empty);

            //    foundItem.InitialMD5 = employeesMD5.InitialMD5;
            //    foundItem.NoEmployees = employeesMD5.NoEmployees;
            //}

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

        public async Task UpdateEmployeesMD5Async(Guid adcSiteID)
        {
            var foundItem = await _repository.GetAsync(adcSiteID)
                ?? throw new BusinessException("The record to update was not found");

            var employeesMD5 = await GetEmployeesMD5Async(foundItem.SiteID ?? Guid.Empty);

            foundItem.InitialMD5 = employeesMD5.InitialMD5;
            foundItem.NoEmployees = employeesMD5.NoEmployees;

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

        // PRIVATE

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
                // NOTA: La mayoria de calculos se va a realizar en el frontend

                var employeesMD5 = await GetEmployeesMD5Async(foundItem.SiteID ?? Guid.Empty);

                foundItem.InitialMD5 = employeesMD5.InitialMD5;
                foundItem.NoEmployees = employeesMD5.NoEmployees;
            }

            foundItem.TotalInitial = item.TotalInitial;     // Se obtiene de la diferencia del InitialMD5 con la suma de todos los Concept Values, no debe reducirse más de un 30%
            foundItem.MD11 = item.MD11;                     // Por lo pronto manual hasta que entienda el MD11
            foundItem.MD11Filename = String.IsNullOrEmpty(item.MD11Filename)
                ? foundItem.MD11Filename
                : item.MD11Filename;                        // Nombre del archivo de evidencia del MD11
            foundItem.MD11UploadedBy = string.IsNullOrEmpty(item.MD11UploadedBy)
                ? foundItem.MD11UploadedBy
                : item.MD11UploadedBy;                      // Usuario que subió el archivo del MD11
            foundItem.Total = item.Total;                   // Total en días ya sea de TotalInitial o de MD11
            foundItem.Surveillance = item.Surveillance;     // Debe ser una tercera parte del TotalInitial (x)/3
            foundItem.ExtraInfo = item.ExtraInfo;
            foundItem.Status = foundItem.Status == StatusType.Nothing && item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status != StatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;
        } // SetValuesUpdateItemAsync

        // STATICs

        public static async Task<EmployeesMD5> GetEmployeesMD5Async(Guid siteID)
        {
            var siteRepository = new SiteRepository();
            var md5Repository = new MD5Repository();
            var site = await siteRepository.GetAsync(siteID)
                    ?? throw new BusinessException("The Site ID does not exist");
            var employeesCount = site.Shifts != null
                ? site.Shifts.Where(s => s.Status == StatusType.Active)
                    .Sum(s => s.NoEmployees) ?? 0
                : 0;
            var initialMD5 = await md5Repository.GetDaysAsync(employeesCount);

            return new EmployeesMD5
            {
                InitialMD5 = initialMD5,
                NoEmployees = employeesCount
            };
        } // GetEmployeesMD5Async

        public static async Task<List<ADCSiteAlertType>> GetAlertsAsync(ADCSite item)
        {
            var alerts = new List<ADCSiteAlertType>();

            var noEmployees = item.Site.Shifts
                .Where(s => s.Status == StatusType.Active)
                .Sum(s => s.NoEmployees) ?? 0;

            if (noEmployees != (item.NoEmployees ?? 0)) { 
                alerts.Add(ADCSiteAlertType.EmployeesMistmatch);
            }

            //// Concept value decrease exceeded
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
        } // GetAlertsAsync

        public static List<ADCSiteAlertType> GetAlerts(ADCSite item)
        {
            var alerts = new List<ADCSiteAlertType>();

            var noEmployees = item.Site.Shifts
                .Where(s => s.Status == StatusType.Active)
                .Sum(s => s.NoEmployees) ?? 0;

            if (noEmployees != (item.NoEmployees ?? 0))
            {
                alerts.Add(ADCSiteAlertType.EmployeesMistmatch);
            }

            //// Concept value decrease exceeded
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

    } // ADCSiteService
}