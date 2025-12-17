using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class AuditCycleQueryFilters : BaseQueryFilters
    {   
        public Guid? OrganizationID { get; set; }

        public AuditCycleType? CycleType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public StatusType? Status { get; set; }

        public AuditCycleOrderType? Order { get; set; }
    }
}