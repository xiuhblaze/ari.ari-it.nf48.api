using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class StandardQueryFilters : BaseQueryFilters
    {
        public string Texto { get; set; }

        public StatusType? Status { get; set; }

        public StandardsOrderType Order { get; set; }
    }
}