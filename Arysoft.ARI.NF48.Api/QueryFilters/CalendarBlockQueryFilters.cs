using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class CalendarBlockQueryFilters : BaseQueryFilters
    {
        public Guid? OwnerID { get; set; }

        public CalendarBlockType? BlockType { get; set; }

        public string Text { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public StatusType? Status { get; set; }

        public CalendarBlockOrderType? Order { get; set; }
    } // CalendarBlockQueryFilters
}