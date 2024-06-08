using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                LocationDescription = item.LocationDescription,
                Order = item.Order,
                Status = item.Status,
                NoShifts = item.Shifts != null ? item.Shifts.Count() : 0,
                NoEmployees = item.Shifts != null ? item.Shifts.Sum(s => s.NoEmployees) ?? 0 : 0,
            };
        } // SiteToItemListDto

        public static SiteItemDetailDto SiteToItemDetailDto(Site item)
        {
            return new SiteItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                Description = item.Description,
                LocationDescription = item.LocationDescription,
                Order = item.Order,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,

                Shifts = item.Shifts != null 
                    ? ShiftMapping.ShiftsToListDto(item.Shifts) 
                    : new List<ShiftItemListDto>(),
                Organization = item.Organization != null
                    ? OrganizationMapping.OrganizationToItemListDto(item.Organization) 
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
            return new Site
            { 
                ID = itemDto.ID,
                Description = itemDto.Description,
                LocationDescription = itemDto.LocationDescription,
                Order = itemDto.Order,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
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