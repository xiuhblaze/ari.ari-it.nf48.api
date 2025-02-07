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

        public string ActivitiesDescription { get; set; }

        public TimeSpan? ShiftStart { get; set; }

        public TimeSpan? ShiftEnd { get; set; }

        public TimeSpan? ShiftStart2 { get; set; }

        public TimeSpan? ShiftEnd2 { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }
    }

    public class ShiftItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid SiteID { get; set; }

        public ShiftType? Type { get; set; }

        public int? NoEmployees { get; set; }

        public string ActivitiesDescription { get; set; }

        public TimeSpan? ShiftStart { get; set; }

        public TimeSpan? ShiftEnd { get; set; }

        public TimeSpan? ShiftStart2 { get; set; }

        public TimeSpan? ShiftEnd2 { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public SiteItemListDto Site { get; set; }
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

        [Required]
        public int NoEmployees { get; set; }

        [StringLength(500)]
        public string ActivitiesDescription { get; set; }

        [Required]
        public TimeSpan? ShiftStart { get; set; }

        [Required]
        public TimeSpan? ShiftEnd { get; set; }

        public TimeSpan? ShiftStart2 { get; set; }

        public TimeSpan? ShiftEnd2 { get; set; }

        [StringLength(1000)]
        public string ExtraInfo { get; set; }

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