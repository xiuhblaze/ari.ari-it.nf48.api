using Arysoft.ARI.NF48.Api.Enumerations;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Organization : BaseModel
    {   
        public string Name { get; set; }

        public string LegalEntity { get; set; }

        public string LogoFile { get; set; }

        public string QRFile { get; set; }

        public string Website { get; set; }

        public string Phone { get; set; }

        public string COID { get; set; }

        public new OrganizationStatusType Status { get; set; } //TODO: Esto lo tengo en duda

        // RELATIONS

        public virtual ICollection<Application> Applications { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }

        public virtual ICollection<Site> Sites { get; set; }

        public virtual ICollection<AuditCycle> AuditCycles { get; set; }
    }
}