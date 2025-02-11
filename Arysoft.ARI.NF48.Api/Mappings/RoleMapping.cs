using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class RoleMapping
    {
        public static IEnumerable<RoleItemListDto> RolesToListDto(IEnumerable<Role> items)
        {
            var itemsDto = new List<RoleItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(RoleToItemListDto(item));
            }

            return itemsDto;
        } // RolesToListDto

        public static RoleItemListDto RoleToItemListDto(Role item)
        {
            return new RoleItemListDto
            {
                ID = item.ID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status
            };
        } // RoleToItemListDto

        public static RoleItemDetailDto RoleToItemDetailDto(Role item)
        {
            var itemDto = new RoleItemDetailDto
            {
                ID = item.ID,
                Name = item.Name,
                Description = item.Description,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Status = item.Status
            };

            // HACK: Agregar los usuarios asociados al Rol

            return itemDto;
        } // RoleToItemDetailDto

        public static Role ItemEditDtoToRole(RolePutDto itemDto)
        {
            return new Role {
                ID = itemDto.ID,
                Name = itemDto.Name,
                Description = itemDto.Description,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToRole

        public static Role ItemDeleteDtoToRole(RoleDeleteDto itemDto)
        {
            return new Role
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToRole
    }
}