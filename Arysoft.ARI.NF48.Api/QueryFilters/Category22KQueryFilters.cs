using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class Category22KQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public Category22KAccreditedType? Accredited { get; set; }

        public StatusType? Status { get; set; }

        public Category22KOrderType Order { get; set; }
    }
}