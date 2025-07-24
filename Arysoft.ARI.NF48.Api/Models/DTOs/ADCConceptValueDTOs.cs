using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class ADCConceptValueItemListDto
    {
        public Guid ID { get; set; }

        public Guid ADCConceptID { get; set; }

        public Guid ADCSiteID { get; set; }

        public bool? CheckValue { get; set; }

        public decimal? Value { get; set; }

        public string Justification { get; set; }

        public decimal? ValueApproved { get; set; }

        public string JustificationApproved { get; set; }

        public ADCConceptUnitType? ValueUnit { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public string ADCConceptDescription { get; set; }

        public string ADCSiteDescription { get; set; }
    } // ADCConceptValueItemListDto

    public class ADCConceptValueItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid ADCConceptID { get; set; }

        public Guid ADCSiteID { get; set; }

        public bool? CheckValue { get; set; }

        public decimal? Value { get; set; }

        public string Justification { get; set; }

        public decimal? ValueApproved { get; set; }

        public string JustificationApproved { get; set; }

        public ADCConceptUnitType? ValueUnit { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public ADCConceptItemListDto ADCConcept { get; set; }

        public ADCSiteItemListDto ADCSite { get; set; }
    } // ADCConceptValueItemDetailDto

    public class ADCConceptValueItemCreateDto
    {
        [Required]
        public Guid ADCConceptID { get; set; }

        [Required]
        public Guid ADCSiteID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCConceptValueItemCreateDto

    public class ADCConceptValueItemUpdateDto
    { 
        [Required]
        public Guid ID { get; set; }

        [Required]
        public bool? CheckValue { get; set; }

        public decimal? Value { get; set; }

        [StringLength(500)]
        public string Justification { get; set; }

        public decimal? ValueApproved { get; set; }

        [StringLength(500)]
        public string JustificationApproved { get; set; }

        public ADCConceptUnitType? ValueUnit { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCConceptValueItemUpdateDto

    public class ADCConceptValueItemDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCConceptValueItemDeleteDto
}