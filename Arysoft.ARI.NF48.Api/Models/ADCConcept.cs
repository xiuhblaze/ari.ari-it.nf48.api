using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class ADCConcept : BaseModel
    {
        public Guid StandardID { get; set; }

        public int IndexSort { get; set; }      // Para ordenar los conceptos de forma personalizada

        public string Description { get; set; }

        public bool? WhenTrue { get; set; }

        public decimal? Increase { get; set; }

        public decimal? Decrease { get; set; }

        public ADCConceptUnitType? IncreaseUnit { get; set; }

        public ADCConceptUnitType? DecreaseUnit { get; set; }

        public bool? ToFinalTiming { get; set; }

        // RELATIONS

        public virtual Standard Standard { get; set; }

        public virtual ICollection<ADCConceptValue> ADCConceptValues { get; set; }
    }
}