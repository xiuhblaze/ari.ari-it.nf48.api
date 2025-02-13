using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Audit : BaseModel
    {
        public Guid AuditCycleID { get; set; }

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? HasWitness { get; set; }

        public new AuditStatusType Status { get; set; }

        // RELATIONS

        public virtual AuditCycle AuditCycle { get; set; }

        public virtual ICollection<AuditAuditor> AuditAuditors { get; set; }

        public virtual ICollection<AuditStandard> AuditStandards { get; set; }

        public virtual ICollection<AuditDocument> AuditDocuments { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
    } // Audit
}