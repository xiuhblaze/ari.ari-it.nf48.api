using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class SiteItemListDto
    {
        public Guid ID { get; set; }

        public string OrganizationName { get; set; }

        public string Description { get; set; }

        public bool IsMainSite { get; set; }

        public string Address { get; set; }

        //public double? LocationLat { get; set; }

        //public double? LocationLong { get; set; }

        public string LocationURL { get; set; }

        public StatusType Status { get; set; }

        public int ShiftsCount { get; set; }

        public int EmployeesCount { get; set; }

    } // SiteItemListDto

    public class SiteItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid OrganizationID { get; set; }

        public string Description { get; set; }

        public bool IsMainSite { get; set; }

        public string Address { get; set; }

        //public double? LocationLat { get; set; }

        //public double? LocationLong { get; set; }

        public string LocationURL { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public IEnumerable<ShiftItemListDto> Shifts { get; set; } 

        public OrganizationItemListDto Organization { get; set; } 

    } // SiteItemDetailDto

    public class SitePostDto
    {
        [Required]
        public Guid OrganizationID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // SitePostDto

    public class SitePutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public bool IsMainSite { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        //public double? LocationLat { get; set; }

        //public double? LocationLong { get; set; }

        [StringLength(250)]
        public string LocationURL { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // SitePutDto

    public class SiteDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // SiteDeleteDto
}