using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class NSSCActivityItemListDto
    {
        public Guid ID { get; set; }

        public Guid NSSCSubCategoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public string NSSCSubCategoryName { get; set; }

        // public int ActivitiesCount { get; set; }
    } // NSSCActivityItemListDto

    public class NSSCActivityItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid NSSCSubCategoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public NSSCSubCategoryItemListDto NSSCSubCategory { get; set; }

        // public IEnumerable<NSSCActivityItemListDto> NSSCActivities { get; set; }
    } // NSSCCategoryItemDetailDto

    public class NSSCActivityPostDto
    {
        [Required]
        public Guid NSSCSubCategoryID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCActivityPostDto

    public class NSSCActivityPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCActivityPutDto

    public class NSSCActivityDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCActivityDeleteDto
}