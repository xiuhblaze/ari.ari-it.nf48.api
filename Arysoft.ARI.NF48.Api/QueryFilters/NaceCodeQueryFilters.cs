using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class NaceCodeQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public int? Sector { get; set; }

        public int? Division { get; set; }

        public int? Group { get; set; }

        public int? Class { get; set; }

        public NaceCodeOnlyOptionType? OnlyOption { get; set; }

        public StatusType? Status { get; set; }

        public NaceCodeOrderType Order { get; set; }
    }
}