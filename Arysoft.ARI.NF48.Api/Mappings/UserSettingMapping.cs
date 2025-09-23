using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class UserSettingMapping
    {
        public static IEnumerable<UserSettingItemDto> UserSettingToListDto(IEnumerable<UserSetting> items)
        { 
            var itemsDto = new List<UserSettingItemDto>();

            foreach (var item in items)
            {
                itemsDto.Add(UserSettingToItemDto(item));
            }

            return itemsDto;
        } // UserSettingToListDto

        public static UserSettingItemDto UserSettingToItemDto(UserSetting item)
        {   
            var itemDto = new UserSettingItemDto
            {
                ID = item.ID,
                UserID = item.UserID,
                Settings = item.Settings,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser
            };

            return itemDto;
        } // UserSettingToItemDto

        public static UserSetting ItemCreateDtoToUserSetting(UserSettingCreateDto itemCreateDto)
        {
            var item = new UserSetting
            {
                UserID = itemCreateDto.UserID,
                Settings = itemCreateDto.Settings,
                UpdatedUser = itemCreateDto.UpdatedUser
            };

            return item;
        } // ItemCreateDtoToUserSetting

        public static UserSetting ItemUpdateDtoToUserSetting(UserSettingUpdateDto itemUpdateDto)
        {
            var item = new UserSetting
            {
                ID = itemUpdateDto.ID,
                Settings = itemUpdateDto.Settings,
                Status = itemUpdateDto.Status,
                UpdatedUser = itemUpdateDto.UpdatedUser
            };

            return item;
        } // ItemUpdateDtoToUserSetting

        public static UserSetting ItemDeleteDtoToUserSetting(UserSettingDeleteDto itemDeleteDto)
        {
            var item = new UserSetting
            {
                ID = itemDeleteDto.ID,
                UpdatedUser = itemDeleteDto.UpdatedUser
            };

            return item;
        } // ItemDeleteDtoToUserSetting
    }
}