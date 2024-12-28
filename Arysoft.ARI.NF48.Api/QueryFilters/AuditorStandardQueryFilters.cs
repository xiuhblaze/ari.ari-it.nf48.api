using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class AuditorStandardQueryFilters : BaseQueryFilters
    {
        public Guid? AuditorID { get; set; }

        public Guid? StandardID { get; set; }

        public string Text { get; set; }

        public StatusType? Status { get; set; }

        public AuditorStandardOrderType Order { get; set; }
    }
}