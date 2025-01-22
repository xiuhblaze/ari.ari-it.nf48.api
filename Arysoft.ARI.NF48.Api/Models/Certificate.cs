using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Certificate : BaseModel
    {
        public Guid OrganizationID { get; set; }

        public Guid? StandardID { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string Comments { get; set; }

        public string Filename { get; set; }

        public DateTime? PrevAuditDate { get; set; }

        public string PrevAuditNote { get; set; }

        public DateTime? NextAuditDate { get; set; }

        public string NextAuditNote { get; set; }

        public new CertificateStatusType Status { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual Standard Standard { get; set; }

        // NOT MAPPED

        public CertificateValidityStatusType ValidityStatus { get; set; }

    } // Certificate
}