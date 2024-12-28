using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class FSSCActivityQueryFilters : BaseQueryFilters
    {
        public Guid? FSSCSubCategoryID { get; set; }

        public string Text { get; set; }

        public StatusType? Status { get; set; }

        public FSSCActivityOrderType? Order { get; set; }
    }
}