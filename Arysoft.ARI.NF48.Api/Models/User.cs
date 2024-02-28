using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class User : BaseModel
    {
        public Guid? OrganizationID { get; set; }

        public Guid? ContactID { get; set; }
                
        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}