using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class ADCSiteAuditQueryFilters : BaseQueryFilters
    {
        public Guid? ADCSiteID { get; set; }

        public StatusType? Status { get; set; }

        public ADCSiteAuditOrderType? Order { get; set; }
    }
}