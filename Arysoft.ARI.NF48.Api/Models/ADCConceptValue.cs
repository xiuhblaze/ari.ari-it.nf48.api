using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class ADCConceptValue : BaseModel
    {
        public Guid ADCConceptID { get; set; }

        public Guid ADCSiteID { get; set; }

        public bool? CheckValue { get; set; }

        public decimal? Value { get; set; }

        public string Justification { get; set; }

        public decimal? ValueApproved { get; set; }

        public string JustificationApproved { get; set; }

        public ADCConceptUnitType ValueUnit { get; set; }

        // RELATIONS

        public virtual ADCConcept ADCConcept { get; set; }

        public virtual ADCSite ADCSite { get; set; }
    }
}