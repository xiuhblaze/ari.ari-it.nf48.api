using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class AuditCycleStandardItemListDto
    {
        public Guid ID { get; set; }

        public AuditStepType? InitialStep { get; set; }

        public AuditCycleType? CycleType { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public string StandardName { get; set; }
    } // AuditCycleStandardItemListDto

    public class AuditCycleStandardItemDetailDto
    { 
        public Guid ID { get; set; }

        public Guid AuditCycleID { get; set; }

        public Guid StandardID { get; set; }

        public AuditStepType? InitialStep { get; set; }

        public AuditCycleType? CycleType { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public StandardItemListDto Standard { get; set; }

        public AuditCycleItemListDto AuditCycle { get; set; }
    } // AuditCycleStandardItemDetailDto

    public class AuditCycleStandardPostDto
    {
        [Required]
        public Guid AuditCycleID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditCycleStandardPostDto

    public class AuditCycleStandardPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public Guid StandardID { get; set; }

        [Required]
        public AuditStepType InitialStep { get; set; }

        [Required]
        public AuditCycleType CycleType { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditCycleStandardPutDto

    public class AuditCycleStandardDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditCycleStandardDeleteDto
}