using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class DayCalculationConceptQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public Guid? StandardID { get; set; }

        public DayCalculationConceptUnitType? Unit { get; set; }

        public StatusType? Status { get; set; }

        public DayCalculationConceptOrderType? Order { get; set; }
    }
}