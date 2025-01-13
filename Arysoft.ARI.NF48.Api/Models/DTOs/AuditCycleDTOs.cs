using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class AuditCycleItemListDto
    {
        public Guid ID { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public string OrganizationName { get; set; }

        public int AuditsCount { get; set; }

        // public IEnumerable<AuditCycleStandardItemListDto> AuditCycleStandards { get; set; }

        public int DocumentsCount { get; set; }
    } // AuditCycleItemListDto

    public class AuditCycleItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid OrganizationID { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public OrganizationItemListDto Organization { get; set; }

        // public IEnumerable<AuditItemListDto> Audits { get; set; }

        public IEnumerable<AuditCycleStandardItemListDto> AuditCycleStandards { get; set; }

        // public IEnumerable<AuditCycleDocumentItemListDto> AuditCycleDocuments { get; set; }
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
        public DateTime? StartDate { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }

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