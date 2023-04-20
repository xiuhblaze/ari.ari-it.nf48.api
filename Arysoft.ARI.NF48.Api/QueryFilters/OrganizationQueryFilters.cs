using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class OrganizationQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public OrganizationStatusType? Status { get; set; }

        public OrganizationOrderType Order { get; set; }
    }
}