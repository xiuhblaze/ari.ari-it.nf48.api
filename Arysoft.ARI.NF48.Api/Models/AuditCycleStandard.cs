using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AuditCycleStandard : BaseModel
    {
        public Guid AuditCycleID { get; set; }

        public Guid? StandardID { get; set; }

        public AuditStepType? InitialStep { get; set; }

        public AuditCycleType? CycleType { get; set; }

        // RELATIONS

        public virtual AuditCycle AuditCycle { get; set; }

        public virtual Standard Standard { get; set; }
    } // AuditCycleStandard
}