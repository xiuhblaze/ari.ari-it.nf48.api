using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.ComTypes;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class FSSCAuditorActivityItemListDto
    {
        public Guid ID { get; set; }

        public Guid AuditorID { get; set; }

        public Guid FSSCActivityID { get; set; }

        public string Education { get; set; }

        public string LegalRequirements { get; set; }

        public string SpecificTraining { get; set; }

        public string Comments { get; set; }

        public StatusType Status { get; set; }

        // From relations

        public string AuditorFullName { get; set; }

        public string Activity { get; set; } // considerar la subcategory y category

        public int FSSCJobExperiencesCount { get; set; }

        public int FSSCAuditExperienceSCount { get; set; }

    } // FSSCAuditorActivityItemListDto

    public class FSSCAuditorActivityItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid AuditorID { get; set; }

        public Guid FSSCActivityID { get; set; }

        public string Education { get; set; }

        public string LegalRequirements { get; set; }

        public string SpecificTraining { get; set; }

        public string Comments { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // Relations

        public AuditorItemListDto Auditor { get; set; }

        public FSSCActivityItemListDto FSSCActivity { get; set; }

        public IEnumerable<FSSCJobExperienceItemListDto> FSSCJobExperiences { get; set; }

        public IEnumerable<FSSCAuditExperienceItemListDto> FSSCAuditExperiences { get; set; }
    } // FSSCAuditorActivityItemDetailDto

    public class FSSCAuditorActivityPostDto
    {
        [Required]
        public Guid AuditorID { get; set; }

        [Required]
        public Guid FSSCActivityID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCAuditorActivityPostDto

    public class FSSCAuditorActivityPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [StringLength(1000)]
        public string Education { get; set; }

        [StringLength(1000)]
        public string LegalRequirements { get; set; }

        [StringLength(1000)]
        public string SpecificTraining { get; set; }

        [StringLength(1000)]
        public string Comments { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCAuditorActivityPutDto

    public class FSSCAuditorActivityDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // FSSCAuditorActivityDeleteDto
}