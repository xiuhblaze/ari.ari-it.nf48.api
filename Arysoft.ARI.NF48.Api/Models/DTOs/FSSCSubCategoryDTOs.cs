using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class FSSCSubCategoryItemListDto
    {
        public Guid ID { get; set; }

        public Guid FSSCCategoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public string FSSCCategoryName { get; set; }

        public int ActivitiesCount { get; set; }
    } // FSSCSubCategoryItemListDto

    public class FSSCSubCategoryItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid FSSCCategoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public FSSCCategoryItemListDto FSSCCategory { get; set; }

        public IEnumerable<FSSCActivityItemListDto> FSSCActivities { get; set; }
    } // FSSCCategoryItemDetailDto

    public class FSSCSubCategoryPostDto
    {
        [Required]
        public Guid FSSCCategoryID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCSubCategoryPostDto

    public class FSSCSubCategoryPutDto
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
    } // FSSCSubCategoryPutDto

    public class FSSCSubCategoryDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCSubCategoryDeleteDto
}