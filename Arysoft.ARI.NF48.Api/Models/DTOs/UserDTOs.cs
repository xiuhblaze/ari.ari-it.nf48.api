using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class UserListItemDto
    {
        public Guid ID { get; set; }

        public Guid? OwnerID { get; set; }

        public string OwnerName { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public UserType Type { get; set; }

        public DateTime? LastAccess { get; set; }

        public StatusType Status { get; set; }

        public ICollection<RoleItemListDto> Roles { get; set; }
    } // UserListItemDto

    public class UserDetailDto
    {
        public Guid ID { get; set; }

        public Guid? OwnerID { get; set; }

        public string OwnerName { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public UserType Type { get; set; }

        public DateTime? LastAccess { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public ICollection<RoleItemListDto> Roles { get; set; }
    } // UserDetailDto

    public class UserPostDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // UserPostDto

    public class UserPutDto
    {
        public Guid ID { get; set; }

        public Guid? OwnerID { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(64)]
        public string Password { get; set; } // INFO: Este solo recibe el password 

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [EnumDataType(typeof(UserType))]
        public UserType Type { get; set; }

        [EnumDataType(typeof(StatusType))]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // UserPutDto

    public class UserEditRoleDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public Guid RoleID { get; set; }
    } // UserAddRoleDto

    public class UserDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}