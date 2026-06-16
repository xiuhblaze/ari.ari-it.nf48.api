using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{ 
    public class RiskLevelItemListDto
    {
        public Guid ID { get; set; }

        public Guid? StandardID { get; set; }

        public RiskLevelCategory? Category { get; set; }

        public string BusinessSector{ get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public string StandardName { get; set; }
    } // RiskLevelItemListDto

    public class RiskLevelItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid? StandardID { get; set; }
        
        public RiskLevelCategory? Category { get; set; }
        
        public string BusinessSector { get; set; }
        
        public StatusType Status { get; set; }
        
        public DateTime Created { get; set; }
        
        public DateTime Updated { get; set; }
        
        public string UpdatedUser { get; set; }

        // RELATIONS
        
        public StandardItemListDto Standard { get; set; }
    } // RiskLevelItemDetailDto

    public class RiskLevelCreateDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // RiskLevelCreateDto

    public class RiskLevelUpdateDto
    {
        [Required]
        public Guid ID { get; set; }

        public Guid? StandardID { get; set; }

        public RiskLevelCategory? Category { get; set; }
        
        public string BusinessSector { get; set; }
        
        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // RiskLevelUpdateDto

    public class RiskLevelDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // RiskLevelDeleteDto
}