using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class UserMapping
    {
        public static IEnumerable<UserListItemDto> UsersToListDto(IEnumerable<User> items)
        {
            var itemsDto = new List<UserListItemDto>();

            foreach (var item in items)
            {
                var itemDto = new UserListItemDto
                {
                    ID = item.ID,
                    OrganizationID = item.OrganizationID,
                    ContactID = item.ContactID,
                    Username = item.Username,
                    Email = item.Email,
                    FullName = item.FirstName + (!string.IsNullOrEmpty(item.LastName) ? " " + item.LastName : string.Empty),
                    Status = item.Status,
                    OrganizationName = item.Organization.Name,
                    ContactName = item.Contact.FirstName + (!string.IsNullOrEmpty(item.Contact.LastName) ? " " + item.Contact.LastName : string.Empty)
                    // Roles = RolesToListDto(item.Roles)
                };

                itemsDto.Add(itemDto);
            }

            return itemsDto;
        } // UsersRoListDto

        public static UserDetailDto UserToDetailDto(User item)
        {
            var itemDto = new UserDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                ContactID = item.ContactID,
                Username = item.Username,
                Email = item.Email,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                OrganizationName = item.Organization.Name,
                ContactName = item.Contact.FirstName + (!string.IsNullOrEmpty(item.Contact.LastName) 
                    ? " " + item.Contact.LastName 
                    : string.Empty)
                // Roles = RolesToListDto(item.Roles)
            };

            return itemDto;
        } // UserToDetailDto

        public static User ItemEditDtoToUser(UserPutDto itemDto)
        {
            var item = new User()
            {
                ID = itemDto.ID,
                OrganizationID = itemDto.OrganizationID,
                ContactID = itemDto.ContactID,
                Username = itemDto.Username,
                PasswordHash = itemDto.PasswordHash,
                Email = itemDto.Email,
                FirstName = itemDto.FirstName,
                LastName = itemDto.LastName,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };

            return item;
        } // ItemEditDtoToUser


    }
}