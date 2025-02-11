using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class FSSCJobExperienceItemListDto
    {
        public Guid ID { get; set; }

        public Guid FSSCAuditorActivityID { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

    } // FSSCJobExperienceItemListDto

    public class FSSCJobExperienceItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid FSSCAuditorActivityID { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public FSSCAuditorActivityItemListDto FSSCAuditorActivity { get; set; }

        public ICollection<FSSCAuditExperienceItemListDto> FSSCAuditExperiences { get; set; }
    } // FSSCJobExperienceItemDetailDto

    public class FSSCJobExperiencePostDto
    {
        [Required]
        public Guid FSSCAuditorActivityID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCJobExperiencePostDto

    public class FSSCJobExperiencePutDto
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
    } // FSSCJobExperiencePutDto

    public class FSSCJobExperienceDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCJobExperienceDeleteDto
}