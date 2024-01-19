using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arysoft.ARI.NF48.Api.Models
{
    [Table("Roles")]
    public class Role : BaseModel
    {
        [Key]
        public Guid RoleID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public ICollection<User> Users { get; set; }
    }
}