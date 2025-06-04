using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class User : BaseModel
    {
        public Guid? OwnerID { get; set; } // Organization, Auditor, ARI User

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public UserType? Type { get; set; }

        public DateTime? LastAccess { get; set; }

        public DateTime? LastPasswordChange { get; set; }

        // RELATIONS

        public virtual ICollection<Role> Roles { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
    }
}