using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class CertificateItemListDto
    {
        public Guid ID { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string Comments { get; set; }

        public string Filename { get; set; }

        public string QRFile { get; set; }

        public DateTime? PrevAuditDate { get; set; }

        public string PrevAuditNote { get; set; }

        public DateTime? NextAuditDate { get; set; }

        public string NextAuditNote { get; set; }

        public bool? HasNCsMinor { get; set; }

        public bool? HasNCsMajor { get; set; }

        public bool? HasNCsCritical { get; set; }

        public DateTime? ActionPlanDate { get; set; }

        public bool? ActionPlanDelivered { get; set; }

        public CertificateStatusType Status { get; set; }

        // RELATIONS

        public string OrganizationName { get; set; }

        public string StandardName { get; set; }

        public int NotesCount { get; set; }

        // CALCULATED

        public CertificateValidityStatusType ValidityStatus { get; set; }

        public DefaultValidityStatusType AuditPlanValidityStatus { get; set; }

    } // CertificateItemListDto

    public class CertificateItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid OrganizationID { get; set; }

        public Guid? StandardID { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string Comments { get; set; }

        public string Filename { get; set; }

        public string QRFile { get; set; }

        public DateTime? PrevAuditDate { get; set; }

        public string PrevAuditNote { get; set; }

        public DateTime? NextAuditDate { get; set; }

        public string NextAuditNote { get; set; }

        public bool? HasNCsMinor { get; set; }

        public bool? HasNCsMajor { get; set; }

        public bool? HasNCsCritical { get; set; }

        public DateTime? ActionPlanDate { get; set; }

        public bool? ActionPlanDelivered { get; set; }

        public CertificateStatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public OrganizationItemListDto Organization { get; set; }

        public StandardItemListDto Standard { get; set; }

        public IEnumerable<NoteItemDto> Notes { get; set; }

        // CALCULATED

        public CertificateValidityStatusType ValidityStatus { get; set; }

        public DefaultValidityStatusType AuditPlanValidityStatus { get; set; }

    } // CertificateItemDetailDto 

    public class CertificatePostDto
    {
        [Required]
        public Guid OrganizationID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // CertificatePostDto

    public class CertificatePutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public Guid StandardID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        public DateTime? PrevAuditDate { get; set; }

        [StringLength(100)]
        public string PrevAuditNote { get; set; }

        public DateTime? NextAuditDate { get; set; }

        [StringLength(100)]
        public string NextAuditNote { get; set; }

        public bool? HasNCsMinor { get; set; }

        public bool? HasNCsMajor { get; set; }

        public bool? HasNCsCritical { get; set; }

        public DateTime? ActionPlanDate { get; set; }

        public bool? ActionPlanDelivered { get; set; }

        [Required]
        public CertificateStatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // CertificatePutDto

    public class CertificateDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // CertificateDeleteDto
}