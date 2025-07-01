using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class MD5ItemListDto
    {
        public Guid ID { get; set; }

        public int? StartValue { get; set; }

        public int? EndValue { get; set; }

        public decimal? Days { get; set; }

        public StatusType Status { get; set; }
    }

    public class MD5ItemDetailDto
    {
        public Guid ID { get; set; }

        public int? StartValue { get; set; }

        public int? EndValue { get; set; }

        public decimal? Days { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }
    }

    public class MD5ItemCreateDto
    {
        public int? StartValue { get; set; }

        public int? EndValue { get; set; }

        public decimal? Days { get; set; }

        public StatusType? Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class MD5ItemUpdateDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public int StartValue { get; set; }

        [Required]
        public int EndValue { get; set; }

        [Required]
        public decimal Days { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class MD5ItemDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}