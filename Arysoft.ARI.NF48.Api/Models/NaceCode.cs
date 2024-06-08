using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class NaceCode : BaseModel
    {
        public int? Sector { get; set; }

        public int? Division { get; set; }

        public int? Group { get; set; }

        public int? Class { get; set; }

         public string Description { get; set; }

        public StatusType Status { get; set; }
    }
}