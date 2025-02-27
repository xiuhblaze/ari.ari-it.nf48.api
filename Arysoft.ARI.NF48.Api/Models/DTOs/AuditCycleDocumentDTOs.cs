using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class AuditCycleDocumentItemListDto
    {
        public Guid ID { get; set; }

        public string Filename { get; set; }

        public string Version { get; set; }

        public string Comments { get; set; }

        public AuditCycleDocumentType DocumentType { get; set; }

        public string OtherDescription { get; set; }

        public string UploadedBy { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public string StandardName { get; set; }
    } // AuditCycleDocumentItemListDto

    public class AuditCycleDocumentItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid AuditCycleID { get; set; }

        public Guid? StandardID { get; set; }

        public string Filename { get; set; }

        public string Version { get; set; }

        public string Comments { get; set; }

        public AuditCycleDocumentType DocumentType { get; set; }

        public string OtherDescription { get; set; }

        public string UploadedBy { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public AuditCycleItemListDto AuditCycle { get; set; }

        public StandardItemListDto Standard { get; set; }
    } // AuditCycleDocumentItemDetailDto

    public class AuditCycleDocumentPostDto
    {
        [Required]
        public Guid AuditCycleID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditCycleDocumentPostDto

    public class AuditCycleDocumentPutDto
    {
        [Required]
        public Guid ID { get; set; }

        public Guid? StandardID { get; set; }

        [StringLength(10)]
        public string Version { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        [Required]
        public AuditCycleDocumentType DocumentType { get; set; }

        [StringLength(100)]
        public string OtherDescription { get; set; }

        [StringLength(50)]
        public string UploadedBy { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditCycleDocumentPutDto

    public class AuditCycleDocumentDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditCycleDocumentDeleteDto
}