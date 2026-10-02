using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class RiskLevel : BaseModel
    {
        public Guid? StandardID { get; set; }

        public RiskLevelCategoryType? Category { get; set; }

        public string BusinessSector { get; set; }

        // RELATIONS

        public virtual Standard Standard { get; set; }
    }
}
