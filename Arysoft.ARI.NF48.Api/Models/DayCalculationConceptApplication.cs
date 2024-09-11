using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class DayCalculationConceptApplication : BaseModel
    {
        public Guid DayCalculationConceptID { get; set; }

        public Guid ApplicationID { get; set; }

        public int? Value { get; set; }

        public string Justification { get; set; }

        public int? ValueApproved { get; set; }

        public string JustificationApproved { get; set; }

        public DayCalculationConceptUnitType? Unit { get; set; }

        public StatusType Status { get; set; }

        // Relations

        public virtual DayCalculationConcept DayCalculationConcept { get; set; }

        public virtual Application Application { get; set; }
    }
}