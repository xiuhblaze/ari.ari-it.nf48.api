using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class Category22KItemListDto
    {
        public Guid ID { get; set; }

        public string Cluster { get; set; }

        public string Category { get; set; }

        public string CategoryDescription { get; set; }

        public string SubCategory { get; set; }

        public string SubCategoryDescription { get; set; }

        public string Examples { get; set; }

        public bool IsAccredited { get; set; }

        public StatusType Status { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }
    } // Category22KItemListDto

    public class Category22KItemDetailDto
    {
        public Guid ID { get; set; }

        public string Cluster { get; set; }

        public string Category { get; set; }

        public string CategoryDescription { get; set; }

        public string SubCategory { get; set; }

        public string SubCategoryDescription { get; set; }

        public string Examples { get; set; }

        public bool IsAccredited { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }
    } // Category22KItemDetailDto

    public class Category22KPostDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class Category22KPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Cluster { get; set; }

        [Required]
        [StringLength(5)]
        public string Category { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryDescription { get; set; }

        [StringLength(5)]
        public string SubCategory { get; set; }

        [StringLength(100)]
        public string SubCategoryDescription { get; set; }

        public string Examples { get; set; }

        public bool IsAccredited { get; set; }

        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class Category22KDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}