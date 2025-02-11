using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class DayCalculationConceptApplicationQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public Guid? DayCalculationConceptID { get; set; }

        public Guid? ApplicationID { get; set; }

        public DayCalculationConceptUnitType? Unit { get; set; }

        public StatusType? Status { get; set; }

        public DayCalculationConceptApplicationOrderType? Order { get; set; }
    }
}