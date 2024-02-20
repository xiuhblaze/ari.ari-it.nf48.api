using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models
{
    [Table("Users")]
    public class User : BaseModel
    {
        public Guid? OrganizationID { get; set; }

        public Guid? ContactID { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(64)]
        public string PasswordHash { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}