
using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class CompanyItemListDto
    {
        public Guid ID { get; set; }

        public Guid OrganizationID { get; set; }

        public string Name { get; set; }

        public string LegalEntity { get; set; }

        public string COID { get; set; }

        public StatusType Status { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public string OrganizationName { get; set; }
    } // CompanyItemListDto

    public class CompanyItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid OrganizationID { get; set; }

        public string Name { get; set; }

        public string LegalEntity { get; set; }

        public string COID { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public OrganizationItemListDto Organization { get; set; }
    } // CompanyItemDetailDto

    public class CompanyPostDto
    {
        [Required]
        public Guid OrganizationID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // CompanyPostDto

    public class CompanyPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string LegalEntity { get; set; }
                
        [StringLength(20)]
        public string COID { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // CompanyPutDto

    public class CompanyDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}