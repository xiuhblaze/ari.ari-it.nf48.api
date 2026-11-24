using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public static class CalendarBlockMapper
    {
        //public static async Task<IEnumerable<CalendarBlockListDto>> CalendarBlockToListAsync(IEnumerable<CalendarBlock> items)
        //{
        //    var itemsDto = new List<CalendarBlockListDto>();

        //    foreach (var item in items)
        //    {
        //        itemsDto.Add(await CalendarBlockToItemListDtoAsync(item));
        //    }

        //    return itemsDto;
        //} // CalendarBlockToListAsync
        // usar: calendarBlock.Select(x => x.ToListDto())

        public static CalendarBlockListDto ToListDto(this CalendarBlock item, string ownerName)
        {   
            return new CalendarBlockListDto
            {
                ID = item.ID,
                OwnerID = item.OwnerID,
                BlockType = item.BlockType,
                Description = item.Description,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                OwnerType = item.OwnerType,
                AllowOverride = item.AllowOverride,
                Status = item.Status,

                OwnerName = ownerName,
            };
        } // CalendarBlockToItemListDtoAsync

        public static CalendarBlockDetailDto ToDetailDto(this CalendarBlock item)
        {
            return new CalendarBlockDetailDto
            {
                ID = item.ID,
                OwnerID = item.OwnerID,
                BlockType = item.BlockType,
                Description = item.Description,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                OwnerType = item.OwnerType,
                AllowOverride = item.AllowOverride,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser
            };
        } // CalendarBlockToItemDetailDto

        public static CalendarBlock ToCalendarBlock(this CalendarBlockCreateDto itemDto)
        {
            return new CalendarBlock
            {
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemCreateDtoToCalendarBlock

        public static CalendarBlock ToCalendarBlock(this CalendarBlockUpdateDto itemDto)
        {   
            return new CalendarBlock
            {
                ID = itemDto.ID,
                OwnerID = itemDto.OwnerID,
                BlockType = itemDto.BlockType,
                Description = itemDto.Description,
                StartDate = itemDto.StartDate,
                EndDate = itemDto.EndDate,
                OwnerType = itemDto.OwnerType,
                AllowOverride= itemDto.AllowOverride,
                ExtraInfo = itemDto.ExtraInfo,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemUpdateDtoToCalendarBlock

        public static CalendarBlock ToCalendarBlock(this CalendarBlockDeleteDto itemDto)
        {
            return new CalendarBlock
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToCalendarBlock
    }
}