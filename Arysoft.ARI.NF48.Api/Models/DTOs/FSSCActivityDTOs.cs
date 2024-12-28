using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class FSSCActivityItemListDto
    {
        public Guid ID { get; set; }

        public Guid FSSCSubCategoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public string FSSCSubCategoryName { get; set; }

        // public int ActivitiesCount { get; set; }
    } // FSSCActivityItemListDto

    public class FSSCActivityItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid FSSCSubCategoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public FSSCSubCategoryItemListDto FSSCSubCategory { get; set; }

        // public IEnumerable<FSSCActivityItemListDto> FSSCActivities { get; set; }
    } // FSSCCategoryItemDetailDto

    public class FSSCActivityPostDto
    {
        [Required]
        public Guid FSSCSubCategoryID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCActivityPostDto

    public class FSSCActivityPutDto
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
    } // FSSCActivityPutDto

    public class FSSCActivityDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCActivityDeleteDto
}