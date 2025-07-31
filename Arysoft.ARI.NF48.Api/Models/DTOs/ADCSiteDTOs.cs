using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

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

        public decimal? Surveillance { get; set; }

        public decimal? RR { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public string ADCDescription { get; set; }

        public string SiteDescription { get; set; }

        public ICollection<ADCConceptValueItemListDto> ADCConceptValues { get; set; }

        // NOT MAPPED

        public List<ADCSiteAlertType> Alerts { get; set; }

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

        public decimal? Surveillance { get; set; }

        public decimal? RR { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public ADCItemListDto ADC { get; set; }

        public SiteItemListDto Site { get; set; }

        public ICollection<ADCConceptValueItemListDto> ADCConceptValues { get; set; }

        // NOT MAPPED

        public List<ADCSiteAlertType> Alerts { get; set; }

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

        //public string MD11Filename { get; set; } // Creo que los voy a generar en el Controller

        //public string MD11UploadedBy { get; set; }

        public decimal? Surveillance { get; set; }

        public decimal? RR { get; set; }

        public string ExtraInfo { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCSiteItemUpdateDto

    public class ADCSiteItemDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCSiteItemDeleteDto
}