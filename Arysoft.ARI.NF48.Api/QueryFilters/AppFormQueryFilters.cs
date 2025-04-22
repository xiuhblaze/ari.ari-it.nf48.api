using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class AppFormQueryFilters : BaseQueryFilters
    {
        public Guid? OrganizationID { get; set; }

        public Guid? AuditCycleID { get; set; }

        public Guid? StandardID { get; set; }

        public string Text { get; set; }

        public AppFormStatusType? Status { get; set; }

        public AppFormOrderType? Order { get; set; }
    }
}