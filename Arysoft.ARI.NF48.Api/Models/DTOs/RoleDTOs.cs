using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class RoleItemListDto
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }
    } // RoleItemListDto

    public class RoleItemDetailDto
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public ICollection<User> Users { get; set; }
    } // RoleItemDetailDto

    public class RolePostDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // RolePostDto

    public class RolePutDto
    {
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [EnumDataType(typeof(StatusType))]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // RolePutDto

    public class RoleDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
} 