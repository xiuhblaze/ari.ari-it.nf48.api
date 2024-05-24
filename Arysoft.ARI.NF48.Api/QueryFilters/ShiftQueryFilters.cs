using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class ShiftQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public Guid? SiteID { get; set; }

        //public DateTime? ShiftStart { get; set; }

        //public datetime? ShiftFinish { get; set; }

        public ShiftType? Type { get; set; }

        public StatusType? Status { get; set; }

        public ShiftOrderType Order { get; set; }
    }
}