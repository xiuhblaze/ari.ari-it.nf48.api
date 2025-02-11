using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

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

        public string QRFile { get; set; }

        public string CRN { get; set; } // Certificate Registration Number

        public DateTime? PrevAuditDate { get; set; }

        public string PrevAuditNote { get; set; }

        public DateTime? NextAuditDate { get; set; }

        public string NextAuditNote { get; set; }

        public bool? HasNCsMinor { get; set; }

        public bool? HasNCsMajor { get; set; }

        public bool? HasNCsCritical { get; set; }

        public DateTime? ActionPlanDate { get; set; }

        public bool? ActionPlanDelivered { get; set; }

        public new CertificateStatusType Status { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual Standard Standard { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        // NOT MAPPED

        public CertificateValidityStatusType ValidityStatus { get; set; }

        public DefaultValidityStatusType AuditPlanValidityStatus { get; set; }

    } // Certificate
}