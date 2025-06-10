using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class ADCSiteQueryFilters : BaseQueryFilters
    {
        public Guid? ADCID { get; set; }

        public Guid? SiteID { get; set; }

        public StatusType? Status { get; set; }

        public ADCSiteOrderType? Order { get; set; }
    }
}