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

            foreach (var item in items)
            {
                // Get alerts
                item.Alerts = GetAlertsAsync(item).GetAwaiter().GetResult();
            }

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
            item.Alerts = await GetAlertsAsync(item);

            return item;
        } // GetAsync

        public async Task<ADCSite> AddAsync(ADCSite item)
        {
            // Validations

            if (item.ADCID == null || item.ADCID == Guid.Empty)
                throw new ArgumentException("The ADC ID is required.");

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
            
            var siteRepository = new SiteRepository();
            var md5Repository = new MD5Repository();

            // Validations

            if (foundItem.Status == StatusType.Nothing) // Si es nuevo...
            { 
                if (item.SiteID == null || item.SiteID == Guid.Empty)
                {
                    throw new ArgumentException("The Site ID is required");
                }

                foundItem.SiteID = item.SiteID; // Solo se asigna si es nuevo
            } // xBlaze Update: Creo que no se va a utilizar, se genera automáticamente

            if (item.Status < StatusType.Inactive) // Si está activo o es nuevo, recalcular
            { 
                var site = await siteRepository.GetAsync(item.SiteID ?? Guid.Empty)
                    ?? throw new BusinessException("The Site ID does not exist");
                var employeesCount = site.Shifts != null
                    ? site.Shifts.Where(s => s.Status == StatusType.Active)
                        .Sum(s => s.NoEmployees) ?? 0
                    : 0;

                if (foundItem.Employees != employeesCount) // Cambió el número de empleados, volver a obtener InitialMD5
                { 
                    item.InitialMD5 = await md5Repository.GetDaysAsync(employeesCount);
                    item.Employees = employeesCount;
                }
            }

            // HACK: Probablemente aqui va el recalculo para el TotalInitial, pero ahorita no jojojo

            // Assigning values

            foundItem.InitialMD5 = item.InitialMD5;     // Este se va a obtener de la tabla MD5
            foundItem.Employees = item.Employees;       // Este se va a obtener de Sites
            foundItem.TotalInitial = item.TotalInitial; // Se obtiene de la diferencia del InitialMD5 con la suma de todos los Concept Values, no debe reducirse más de un 30%
            foundItem.MD11 = item.MD11;                 // Por lo pronto manual hasta que entienda el MD11
            foundItem.Surveillance = item.Surveillance; // Debe ser una tercera parte del TotalInitial (x)/3
            foundItem.RR = item.RR;                     // Deben ser dos terceras partes del TotalInitial (2x)/3
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
                throw new BusinessException($"ADCSite.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

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

        public static async Task<List<ADCSiteAlertType>> GetAlertsAsync(ADCSite item)
        {
            var alerts = new List<ADCSiteAlertType>();

            var noEmployees = item.Site.Shifts
                .Where(s => s.Status == StatusType.Active)
                .Sum(s => s.NoEmployees) ?? 0;

            if (noEmployees != (item.Employees ?? 0)) { 
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
    } // ADCSiteService
}