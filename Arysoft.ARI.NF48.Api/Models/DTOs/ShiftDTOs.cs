using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class ShiftItemListDto
    {
        public Guid ID { get; set; }

        public string SiteDescription { get; set; }

        public ShiftType? Type { get; set; }

        public int? NoEmployees { get; set; }

        public DateTime? ShiftBegin { get; set; }

        public DateTime? ShiftEnd { get; set; }

        public string ActivitiesDescription { get; set; }

        public StatusType Status { get; set; }
    }

    public class ShiftItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid SiteID { get; set; }

        public ShiftType? Type { get; set; }

        public int? NoEmployees { get; set; }

        public DateTime? ShiftBegin { get; set; }

        public DateTime? ShiftEnd { get; set; }

        public string ActivitiesDescription { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public SiteItemListDto Site { get; set; } // // HACK: Cambiar por SiteItemListDto cuando exista
    } // ShiftItemDetailDto

    public class ShiftPostDto
    {
        [Required]
        public Guid SiteID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class ShiftPutDto
    {
        [Required(ErrorMessage = "ID is required")]
        public Guid ID { get; set; }

        [Required]
        public ShiftType Type { get; set; }

        public int NoEmployees { get; set; }

        [Required]
        //[Range(0, 23, ErrorMessage = "Only hours, can be between 0 .. 23")]
        public DateTime? ShiftBegin { get; set; }

        [Required]
        //[Range(0, 23, ErrorMessage = "Only hours, can be between 0 .. 23")]
        public DateTime? ShiftEnd { get; set; }

        [StringLength(500)]
        public string ActivitiesDescription { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class ShiftDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}