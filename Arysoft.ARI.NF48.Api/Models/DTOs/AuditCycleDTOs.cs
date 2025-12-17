using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Arysoft.ARI.NF48.Api.Attributes;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class AuditCycleItemListDto
    {
        public Guid ID { get; set; }

        public Guid OrganizationID { get; set; }

        public Guid? StandardID { get; set; }

        public string Name { get; set; }

        public AuditCycleType? CycleType { get; set; }

        public AuditStepType? InitialStep { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public AuditCyclePeriodicityType? Periodicity { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public string OrganizationName { get; set; }

        public string StandardName { get; set; }

        public int AuditsCount { get; set; }

        //public IEnumerable<AuditCycleStandardItemListDto> AuditCycleStandards { get; set; }

        public int AuditCycleDocumentsCount { get; set; } 
    } // AuditCycleItemListDto

    public class AuditCycleItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid OrganizationID { get; set; }

        public Guid? StandardID { get; set; }

        public string Name { get; set; }

        public AuditCycleType? CycleType { get; set; }

        public AuditStepType? InitialStep { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public AuditCyclePeriodicityType? Periodicity { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public OrganizationItemListDto Organization { get; set; }

        public StandardItemListDto Standard { get; set; }

        public IEnumerable<AuditStandardItemListDto> AuditStandards { get; set; }

        // public IEnumerable<AuditItemListDto> Audits { get; set; }

        // public IEnumerable<AuditCycleStandardItemListDto> AuditCycleStandards { get; set; }

        public IEnumerable<AuditCycleDocumentItemListDto> AuditCycleDocuments { get; set; }
    } // AuditCycleItemDetailDto

    public class AuditCyclePostDto
    {
        [Required]
        public Guid OrganizationID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditCyclePostDto

    public class AuditCyclePutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public Guid? StandardID { get; set; }

        [StringLength(50, ErrorMessage = "The audit cycle name ")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The cycle type is required")]
        [ValidEnumValue(typeof(AuditCycleType))]
        public AuditCycleType? CycleType { get; set; }

        [Required(ErrorMessage = "The initial step is required")]
        [ValidEnumValue(typeof(AuditStepType))]
        public AuditStepType? InitialStep { get; set; }

        public DateTime? StartDate { get; set; }    // Obligatorio hasta que exista el certificado

        public DateTime? EndDate { get; set; }

        [ValidEnumValue(typeof(AuditCyclePeriodicityType))]
        public AuditCyclePeriodicityType? Periodicity { get; set; }

        [StringLength(1000, ErrorMessage = "The extra info must be less than 1000 characters")]
        public string ExtraInfo { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditCyclePutDto

    public class AuditCycleDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditCycleDeleteDto
}