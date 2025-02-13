using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class AuditAuditorItemListDto
    {
        public Guid ID { get; set; }

        public Guid AuditID { get; set; }

        public Guid? AuditorID { get; set; }

        public bool? IsLeader { get; set; }

        public bool? IsWitness { get; set; }

        public string Comments { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public string AuditDescription { get; set; }

        public string AuditorName { get; set; }
    } // AuditAuditorItemListDto

    public class AuditAuditorItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid AuditID { get; set; }

        public Guid? AuditorID { get; set; }

        public bool? IsLeader { get; set; }

        public bool? IsWitness { get; set; }

        public string Comments { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public AuditItemListDto Audit { get; set; }

        public AuditorItemListDto Auditor { get; set; }
    } // AuditAuditorItemDetailDto

    public class AuditAuditorPostDto
    {
        [Required]
        public Guid AuditID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditAuditorPostDto

    public class AuditAuditorPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public Guid? AuditorID { get; set; }

        public bool? IsLeader { get; set; }

        public bool? IsWitness { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditAuditorPutDto

    public class AuditAuditorDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditAuditorDeleteDto
}