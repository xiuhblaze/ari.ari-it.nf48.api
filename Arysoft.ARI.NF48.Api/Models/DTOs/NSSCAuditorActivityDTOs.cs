using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.ComTypes;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class NSSCAuditorActivityItemListDto
    {
        public Guid ID { get; set; }

        public Guid AuditorID { get; set; }

        public Guid NSSCActivityID { get; set; }

        public string Education { get; set; }

        public string LegalRequirements { get; set; }

        public string SpecificTraining { get; set; }

        public string Comments { get; set; }

        public StatusType Status { get; set; }

        // From relations

        public string AuditorFullName { get; set; }

        public string Activity { get; set; } // considerar la subcategory y category

        public int NSSCJobExperiencesCount { get; set; }

        public int NSSCAuditExperienceSCount { get; set; }

    } // NSSCAuditorActivityItemListDto

    public class NSSCAuditorActivityItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid AuditorID { get; set; }

        public Guid NSSCActivityID { get; set; }

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

        public NSSCActivityItemListDto NSSCActivity { get; set; }

        public IEnumerable<NSSCJobExperienceItemListDto> NSSCJobExperiences { get; set; }

        public IEnumerable<NSSCAuditExperienceItemListDto> NSSCAuditExperiences { get; set; }
    } // NSSCAuditorActivityItemDetailDto

    public class NSSCAuditorActivityPostDto
    {
        [Required]
        public Guid AuditorID { get; set; }

        [Required]
        public Guid NSSCActivityID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCAuditorActivityPostDto

    public class NSSCAuditorActivityPutDto
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
    } // NSSCAuditorActivityPutDto

    public class NSSCAuditorActivityDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NSSCAuditorActivityDeleteDto
}