using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class AuditQueryFilters : BaseQueryFilters
    {
        public Guid? OrganizationID { get; set; }

        public Guid? AuditCycleID { get; set; }

        public string Text { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public AuditStatusType? Status { get; set; }

        public AuditOrderType? Order { get; set; }
    }
}