using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Linq;
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
                    items = items.OrderBy(e => e.IsMainSite)
                        .ThenBy(e => e.Description);
                    break;
                case SiteOrderType.Status:
                    items = items.OrderBy(e => e.Status);
                    break;
                case SiteOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case SiteOrderType.DescriptionDesc:
                    items = items.OrderByDescending(e => e.Description);
                    break;
                case SiteOrderType.IsMainSiteDesc:
                    items = items.OrderByDescending(e => e.IsMainSite)
                        .ThenByDescending(e => e.Description);
                    break;
                case SiteOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status);
                    break;
                case SiteOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.IsMainSite)
                        .ThenBy(e => e.Description);
                    break;
            }

            // Paging

            var pagedItems = PagedList<Site>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Site> GetAsync(Guid id)
        { 
            return await _siteRepository.GetAsync(id);
        } // GetAsync

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

            // TODO: Ver si se generaron Shifts para este site, si es asi, borrarlos primero
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

            if (item.IsMainSite)
            {
                await _siteRepository.SetToNotSiteMainAsync(foundItem.OrganizationID);
            }

            // Assigning values

            foundItem.Description = item.Description;
            foundItem.IsMainSite = item.IsMainSite;
            foundItem.Address = item.Address;
            foundItem.LocationGPS = item.LocationGPS;
            foundItem.LocationURL = item.LocationURL;
            foundItem.Status = foundItem.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;
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
                throw new BusinessException(ex.Message);
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
            _siteRepository.SaveChanges();
        } // DeleteAsync

//        Y para hacer consultas filtrando por ese campo, que opciones hay?
//Para realizar consultas filtrando por datos geoespaciales(GEOGRAPHY) en SQL Server a través de Entity Framework, tienes varias opciones útiles.Estas son algunas de las más comunes:

//Filtro de proximidad(distancia) : Puedes realizar consultas para encontrar ubicaciones dentro de un cierto radio de un punto específico utilizando el método Distance.

//csharp
//using (var contexto = new MiContexto())
//{
//    var latitud = 40.7128;
//    var longitud = -74.0060;
//    var puntoReferencia = DbGeography.FromText($"POINT({longitud} {latitud})");
//    var radioEnMetros = 1000;  // 1 km de radio

//    var lugaresCercanos = contexto.Lugares
//        .Where(l => l.Ubicacion.Distance(puntoReferencia) <= radioEnMetros)
//        .ToList();

//    foreach (var lugar in lugaresCercanos)
//    {
//        Console.WriteLine($"{lugar.Nombre}: {lugar.Ubicacion.Latitude}, {lugar.Ubicacion.Longitude}");
//    }
//}
//Filtro por intersección de áreas: Puedes buscar ubicaciones que caigan dentro de una área geográfica específica utilizando el método Intersects.

//csharp
//using (var contexto = new MiContexto())
//{
//    var poligono = DbGeography.FromText("POLYGON((-74.0060 40.7128, -74.0010 40.7128, -74.0010 40.7188, -74.0060 40.7188, -74.0060 40.7128))");

//    var lugaresDentroDelArea = contexto.Lugares
//        .Where(l => poligono.Intersects(l.Ubicacion))
//        .ToList();

//    foreach (var lugar in lugaresDentroDelArea)
//    {
//        Console.WriteLine($"{lugar.Nombre}: {lugar.Ubicacion.Latitude}, {lugar.Ubicacion.Longitude}");
//    }
//}
//Consultas de ordenamiento por distancia: Puedes ordenar los resultados de tu consulta por distancia a un punto específico.

//csharp
//using (var contexto = new MiContexto())
//{
//    var latitud = 40.7128;
//    var longitud = -74.0060;
//    var puntoReferencia = DbGeography.FromText($"POINT({longitud} {latitud})");

//    var lugaresOrdenadosPorDistancia = contexto.Lugares
//        .OrderBy(l => l.Ubicacion.Distance(puntoReferencia))
//        .ToList();

//    foreach (var lugar in lugaresOrdenadosPorDistancia)
//    {
//        Console.WriteLine($"{lugar.Nombre}: {lugar.Ubicacion.Latitude}, {lugar.Ubicacion.Longitude}");
//    }
//}
//Estas son algunas formas comunes de filtrar y ordenar tus datos geoespaciales. Dependiendo de tus necesidades específicas, puedes ajustar estos ejemplos para realizar consultas más avanzadas y personalizadas.
    }
}