using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class DayCalculationConcept : BaseModel
    {
        public Guid? StandardID { get; set; }

        public string Description { get; set; }

        public int? Increase { get; set; }

        public int? Decrease { get; set; }

        public DayCalculationConceptUnitType Unit { get; set; }

        //public StatusType Status { get; set; }

        // Relations

        public virtual Standard Standard { get; set; }
    }
}