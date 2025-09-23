using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class ADCSiteItemListDto
    {
        public Guid ID { get; set; }

        public Guid ADCID { get; set; }

        public Guid? SiteID { get; set; }

        public decimal? InitialMD5 { get; set; }

        public int? NoEmployees { get; set; }

        public decimal? TotalInitial { get; set; }

        public decimal? MD11 { get; set; } // Ahora se va a manejar como porcentaje

        public string MD11Filename { get; set; }

        public string MD11UploadedBy { get; set; }

        public decimal? Total { get; set; }

        public decimal? Surveillance { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public string ADCDescription { get; set; }

        public string SiteDescription { get; set; }

        public string SiteAddress { get; set; }

        public ICollection<ADCConceptValueItemListDto> ADCConceptValues { get; set; }

        public ICollection<ADCSiteAuditItemDto> ADCSiteAudits { get; set; }

        // NOT MAPPED

        public List<ADCSiteAlertType> Alerts { get; set; }

        public bool IsMultiStandard { get; set; }

    } // ADCSiteItemListDto

    public class ADCSiteItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid ADCID { get; set; }

        public Guid? SiteID { get; set; }

        public decimal? InitialMD5 { get; set; }

        public int? NoEmployees { get; set; }

        public decimal? TotalInitial { get; set; }

        public decimal? MD11 { get; set; }

        public string MD11Filename { get; set; }

        public string MD11UploadedBy { get; set; }

        public decimal? Total { get; set; }

        public decimal? Surveillance { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public ADCItemListDto ADC { get; set; }

        public SiteItemListDto Site { get; set; }

        public ICollection<ADCConceptValueItemListDto> ADCConceptValues { get; set; }

        public ICollection<ADCSiteAuditItemDto> ADCSiteAudits { get; set; }

        // NOT MAPPED

        public List<ADCSiteAlertType> Alerts { get; set; }

        public bool IsMultiStandard { get; set; }

    } // ADCSiteItemDetailDto

    public class ADCSiteItemCreateDto
    {
        [Required]
        public Guid ADCID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCSiteItemCreateDto

    public class ADCSiteItemUpdateDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public Guid SiteID { get; set; }

        public decimal? TotalInitial { get; set; }

        public decimal? MD11 { get; set; }

        public decimal? Total { get; set; }

        public decimal? Surveillance { get; set; }

        public string ExtraInfo { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCSiteItemUpdateDto

    public class ADCSiteListUpdateDto
    { 
        [Required]
        public List<ADCSiteItemUpdateDto> Items { get; set; }
    } // ADCSiteListUpdateDto

    public class ADCSiteWithCVListUpdateDto
    {
        [Required]
        public ADCSiteItemUpdateDto ADCSite { get; set; }

        [Required]
        public List<ADCConceptValueItemUpdateDto> ADCConceptValues { get; set; }
    } // ADCSiteWithCVListUpdateDto

    public class ADCSiteItemDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCSiteItemDeleteDto
}