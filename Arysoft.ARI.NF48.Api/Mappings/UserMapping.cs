using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class UserMapping
    {
        public static IEnumerable<UserListItemDto> UsersToListDto(IEnumerable<User> items)
        {
            var itemsDto = new List<UserListItemDto>();

            foreach (var item in items)
            {
                itemsDto.Add(UserToItemListDto(item));
            }

            return itemsDto;
        } // UsersRoListDto

        public static UserListItemDto UserToItemListDto(User item)
        {
            return new UserListItemDto
            {
                ID = item.ID,                
                OwnerID = item.OwnerID,
                OwnerName = "", // Este se debe obtener donde se llame el Mapping
                Username = item.Username,
                Email = item.Email,
                FullName = Tools.Strings.FullName(item.FirstName, null, item.LastName),
                Type = item.Type,
                Status = item.Status,                
                Roles = item.Roles != null
                    ? RoleMapping.RolesToListDto(item.Roles).ToList()
                    : null
            };
        } // UserToItemListDto

        public static UserDetailDto UserToDetailDto(User item)
        {
            var itemDto = new UserDetailDto
            {
                ID = item.ID,
                OwnerID = item.OwnerID,
                OwnerName = "", // Este se debe obtener donde se llame el Mapping
                Username = item.Username,
                Email = item.Email,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Type = item.Type,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Roles = item.Roles != null
                    ? RoleMapping.RolesToListDto(item.Roles).ToList()
                    : null
            };

            return itemDto;
        } // UserToDetailDto

        public static User ItemEditDtoToUser(UserPutDto itemDto)
        {
            var item = new User()
            {
                ID = itemDto.ID,
                OwnerID = itemDto.OwnerID,                
                Username = itemDto.Username,
                PasswordHash = itemDto.Password,
                Email = itemDto.Email,
                FirstName = itemDto.FirstName,
                LastName = itemDto.LastName,
                Type = itemDto.Type,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };

            return item;
        } // ItemEditDtoToUser

        public static User ItemDeleteDtoToUser(UserDeleteDto itemDto)
        {
            var item = new User()
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };

            return item;
        } // ItemDeleteDtoToUser
    }
}