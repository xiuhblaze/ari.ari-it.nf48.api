using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class AuditStandardQueryFilters : BaseQueryFilters
    {
        public Guid? AuditID { get; set; }

        public Guid? StandardID { get; set; }

        public AuditStepType? Step { get; set; }

        public StatusType? Status { get; set; }

        public AuditStandardOrderType? Order { get; set; }
    } // AuditStandardQueryFilters
}