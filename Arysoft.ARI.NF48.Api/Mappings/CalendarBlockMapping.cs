using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class CalendarBlockMapping
    {
        public static async Task<IEnumerable<CalendarBlockItemListDto>> CalendarBlockToListAsync(IEnumerable<CalendarBlock> items)
        {
            var itemsDto = new List<CalendarBlockItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(await CalendarBlockToItemListDtoAsync(item));
            }

            return itemsDto;
        } // CalendarBlockToListAsync

        public static async Task<CalendarBlockItemListDto> CalendarBlockToItemListDtoAsync(CalendarBlock item)
        {
            var _auditorRepository = new AuditorRepository();
            var _userRepository = new UserRepository();
            string ownerName = string.Empty;

            switch (item.OwnerType)
            {
                case CalendarBlockOwnerType.Auditor:
                    if (item.OwnerID.HasValue)
                        ownerName = await _auditorRepository.GetFullNameByIDAsync(item.OwnerID.Value);
                    break;
                case CalendarBlockOwnerType.User:
                    if (item.OwnerID.HasValue)
                        ownerName = await _userRepository.GetFullNameByIDAsync(item.OwnerID.Value);
                    break;
            }

            return new CalendarBlockItemListDto
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

        public static CalendarBlockItemDetailDto CalendarBlockToItemDetailDto(CalendarBlock item)
        {
            return new CalendarBlockItemDetailDto
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

        public static CalendarBlock ItemCreateDtoToCalendarBlock(CalendarBlockCreateDto itemDto)
        {
            return new CalendarBlock
            {
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemCreateDtoToCalendarBlock

        public static CalendarBlock ItemUpdateDtoToCalendarBlock(CalendarBlockUpdateDto itemDto)
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

        public static CalendarBlock ItemDeleteDtoToCalendarBlock(CalendarBlockDeleteDto itemDto)
        {
            return new CalendarBlock
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToCalendarBlock
    }
}