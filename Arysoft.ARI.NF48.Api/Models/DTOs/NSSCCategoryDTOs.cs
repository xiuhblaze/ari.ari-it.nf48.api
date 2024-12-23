using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class NSSCCategoryItemListDto
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public int SubCategoriesCount { get; set; }
    } // NSSCCategoryItemListDto

    public class NSSCCategoryItemDetailDto
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public IEnumerable<NSSCSubCategoryItemListDto> NSSCSubCategories { get; set; }
    } // NSSCCategoryItemDetailDto

    public class NSSCCategoryPostDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCCategoryPostDto

    public class NSSCCategoryPutDto
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
    } // NSSCCategoryPutDto

    public class NSSCCategoryDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCCategoryDeleteDto
}