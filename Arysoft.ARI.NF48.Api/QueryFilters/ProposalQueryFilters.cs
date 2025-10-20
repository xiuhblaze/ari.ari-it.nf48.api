using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class ProposalQueryFilters : BaseQueryFilters
    {
        public Guid? AuditCycleID { get; set; }

        public Guid? OrganizationID { get; set; }

        public string Text { get; set; }

        public ProposalStatusType? Status { get; set; }

        public ProposalOrderType? Order { get; set; }
    }
}