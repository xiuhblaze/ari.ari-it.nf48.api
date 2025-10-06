using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class ADCItemListDto
    {
        public Guid ID { get; set; }

        public Guid AuditCycleID { get; set; }

        public Guid AppFormID { get; set; }

        public string Description { get; set; }

        public int? TotalEmployees { get; set; }

        public decimal? TotalInitial { get; set; }

        public decimal? TotalMD11 { get; set; }

        public decimal? TotalSurveillance { get; set; }

        public decimal? TotalRR { get; set; }

        public string UserCreates { get; set; }

        public string UserReview { get; set; }

        public DateTime? ReviewDate { get; set; }

        public string ReviewComments { get; set; }

        public DateTime? ActiveDate { get; set; }

        public string ExtraInfo { get; set; }

        public ADCStatusType Status { get; set; }

        // INTERNAL

        public string HistoricalDataJSON { get; set; }

        // RELATIONS

        public string AuditCycleName { get; set; }

        public string AppFormOrganizationName { get; set; }

        public string AppFormAuditCycleName { get; set; } // Posiblemente lo vaya a eliminar

        public string AppFormStandardName { get; set; }

        public int NotesCount { get; set; }

        public int ADCSitesCount { get; set; }

        // NOT MAPPED

        public List<ADCAlertType> Alerts { get; set; }

    } // ADCItemListDto

    public class ADCItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid AuditCycleID { get; set; }

        public Guid AppFormID { get; set; }

        public string Description { get; set; }

        public int? TotalEmployees { get; set; }

        public decimal? TotalInitial { get; set; }

        public decimal? TotalMD11 { get; set; }

        public decimal? TotalSurveillance { get; set; }

        public decimal? TotalRR { get; set; }

        public string UserCreates { get; set; }

        public string UserReview { get; set; }

        public DateTime? ReviewDate { get; set; }

        public string ReviewComments { get; set; }

        public DateTime? ActiveDate { get; set; }

        public string ExtraInfo { get; set; }

        public ADCStatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // INTERNAL

        public string HistoricalDataJSON { get; set; }

        // RELATIONS

        public AuditCycleItemListDto AuditCycle { get; set; }

        public AppFormItemListDto AppForm { get; set; }

        public ICollection<ADCSiteItemListDto> ADCSites { get; set; }

        public ICollection<NoteItemDto> Notes { get; set; }

        // NOT MAPPED

        public List<ADCAlertType> Alerts { get; set; }

    } // ADCItemDetailDto

    public class ADCCreateDto
    {
        [Required]
        public Guid AppFormID { get; set; }

        [Required]
        public Guid AuditCycleID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCCreateDto

    public class ADCUpdateDto
    {
        [Required]
        public Guid ID { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public decimal? TotalInitial { get; set; }

        public decimal? TotalMD11 { get; set; }

        public decimal? TotalSurveillance { get; set; }
        
        [StringLength(1000)]
        public string ReviewComments { get; set; }

        [StringLength(500)]
        public string ExtraInfo { get; set; }

        [Required]
        public ADCStatusType Status { get; set; }

        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCUpdateDto

    public class ADCWithSiteListUpdateDto
    {
        [Required]
        public ADCUpdateDto ADC { get; set; }

        [Required]
        public List<ADCSiteWithCVListUpdateDto> ADCSites { get; set; }
    } // ADCWithSiteListUpdateDto

    public class ADCDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCDeleteDto
}