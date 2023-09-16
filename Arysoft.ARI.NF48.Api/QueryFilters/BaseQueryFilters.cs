namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public abstract class BaseQueryFilters
    {
        public bool? IncludeDeleted { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }
    }
}