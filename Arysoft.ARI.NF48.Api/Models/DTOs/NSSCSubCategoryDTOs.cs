using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class NSSCSubCategoryItemListDto
    {
        public Guid ID { get; set; }

        public Guid NSSCCategoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public string NSSCCategoryName { get; set; }

        public int ActivitiesCount { get; set; }
    } // NSSCSubCategoryItemListDto

    public class NSSCSubCategoryItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid NSSCCategoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public NSSCCategoryItemListDto NSSCCategory { get; set; }

        // public IEnumerable<NSSCActivityItemListDto> NSSCActivities { get; set; }
    } // NSSCCategoryItemDetailDto

    public class NSSCSubCategoryPostDto
    {
        [Required]
        public Guid NSSCCategoryID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCSubCategoryPostDto

    public class NSSCSubCategoryPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCSubCategoryPutDto

    public class NSSCSubCategoryDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCSubCategoryDeleteDto
}