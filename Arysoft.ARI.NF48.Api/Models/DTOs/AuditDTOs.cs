using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class AuditItemListDto
    {
        public Guid ID { get; set; }

        public Guid? OrganizationID { get; set; }

        //public Guid AuditCycleID { get; set; }

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsMultisite { get; set; }

        public string Days { get; set; }

        public bool? IncludeSaturday { get; set; }

        public bool? IncludeSunday { get; set; }

        public string ExtraInfo { get; set; }

        public AuditStatusType Status { get; set; }

        // RELATIONS

        public string OrganizationName { get; set; }

        public string AuditCycleName { get; set; }

        public int AuditorsCount { get; set; }

        public IEnumerable<AuditAuditorItemListDto> Auditors { get; set; }

        public IEnumerable<AuditStandardItemListDto> Standards { get; set; }

        public int DocumentsCount { get; set; }

        public int NotesCount { get; set; }

        public int SitesCount { get; set; }

    } // AuditItemListDto

    public class AuditItemDetailDto
    { 
        public Guid ID { get; set; }

        public Guid? OrganizationID { get; set; }

        //public Guid AuditCycleID { get; set; }

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        //public bool? HasWitness { get; set; }

        public bool? IsMultisite { get; set; }

        public string Days { get; set; }

        public bool? IncludeSaturday { get; set; }

        public bool? IncludeSunday { get; set; }

        public string ExtraInfo { get; set; }

        public AuditStatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public OrganizationItemListDto Organization { get; set; }

        // public AuditCycleItemListDto AuditCycle { get; set; }

        public IEnumerable<AuditAuditorItemListDto> Auditors { get; set; }

        public IEnumerable<AuditDocumentItemListDto> Documents { get; set; }

        public IEnumerable<NoteItemDto> Notes { get; set; }

        public IEnumerable<AuditStandardItemListDto> Standards { get; set; }

        public IEnumerable<SiteItemListDto> Sites { get; set; }
    } // AuditItemDetailDto

    public class AuditPostDto
    {
        [Required]
        public Guid OrganizationID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditPostDto

    public class AuditPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }

        public bool? IsMultisite { get; set; }

        //public bool? HasWitness { get; set; }

        [StringLength(5)]
        public string Days { get; set; }

        public bool? IncludeSaturday { get; set; }

        public bool? IncludeSunday { get; set; }

        [StringLength(1000)]
        public string ExtraInfo { get; set; }

        [Required]
        public AuditStatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditPutDto

    public class AuditDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditDeleteDto

    public class AuditorInAuditDto
    {
        [Required]
        public Guid AuditorID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public Guid? AuditExceptionID { get; set; }
    }

    public class StandardStepInAuditCycleDto
    {

        [Required] 
        public Guid AuditCycleID { get; set; }

        [Required]
        public Guid StandardID { get; set; }

        [Required] 
        public AuditStepType Step { get; set; }

        public Guid? AuditExceptionID { get; set; }
    }

    public class NextAuditDto
    { 
        [Required]
        public Guid OwnerID { get; set; }

        /// <summary>
        /// A partír de esta fecha buscar la siguiente auditoría
        /// </summary>
        public DateTime? InitialDate { get; set; }

        [Required]
        public AuditNextAuditOwnerType Owner { get; set; }
    }

    // SITES

    /// <summary>
    /// Para agregar o quitar un sitio de una auditoria
    /// </summary>
    public class AuditEditSiteDto
    {
        [Required]
        public Guid AuditID { get; set; }

        [Required]
        public Guid SiteID { get; set; }        
    } // AuditEditSiteDto
}