using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class RiskLevelQueryFilters : BaseQueryFilters
    {
        public Guid? StandardID { get; set; }

        public RiskLevelCategory? Category { get; set; }

        public string Text { get; set; }

        public StatusType? Status { get; set; }

        public RiskLevelOrderType? Order { get; set; }
    }
}