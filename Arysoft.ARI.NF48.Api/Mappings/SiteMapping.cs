using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
// using System.Data.Entity.Spatial;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class SiteMapping
    {
        public static IEnumerable<SiteItemListDto> SiteToListDto(IEnumerable<Site> items)
        { 
            var itemsDto = new List<SiteItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(SiteToItemListDto(item));
            }

            return itemsDto;
        } // SiteToListDto

        public static SiteItemListDto SiteToItemListDto(Site item)
        {
            return new SiteItemListDto
            {
                ID = item.ID,
                OrganizationName = item.Organization.Name,
                Description = item.Description,
                IsMainSite = item.IsMainSite,
                Address = item.Address,
                Country = item.Country,
                LocationURL = item.LocationURL,
                Status = item.Status,
                ShiftsCount = item.Shifts != null 
                    ? item.Shifts
                        .Where(i => i.Status != StatusType.Nothing)
                        .Count() 
                    : 0,
                EmployeesCount = item.Shifts != null 
                    ? item.Shifts
                        .Where(i => i.Status == StatusType.Active)
                        .Sum(i => i.NoEmployees) ?? 0 
                    : 0,
                Shifts = item.Shifts != null
                    ? ShiftMapping.ShiftsToListDto(item.Shifts
                        .Where(i => i.Status != StatusType.Nothing))
                    : new List<ShiftItemListDto>(),
            };
        } // SiteToItemListDto

        public static async Task<SiteItemDetailDto> SiteToItemDetailDto(Site item)
        {
            return new SiteItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                Description = item.Description,
                IsMainSite = item.IsMainSite,
                Address = item.Address,
                Country = item.Country,
                LocationURL = item.LocationURL,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Shifts = item.Shifts != null 
                    ? ShiftMapping.ShiftsToListDto(item.Shifts
                        .Where(i => i.Status != StatusType.Nothing))
                    : new List<ShiftItemListDto>(),
                Organization = item.Organization != null
                    ? await OrganizationMapping.OrganizationToItemListDto(item.Organization)
                    : null
            };
        } // SiteToItemDetailDto

        public static Site ItemAddDtoToSite(SitePostDto itemDto)
        {
            return new Site
            {
                OrganizationID = itemDto.OrganizationID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToSite

        public static Site ItemEditDtoToSite(SitePutDto itemDto)
        {
            var item = new Site
            { 
                ID = itemDto.ID,
                Description = itemDto.Description,
                IsMainSite = itemDto.IsMainSite,
                Address = itemDto.Address,
                Country = itemDto.Country,
                LocationURL = itemDto.LocationURL,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };

            //if (itemDto.LocationLat != null && itemDto.LocationLong != null)
            //{
            //    item.LocationGPS = DbGeography.FromText($"POINT({itemDto.LocationLong} {itemDto.LocationLat})");
            //}

            return item;
        } // ItemEditDtoToSite

        public static Site ItemDeleteDtoToSite(SiteDeleteDto itemDto)
        {
            return new Site
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToSite


    }
}