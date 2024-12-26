using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class NSSCJobExperienceItemListDto
    {
        public Guid ID { get; set; }

        public Guid NSSCAuditorActivityID { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

    } // NSSCJobExperienceItemListDto

    public class NSSCJobExperienceItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid NSSCAuditorActivityID { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public NSSCAuditorActivityItemListDto NSSCAuditorActivity { get; set; }

        // public ICollection<NSSCAuditExperienceItemListDto> NSSCAuditExperiences { get; set; }
    } // NSSCJobExperienceItemDetailDto

    public class NSSCJobExperiencePostDto
    {
        [Required]
        public Guid NSSCAuditorActivityID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCJobExperiencePostDto

    public class NSSCJobExperiencePutDto
    {
        [Required]
        public Guid ID { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCJobExperiencePutDto

    public class NSSCJobExperienceDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCJobExperienceDeleteDto
}