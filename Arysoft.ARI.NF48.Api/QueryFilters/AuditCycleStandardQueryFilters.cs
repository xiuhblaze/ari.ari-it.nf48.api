using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class AuditCycleStandardQueryFilters : BaseQueryFilters
    {
        public Guid? AuditCycleID { get; set; }

        public Guid? StandardID { get; set; }

        public AuditStepType? InitialStep { get; set; }

        public AuditCycleType? CycleType { get; set; }

        public StatusType? Status { get; set; }

        public AuditCycleStandardOrderType? Order { get; set; }
    }
}