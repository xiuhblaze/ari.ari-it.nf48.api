using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class MD5QueryFilters : BaseQueryFilters
    {
        public MD5TableType? MD5TableType { get; set; }

        public int? NumEmployees { get; set; }

        public int? Days { get; set; }

        public int? StartDays { get; set; }

        public int? EndDays { get; set; }

        public int? StartEmployees { get; set; }

        public int? EndEmployees { get; set; }

        public StatusType? Status { get; set; }

        public MD5OrderType? Order { get; set; }
    }
}