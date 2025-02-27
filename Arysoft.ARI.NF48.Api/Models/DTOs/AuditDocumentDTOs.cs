using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class AuditDocumentItemListDto
    {
        public Guid ID { get; set; }

        public Guid AuditID { get; set; }

        // public Guid? StandardID { get; set; }

        public string Filename { get; set; }

        public string Comments { get; set; }

        public AuditDocumentType? DocumentType { get; set; }

        public string OtherDescription { get; set; }

        public string UploadedBy { get; set; }

        public bool? IsWitnessIncluded { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public string AuditDescription { get; set; }

        // public string StandardName { get; set; }

        public IEnumerable<string> StandardsNames { get; set; }

    } // AuditDocumentItemListDto

    public class AuditDocumentItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid AuditID { get; set; }

        // public Guid? StandardID { get; set; }

        public string Filename { get; set; }

        public string Comments { get; set; }

        public AuditDocumentType? DocumentType { get; set; }

        public string OtherDescription { get; set; }

        public string UploadedBy { get; set; }

        public bool? IsWitnessIncluded { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public AuditItemListDto Audit { get; set; }

        // public StandardItemListDto Standard { get; set; }

        public IEnumerable<AuditStandardItemListDto> AuditStandards { get; set; }
    } // AuditDocumentItemDetailDto

    public class AuditDocumentPostDto
    {
        [Required]
        public Guid AuditID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditDocumentPostDto

    public class AuditDocumentAddAuditStandardDto
    {
        [Required]
        public Guid AuditDocumentID { get; set; }

        [Required]
        public Guid AuditStandardID { get; set; }        
    } // AuditDocumentAddAuditStandardDto

    public class AuditDocumentPutDto
    {
        [Required]
        public Guid ID { get; set; }

        public Guid? StandardID { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        [Required]
        public AuditDocumentType? DocumentType { get; set; }

        [StringLength(100)]
        public string OtherDescription { get; set; }

        [StringLength(50)]
        public string UploadedBy { get; set; }

        public bool? IsWitnessIncluded { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditDocumentPutDto

    public class AuditDocumentDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditDocumentDeleteDto
}