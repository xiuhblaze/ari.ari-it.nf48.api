using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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