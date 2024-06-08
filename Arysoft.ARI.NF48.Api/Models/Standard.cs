using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Standard : BaseModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? MaxReductionsDays { get; set; } 

        public StatusType Status { get; set; }
    }
}