using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class User : BaseModel
    {
        public Guid? OwnerID { get; set; } // Organization, Auditor

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public UserType Type { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual ICollection<Role> Roles { get; set; }
    }
}