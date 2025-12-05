using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Audit : BaseModel
    {
        public Guid? OrganizationID { get; set; }

        public Guid AuditCycleID { get; set; }  // #CHANGE_CYCLES: Por eliminar

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsMultisite { get; set; }

        public string Days { get; set; }

        public bool? IncludeSaturday { get; set; }

        public bool? IncludeSunday { get; set; }

        public string ExtraInfo { get; set; }

        public new AuditStatusType Status { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual AuditCycle AuditCycle { get; set; } // #CHANGE_CYCLES: Por eliminar

        public virtual ICollection<AuditDocument> AuditDocuments { get; set; }

        public virtual ICollection<AuditAuditor> AuditAuditors { get; set; }

        public virtual ICollection<Site> Sites { get; set; }

        public virtual ICollection<AuditStandard> AuditStandards { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
    } // Audit
}