using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class FSSCJobExperienceQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public StatusType? Status { get; set; }

        public FSSCJobExperienceOrderType? Order { get; set; }
    }
}