using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class ProposalAuditQueryFilters : BaseQueryFilters
    {
        public Guid? ProposalID { get; set; }

        public StatusType? Status { get; set; }

        public ProposalAuditOrderType? Order { get; set; }
    }
}