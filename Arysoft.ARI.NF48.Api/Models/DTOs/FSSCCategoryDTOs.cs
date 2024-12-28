using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class FSSCCategoryItemListDto
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public int SubCategoriesCount { get; set; }
    } // FSSCCategoryItemListDto

    public class FSSCCategoryItemDetailDto
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public IEnumerable<FSSCSubCategoryItemListDto> FSSCSubCategories { get; set; }
    } // FSSCCategoryItemDetailDto

    public class FSSCCategoryPostDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCCategoryPostDto

    public class FSSCCategoryPutDto
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
    } // FSSCCategoryPutDto

    public class FSSCCategoryDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCCategoryDeleteDto
}