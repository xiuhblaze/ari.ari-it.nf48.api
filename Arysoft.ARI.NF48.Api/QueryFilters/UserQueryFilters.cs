using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class UserQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public UserType? Type { get; set; }

        public StatusType? Status { get; set; }

        public UserOrderType? Order { get; set; }
    }
}