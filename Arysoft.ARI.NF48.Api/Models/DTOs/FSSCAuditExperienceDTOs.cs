using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class FSSCAuditExperienceItemListDto
    {
        public Guid ID { get; set; }

        public Guid FSSCAuditorActivityID { get; set; }

        public Guid? FSSCJobExperienceID { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }
    } // FSSCAuditExperienceItemListDto

    public class FSSCAuditExperienceItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid FSSCAuditorActivityID { get; set; }

        public Guid? FSSCJobExperienceID { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public FSSCAuditorActivityItemListDto FSSCAuditorActivity { get; set; }

        public FSSCJobExperienceItemListDto FSSCJobExperience { get; set; }
    } // FSSCAuditExperienceItemDetailDto

    public class FSSCAuditExperiencePostDto
    {
        [Required]
        public Guid FSSCAuditorActivityID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCAuditExperiencePostDto

    public class FSSCAuditExperiencePutDto
    {
        [Required]
        public Guid ID { get; set; }

        public Guid? FSSCJobExperienceID { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCAuditExperiencePutDto

    public class FSSCAuditExperienceDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCAuditExperienceDeleteDto
}