using System;
using System.Collections;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AuditCycle : BaseModel
    {
        public Guid OrganizationID { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        // public virtual ICollection<Audit> Audits { get; set; }

        public virtual ICollection<AuditCycleStandard> Standards { get; set; }

        // public virtual ICollection<AuditCycleDocument> Documents { get; set; }
    }
}