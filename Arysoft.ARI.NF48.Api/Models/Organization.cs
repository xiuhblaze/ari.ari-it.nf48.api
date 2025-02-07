using Arysoft.ARI.NF48.Api.Enumerations;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Organization : BaseModel
    {
        public int? Folio { get; set; }

        public string Name { get; set; }

        public string LogoFile { get; set; }

        public string Website { get; set; }

        public string Phone { get; set; }

        public string ExtraInfo { get; set; }

        public new OrganizationStatusType Status { get; set; }

        // RELATIONS

        public virtual ICollection<Application> Applications { get; set; }

        // public virtual ICollection<AuditCycle> AuditCycles { get; set; }

        public virtual ICollection<Certificate> Certificates { get; set; }

        public virtual ICollection<Company> Companies { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }

        public virtual ICollection<Site> Sites { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        public virtual ICollection<OrganizationStandard> OrganizationStandards { get; set; }

        // NOT MAPPED

        public CertificateValidityStatusType? CertificatesValidityStatus { get; set; }

    } // Organization
}