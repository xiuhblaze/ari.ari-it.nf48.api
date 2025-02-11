using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class ShiftMapping
    {
        public static IEnumerable<ShiftItemListDto> ShiftsToListDto(IEnumerable<Shift> items)
        {
            var itemsDto = new List<ShiftItemListDto>();

            foreach(var item in items)
            {
                itemsDto.Add(ShiftToItemListDto(item));
            }

            return itemsDto;
        } // ShiftsToListDto

        public static ShiftItemListDto ShiftToItemListDto(Shift item)
        {
            return new ShiftItemListDto
            {
                ID = item.ID,
                SiteDescription = item.Site != null 
                    ? item.Site.Description
                    : string.Empty,
                Type = item.Type,
                NoEmployees = item.NoEmployees,
                ActivitiesDescription = item.ActivitiesDescription,
                ShiftStart = item.ShiftStart,
                ShiftEnd = item.ShiftEnd,
                ShiftStart2 = item.ShiftStart2,
                ShiftEnd2 = item.ShiftEnd2,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status
            };
        } // ShiftToItemListDto

        public static ShiftItemDetailDto ShiftToItemDetailDto(Shift item)
        {
            var itemDto = new ShiftItemDetailDto
            {
                ID = item.ID,
                SiteID = item.SiteID,
                Type = item.Type,
                NoEmployees = item.NoEmployees,
                ActivitiesDescription = item.ActivitiesDescription,
                ShiftStart = item.ShiftStart,
                ShiftEnd = item.ShiftEnd,
                ShiftStart2 = item.ShiftStart2,
                ShiftEnd2 = item.ShiftEnd2,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,

                Site = item.Site != null
                    ? SiteMapping.SiteToItemListDto(item.Site)
                    : null
            };

            return itemDto;
        } // ShiftToItemDetailDto

        public static Shift ItemAddDtoToShift(ShiftPostDto itemDto)
        {
            return new Shift
            {
                SiteID = itemDto.SiteID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToShift

        public static Shift ItemEditDtoToShift(ShiftPutDto itemDto)
        {
            return new Shift
            {
                ID = itemDto.ID,
                Type = itemDto.Type,
                NoEmployees = itemDto.NoEmployees,
                ActivitiesDescription = itemDto.ActivitiesDescription,
                ShiftStart = itemDto.ShiftStart,
                ShiftEnd = itemDto.ShiftEnd,
                ShiftStart2 = itemDto.ShiftStart2,
                ShiftEnd2 = itemDto.ShiftEnd2,
                ExtraInfo = itemDto.ExtraInfo,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToShift

        public static Shift ItemDeleteDtoToShift(ShiftDeleteDto itemDto)
        {
            return new Shift
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToShift
    }
}