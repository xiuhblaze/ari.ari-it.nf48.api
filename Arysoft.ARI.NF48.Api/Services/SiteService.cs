using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class SiteService
    {
        private readonly SiteRepository _siteRepository;

        // CONSTRUCTOR

        public SiteService()
        {
            _siteRepository = new SiteRepository();
        }

        // METHODS

        public PagedList<Site> Gets(SiteQueryFilters filters)
        {
            var items = _siteRepository.Gets();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e =>
                    (e.Description != null && e.Description.ToLower().Contains(filters.Text))
                    || (e.Address != null && e.Address.ToLower().Contains(filters.Text))
                    || (e.Country != null && e.Country.ToLower().Contains(filters.Text))
                );
            }

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty)
            {
                items = items.Where(e => e.OrganizationID == filters.OrganizationID);
            }

            if (filters.Status != null && filters.Status != StatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != StatusType.Nothing)
                    : items.Where(e => e.Status != StatusType.Nothing && e.Status != StatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            {
                case SiteOrderType.Description:
                    items = items.OrderBy(e => e.Description);
                    break;
                case SiteOrderType.IsMainSite:
                    items = items.OrderByDescending(e => e.IsMainSite)
                        .ThenBy(e => e.Description);
                    break;
                case SiteOrderType.Type:
                    items = items.OrderBy(e => e.Type)
                        .ThenBy(e => e.Description);
                    break;
                case SiteOrderType.DescriptionDesc:
                    items = items.OrderByDescending(e => e.Description);
                    break;
                case SiteOrderType.IsMainSiteDesc:
                    items = items.OrderBy(e => e.IsMainSite)
                        .ThenByDescending(e => e.Description);
                    break;
                case SiteOrderType.TypeDesc:
                    items = items.OrderByDescending(e => e.Type)
                        .ThenByDescending(e => e.Description);
                    break;
                default:
                    items = items.OrderByDescending(e => e.IsMainSite)
                        .ThenBy(e => e.Description);
                    break;
            }

            // Paging

            var pagedItems = PagedList<Site>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        /// <summary>
        /// Obtiene un listado de todos los Sites asociados a los ADCs
        /// de la propuesta, dado el ID de la propuesta.
        /// </summary>
        /// <param name="id">Identificador de la Propuesta</param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<List<Site>> GetsByProposalID(Guid id)
        {
            var proposalRepository = new ProposalRepository();
            var proposalItem = await proposalRepository.GetAsync(id)
                ?? throw new BusinessException("The Proposal item not found");

            var sites = proposalItem.ADCs != null
                ? proposalItem.ADCs.Where(adc => adc.ADCSites != null)
                    .SelectMany(adc => adc.ADCSites)
                    .Select(adcSite => adcSite.Site)
                    .Distinct()
                    .OrderByDescending(adc => adc.IsMainSite)
                    .ThenBy(adc => adc.Description)
                    .ToList()
                : new List<Site>();

            return sites;
        } // GetsByProposalID

        /// <summary>
        /// Obtiene los datos de un Site registrado en la base de datos
        /// de acuerdo con el identificador recibido
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Site> GetAsync(Guid id)
        { 
            return await _siteRepository.GetAsync(id);
        } // GetAsync

        /// <summary>
        /// Crea un registro temporal de un site con la información minima
        /// requerida
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<Site> AddAsync(Site item)
        {
            // Validations

            //if (string.IsNullOrEmpty(item.UpdatedUser)) // Debe de validar si existe el usuario
            //    throw new BusinessException("Updated user was not specified");

            if (item.OrganizationID == null || item.OrganizationID == Guid.Empty)
                throw new BusinessException("Must first assign Organization");

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries

            // Nota: Cuenta con eliminación en cascada para los Shifts
            await _siteRepository.DeleteTmpByUserAsync(item.UpdatedUser); 
            _siteRepository.Add(item);
            await _siteRepository.SaveChangesAsync();

            return item;
        } // AddAsync

        /// <summary>
        /// Actualiza la información de un Site, si es un registro nuevo (Status: 0), lo marca como
        /// activo, si es el sitio principal (true), marca el resto como secundarios (false)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<Site> UpdateAsync(Site item)
        { 
            // Validations

            var foundItem = await _siteRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (item.IsMainSite || item.Type == SiteType.Main)
            {
                await _siteRepository.SetToNotSiteMainAsync(foundItem.OrganizationID);
            }

            // Assigning values

            foundItem.Description = item.Description;
            foundItem.IsMainSite = item.Type == SiteType.Main; // item.IsMainSite;
            foundItem.Address = item.Address;
            foundItem.Country = item.Country;
            foundItem.Type = item.Type;
            //foundItem.LocationGPS = item.LocationGPS;
            foundItem.LocationURL = item.LocationURL;
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
                _siteRepository.Update(foundItem);
                await _siteRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"SiteService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Site item)
        {
            var foundItem = await _siteRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");


            if (foundItem.Status == StatusType.Deleted)
            {
                if (foundItem.Shifts.Any())
                    throw new BusinessException("The record want to delete, still has Shifts");

                _siteRepository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status == StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _siteRepository.Update(foundItem);
            }

            try
            { 
                _siteRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"SiteService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync
    }
}