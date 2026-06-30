using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class MD5 : BaseModel
    {
        public MD5TableType? TableType { get; set; }

        public int? StartValue { get; set; }

        public int? EndValue { get; set; }

        public decimal? HighDays { get; set; }

        public decimal? Days { get; set; }

        public decimal? LowDays { get; set; }

        public decimal? LimDays { get; set; }
    }
}