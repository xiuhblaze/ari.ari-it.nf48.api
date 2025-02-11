using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class DayCalculationConceptApplicationItemListDto
    {
        public Guid ID { get; set; }

        //public Guid? DayCalculationConceptID { get; set; }

        //public Guid? ApplicationID { get; set; }

        public int? Value { get; set; }

        public string Justification { get; set; }

        public int? ValueApproved { get; set; }

        public string JustificationApproved { get; set; }

        public DayCalculationConceptUnitType? Unit { get; set; }

        public StatusType Status { get; set; }

        public string DayCalculationConceptDescription { get; set; }
    }

    public class DayCalculationConceptApplicationItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid DayCalculationConceptID { get; set; }

        public Guid ApplicationID { get; set; }

        public int? Value { get; set; }

        public string Justification { get; set; }

        public int? ValueApproved { get; set; }

        public string JustificationApproved { get; set; }

        public DayCalculationConceptUnitType? Unit { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public DayCalculationConceptItemListDto DayCalculationConcept { get; set; }

        public ApplicationItemListDto Application { get; set; }
    }

    public class DayCalculationConceptApplicationPostDto
    {
        [Required]
        public Guid DayCalculationConceptID { get; set; }

        [Required]
        public Guid ApplicationID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class DayCalculationConceptApplicationPutDto
    {
        public Guid ID { get; set; }

        public int? Value { get; set; }

        public string Justification { get; set; }

        public int? ValueApproved { get; set; }

        public string JustificationApproved { get; set; }

        public DayCalculationConceptUnitType? Unit { get; set; }

        public StatusType Status { get; set; }

        public string UpdatedUser { get; set; }
    }

    public class DayCalculationConceptApplicationDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}