using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class MD5QueryFilters : BaseQueryFilters
    {
        public int? NumEmployees { get; set; }

        public int? Days { get; set; }

        public StatusType? Status { get; set; }

        public MD5OrderType? Order { get; set; }
    }
}