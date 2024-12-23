using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class NSSCCategoryQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public StatusType? Status { get; set; }

        public NSSCCategoryOrderType? Order { get; set; }
    }
}