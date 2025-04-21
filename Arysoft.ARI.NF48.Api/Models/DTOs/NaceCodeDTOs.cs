using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class NaceCodeItemListDto
    {
        public Guid ID { get; set; }

        public int? Sector { get; set; }

        public int? Division { get; set; }

        public int? Group { get; set; }

        public int? Class { get; set; }

        public string Description { get; set; }

        public NaceCodeAccreditedType? AccreditedStatus { get; set; }

        public string AccreditationInfo { get; set; }

        public DateTime? AccreditationDate { get; set; }

        public StatusType Status { get; set; }
    }

    public class NaceCodeItemDetailDto
    {
        public Guid ID { get; set; }

        public int? Sector { get; set; }

        public int? Division { get; set; }

        public int? Group { get; set; }

        public int? Class { get; set; }

        public string Description { get; set; }

        public NaceCodeAccreditedType? AccreditedStatus { get; set; }

        public string AccreditationInfo { get; set; }

        public DateTime? AccreditationDate { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }
    } // NaceCodeItemDetailDto

    public class NaceCodePostDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class NaceCodePutDto
    {
        [Required]
        public Guid ID { get; set; }

        public int? Sector { get; set; }

        public int? Division { get; set; }

        public int? Group { get; set; }

        public int? Class { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public NaceCodeAccreditedType? AccreditedStatus { get; set; }

        [StringLength(500)]
        public string AccreditationInfo { get; set; }

        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NaceCodePutDto

    public class NaceCodeDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NaceCodeDeleteDto
}