using Arysoft.ARI.NF48.Api.Enumerations;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Organization : BaseModel
    {   
        public string Name { get; set; }

        public string LegalEntity { get; set; }

        public string LogoFile { get; set; }

        public string Website { get; set; }

        public string Phone { get; set; }

        public OrganizationStatusType Status { get; set; }

        // RELATIONS

        public virtual ICollection<Application> Applications { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }

        public virtual ICollection<Site> Sites { get; set; }

    }
}