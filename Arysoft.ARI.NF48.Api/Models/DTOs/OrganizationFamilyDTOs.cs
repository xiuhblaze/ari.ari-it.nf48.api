using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class OrganizationFamilyItemListDto
    {
        public Guid ID { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public IEnumerable<OrganizationItemSimpleDto> Organizations { get; set; }
    } // OrganizationFamilyItemListDto

    public class OrganizationFamilyItemDetailDto
    {
        public Guid ID { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public IEnumerable<OrganizationItemListDto> Organizations { get; set; }
    } // OrganizationFamilyItemDetailDto

    public class OrganizationFamilyPostDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // OrganizationFamilyPostDto

    public class OrganizationFamilyPutDto
    {
        [Required]
        public Guid ID { get; set; }

        public string Description { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // OrganizationFamilyPutDto

    public class OrganizationFamilyDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // OrganizationFamilyDeleteDto
}