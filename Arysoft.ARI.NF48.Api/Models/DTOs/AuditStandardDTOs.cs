using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class AuditStandardItemListDto
    {
        public Guid ID { get; set; }

        public Guid? AuditCycleID { get; set; }

        public Guid AuditID { get; set; }
        
        public Guid? StandardID { get; set; }

        public AuditStepType? Step { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public string AuditCycleName { get; set; }

        public string AuditDescription { get; set; }

        public string StandardName { get; set; }

        public StandardBaseType StandardBase { get; set; }

        public StatusType StandardStatus { get; set; }

        public int AuditorsCount { get; set; }

        public int DocumentsCount { get; set; }
    } // AuditStandardItemListDto

    public class AuditStandardItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid? AuditCycleID { get; set; }

        public Guid AuditID { get; set; }

        public Guid? StandardID { get; set; }

        public AuditStepType? Step { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public AuditCycleItemListDto AuditCycle { get; set; }

        public AuditItemListDto Audit { get; set; }

        public StandardItemListDto Standard { get; set; }

        public IEnumerable<AuditAuditorItemListDto> Auditors { get; set; }

        public IEnumerable<AuditDocumentItemListDto> Documents { get; set; }
    } // AuditStandardItemDetailDto

    public class AuditStandardPostDto
    {
        [Required]
        public Guid AuditID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditStandardPostDto aryadne9000

    public class AuditStandardPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public Guid? AuditCycleID { get; set; }

        //public Guid? StandardID { get; set; }

        [Required]
        public AuditStepType Step { get; set; }

        [StringLength(1000)]
        public string ExtraInfo { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditStandardPutDto

    public class AuditStandardDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}