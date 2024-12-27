using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class NSSCAuditExperienceItemListDto
    {
        public Guid ID { get; set; }

        public Guid NSSCAuditorActivityID { get; set; }

        public Guid? NSSCJobExperienceID { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }
    } // NSSCAuditExperienceItemListDto

    public class NSSCAuditExperienceItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid NSSCAuditorActivityID { get; set; }

        public Guid? NSSCJobExperienceID { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public NSSCAuditorActivityItemListDto NSSCAuditorActivity { get; set; }

        public NSSCJobExperienceItemListDto NSSCJobExperience { get; set; }
    } // NSSCAuditExperienceItemDetailDto

    public class NSSCAuditExperiencePostDto
    {
        [Required]
        public Guid NSSCAuditorActivityID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCAuditExperiencePostDto

    public class NSSCAuditExperiencePutDto
    {
        [Required]
        public Guid ID { get; set; }

        public Guid? NSSCJobExperienceID { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCAuditExperiencePutDto

    public class NSSCAuditExperienceDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCAuditExperienceDeleteDto
}