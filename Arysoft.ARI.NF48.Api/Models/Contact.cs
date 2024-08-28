using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Contact : BaseModel
    {
        public Guid OrganizationID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string PhoneAlt { get; set; }

        public string LocationDescription { get; set; }

        public string Position { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }
    }
}