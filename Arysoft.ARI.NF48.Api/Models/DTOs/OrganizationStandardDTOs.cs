using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class OrganizationStandardItemListDto
    {
        public Guid ID { get; set; }

        public Guid OrganizationID { get; set; }

        public Guid StandardID { get; set; }

        public string ExtraInfo { get; set; }

        public string OrganizationName { get; set; }

        public string StandardName { get; set; }

        public StandardBaseType StandardBase { get; set; }

        public StatusType Status { get; set; }
    } // OrganizationStandardItemListDto

    public class OrganizationStandardItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid OrganizationID { get; set; }

        public Guid StandardID { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public OrganizationItemListDto Organization { get; set; }

        public StandardItemListDto Standard { get; set; }
    } // OrganizationStandardItemDetailDto

    public class OrganizationStandardPostDto
    {
        [Required]
        public Guid OrganizationID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // OrganizationStandardPostDto

    public class OrganizationStandardPutDto
    {
        [Required]
        public Guid ID { get; set; }

        public Guid StandardID { get; set; }

        [StringLength(1000)]
        public string ExtraInfo { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // OrganizationStandardPutDto

    public class OrganizationStandardDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // OrganizationStandardDeleteDto
}