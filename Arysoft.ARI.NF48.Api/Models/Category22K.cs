using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Category22K : BaseModel
    {
        public string Cluster { get; set; }

        public string Category { get; set; }

        public string CategoryDescription { get; set; }

        public string SubCategory { get; set; }

        public string SubCategoryDescription { get; set; }

        public string Examples { get; set; }

        public decimal? BasicDaysTD { get; set; }

        public decimal? HACCPDaysTH { get; set; }

        public Categories22KVersionType? Version { get; set; }

        public Category22KAccreditedType? AccreditedStatus { get; set; }
    }
}