using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AuditCycle : BaseModel
    {
        public Guid OrganizationID { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public AuditCyclePeriodicityType? Periodicity { get; set; }

        public string ExtraInfo { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual ICollection<Audit> Audits { get; set; }

        public virtual ICollection<AuditCycleStandard> AuditCycleStandards { get; set; }

        public virtual ICollection<AuditCycleDocument> AuditCycleDocuments { get; set; }
    }
}