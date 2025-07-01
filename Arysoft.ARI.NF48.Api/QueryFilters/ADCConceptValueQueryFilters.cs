using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class ADCConceptValueQueryFilters : BaseQueryFilters
    {
        public Guid? ADCConceptID { get; set; }

        public Guid? ADCSiteID { get; set; }

        public string Text { get; set; }

        public StatusType? Status { get; set; }

        public ADCConceptValueOrderType? Order { get; set; }
    }
}