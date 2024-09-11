using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class DayCalculationConceptItemListDto
    {
        public Guid ID { get; set; }

        public string StandardName {  get; set; }

        public string Description { get; set; }

        public int? Increase { get; set; }

        public int? Decrease { get; set; }

        public DayCalculationConceptUnitType Unit { get; set; }

        public StatusType Status { get; set; }
    }

    public class DayCalculationConceptItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid? StandardID { get; set; }

        public string Description { get; set; }

        public int? Increase { get; set; }

        public int? Decrease { get; set; }

        public DayCalculationConceptUnitType Unit { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public StandardItemListDto Standard { get; set; }
    }

    public class DayCalculationConceptPostDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class DayCalculationConceptPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public Guid? StandardID { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public int? Increase { get; set; }

        public int? Decrease { get; set; }

        public DayCalculationConceptUnitType Unit { get; set; }

        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class DayCalculationConceptDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}