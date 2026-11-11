using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class CalendarBlockItemListDto
    {
        public Guid ID { get; set; }

        public Guid? OwnerID { get; set; }

        public CalendarBlockType? BlockType { get; set; }

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public CalendarBlockOwnerType? OwnerType { get; set; }

        public bool AllowOverride { get; set; }

        public StatusType Status { get; set; }

        // Relations

        public string OwnerName { get; set; }
    } // CalendarBlockItemListDto

    public class CalendarBlockItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid? OwnerID { get; set; }

        public CalendarBlockType? BlockType { get; set; }

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public CalendarBlockOwnerType? OwnerType { get; set; }

        public bool AllowOverride { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }
    } // CalendarBlockItemDetailDto

    public class CalendarBlockCreateDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // CalendarBlockCreateDto

    public class CalendarBlockUpdateDto
    {
        [Required]
        public Guid ID { get; set; }

        public Guid? OwnerID { get; set; }

        public CalendarBlockType? BlockType { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }

        [Required]
        public CalendarBlockOwnerType? OwnerType { get; set; }

        [Required]
        public bool AllowOverride { get; set; }

        [StringLength(500)]
        public string ExtraInfo { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // CalendarBlockUpdateDto

    public class CalendarBlockDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // CalendarBlockDeleteDto
}