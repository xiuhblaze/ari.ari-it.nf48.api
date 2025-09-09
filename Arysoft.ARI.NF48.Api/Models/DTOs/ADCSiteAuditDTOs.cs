using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class ADCSiteAuditItemDto
    {
        public Guid ID { get; set; }

        public Guid ADCSiteID { get; set; }

        public bool? Value { get; set; }

        public AuditStepType? AuditStep { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }
    } // ADCSiteAuditItemDetailDto

    public class ADCSiteAuditCreateDto
    {
        [Required]
        public Guid ADCSiteID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCSiteAuditCreateDto

    public class ADCSiteAuditUpdateDto
    {
        [Required]
        public Guid ID { get; set; }

        public bool? Value { get; set; }
        
        public AuditStepType? AuditStep { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCSiteAuditUpdateDto

    public class ADCSiteAuditListUpdateDto
    { 
        [Required]
        public List<ADCSiteAuditUpdateDto> Items { get; set; }
    } // ADCSiteAuditListUpdateDto

    public class ADCSiteAuditDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ADCSiteAuditDeleteDto
}