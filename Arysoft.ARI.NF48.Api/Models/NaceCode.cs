using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class NaceCode : BaseModel
    {
        public int? Sector { get; set; }

        public int? Division { get; set; }

        public int? Group { get; set; }

        public int? Class { get; set; }

        public string Description { get; set; }

        public NaceCodeAccreditedType? AccreditedStatus { get; set; }

        public string AccreditationInfo { get; set; }

        public DateTime? AccreditationDate { get; set; }
    }
}