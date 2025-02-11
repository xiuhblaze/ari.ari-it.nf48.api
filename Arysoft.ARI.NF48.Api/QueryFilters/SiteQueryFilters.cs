using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class SiteQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public Guid? OrganizationID { get; set; }

        public StatusType? Status { get; set; }

        public SiteOrderType? Order { get; set; }
    }
}