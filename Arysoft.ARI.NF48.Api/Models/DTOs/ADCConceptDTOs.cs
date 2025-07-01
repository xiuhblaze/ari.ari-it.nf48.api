using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class ADCConceptItemListDto
    {
        public Guid ID { get; set; }

        public Guid StandardID { get; set; }

        public string Description { get; set; }

        public int? IndexSort { get; set; }

        public bool? WhenTrue { get; set; }
        
        public decimal? Increase { get; set; }
        
        public decimal? Decrease { get; set; }
        
        public ADCConceptUnitType? IncreaseUnit { get; set; }
        
        public ADCConceptUnitType? DecreaseUnit { get; set; }
        
        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public string StandardName { get; set; }
    } // ADCConceptItemListDto

    public class ADCConceptItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid StandardID { get; set; }

        public int? IndexSort { get; set; }
        
        public string Description { get; set; }
        
        public bool? WhenTrue { get; set; }

        public decimal? Increase { get; set; }

        public decimal? Decrease { get; set; }

        public ADCConceptUnitType? IncreaseUnit { get; set; }

        public ADCConceptUnitType? DecreaseUnit { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }
        
        public DateTime Created { get; set; }
        
        public DateTime Updated { get; set; }
        
        public string UpdatedUser { get; set; }

        // RELATIONS

        public StandardItemListDto Standard { get; set; }
    } // ADCConceptItemDetailDto

    public class ADCConceptItemCreateDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCConceptItemCreateDto

    public class ADCConceptItemUpdateDto
    {
        [Required]
        public Guid ID { get; set; }

        public Guid StandardID { get; set; }

        public int? IndexSort { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public bool? WhenTrue { get; set; }

        public decimal? Increase { get; set; }

        public decimal? Decrease { get; set; }

        public ADCConceptUnitType? IncreaseUnit { get; set; }

        public ADCConceptUnitType? DecreaseUnit { get; set; }

        [StringLength(500)]
        public string ExtraInfo { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCConceptItemUpdateDto

    public class ADCConceptItemDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCConceptItemDeleteDto
}