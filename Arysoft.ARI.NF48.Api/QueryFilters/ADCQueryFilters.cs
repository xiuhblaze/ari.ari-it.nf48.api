using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class ADCQueryFilters : BaseQueryFilters
    {
        public Guid? AppFormID { get; set; }

        public ADCStatusType? Status { get; set; }

        public ADCOrderType? Order { get; set; }
    }
}