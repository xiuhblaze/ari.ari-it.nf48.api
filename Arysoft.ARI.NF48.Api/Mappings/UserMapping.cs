using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class UserMapping
    {
        public static async Task<IEnumerable<UserListItemDto>> UsersToListDto(IEnumerable<User> items)
        {
            var itemsDto = new List<UserListItemDto>();

            foreach (var item in items)
            {
                itemsDto.Add(await UserToItemListDto(item));
            }

            return itemsDto;
        } // UsersRoListDto

        public static async Task<UserListItemDto> UserToItemListDto(User item)
        {
            var roles = item.Roles != null
                ? RoleMapping.RolesToListDto(item.Roles
                    .Where(r => r.Status != StatusType.Nothing
                        && r.Status != StatusType.Deleted))
                    .ToList()
                : null;

            return new UserListItemDto
            {
                ID = item.ID,                
                OwnerID = item.OwnerID,
                OwnerName = await GetOwnerNameAsync(item.OwnerID, item.Type ?? UserType.Nothing), // "", // Este se debe obtener donde se llame el Mapping
                Username = item.Username,
                Email = item.Email,
                FullName = Tools.Strings.FullName(item.FirstName, null, item.LastName),
                Type = item.Type ?? UserType.Nothing,
                LastAccess = item.LastAccess,
                LastPasswordChange = item.LastPasswordChange,
                Status = item.Status,                
                Roles = roles
            };
        } // UserToItemListDto

        public static async Task<UserDetailDto> UserToDetailDto(User item)
        {
            var roles = item.Roles != null
                ? RoleMapping.RolesToListDto(item.Roles
                    .Where(r => r.Status != StatusType.Nothing
                        && r.Status != StatusType.Deleted))
                    .ToList()
                : null;

            return new UserDetailDto
            {
                ID = item.ID,
                OwnerID = item.OwnerID,
                OwnerName = await GetOwnerNameAsync(item.OwnerID, item.Type ?? UserType.Nothing), // Este se debe obtener donde se llame el Mapping
                Username = item.Username,
                Email = item.Email,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Type = item.Type ?? UserType.Nothing,
                LastAccess = item.LastAccess,
                LastPasswordChange = item.LastPasswordChange,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Roles = roles
            };
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

        private static async Task<string> GetOwnerNameAsync(Guid? id, UserType type)
        {
            var ownerName = string.Empty;

            switch (type)
            {
                case UserType.SuperAdmin:
                    ownerName = "GOD";
                    break;

                case UserType.Admin:
                    ownerName = "ARI";
                    break;

                case UserType.Auditor:
                    if (id != null)
                    {
                        var audRepository = new AuditorRepository();
                        var auditor = await audRepository.GetAsync(id ?? Guid.Empty);

                        if (auditor != null)
                        {
                            ownerName = Tools.Strings.FullName(auditor.FirstName, auditor.MiddleName, auditor.LastName);
                        }
                    }
                    else ownerName = "ARI";
                    break;

                case UserType.Organization:
                    if (id != null)
                    {
                        var orgRepository = new OrganizationRepository();
                        var organization = await orgRepository.GetAsync(id ?? Guid.Empty);
                        if (organization != null)
                        {
                            ownerName = organization.Name;
                        }
                    }
                    else ownerName = "(organization)";
                    break;
                
                default:
                    ownerName = "(unknow)";
                    break;
            }

            return ownerName;
        } // GetOwnerNameAsync
    }
}