using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class ShiftPostDto
    {
        [Required]
        public Guid SiteID { get; set; }

        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class ShiftPutDto
    {
        [Required]
        public Guid ShiftID { get; set; }

        [Required]
        public ShiftType Type { get; set; }

        public int NoEmployees { get; set; }

        [Required]
        [Range(0, 23, ErrorMessage = "Only hours, can be between 0 .. 23")]
        public int ShiftBegin { get; set; }

        [Required]
        [Range(0, 23, ErrorMessage = "Only hours, can be between 0 .. 23")]
        public int ShiftEnd { get; set; }

        [StringLength(500)]
        public string ActivitiesDescription { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}