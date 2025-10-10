using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class ProposalAuditItemDto
    {
        public Guid ID { get; set; }

        public Guid ProposalID { get; set; }

        public AuditStepType? AuditStep { get; set; }

        public decimal? TotalAuditDays { get; set; }

        public decimal? CertificateIssue { get; set; }

        public decimal? TotalCost { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }
    }

    public class ProposalAuditCreateDto
    {
        [Required(ErrorMessage = "The proposal ID is requierd")]
        public Guid? ProposalID { get; set; }

        public AuditStepType? AuditStep { get; set; }

        [Required(ErrorMessage = "The User that creates is required")]
        [StringLength(50, ErrorMessage = "The User name must be less than 50 characters")]
        public string UpdatedUser { get; set; }
    }

    public class ProposalAuditUpdateDto
    { 
        [Required(ErrorMessage = "The ID is requierd")]
        public Guid? ID { get; set; }
                
        public decimal? TotalAuditDays { get; set; }
        
        public decimal? CertificateIssue { get; set; }
        
        public decimal? TotalCost { get; set; }
        
        [Required(ErrorMessage = "The User that updates is required")]
        [StringLength(50, ErrorMessage = "The User name must be less than 50 characters")]
        public string UpdatedUser { get; set; }
    }

    public class ProposalAuditDeleteDto
    { 
        [Required(ErrorMessage = "The ID is requierd")]
        public Guid? ID { get; set; }

        [Required(ErrorMessage = "The User that deletes is required")]
        [StringLength(50, ErrorMessage = "The User name must be less than 50 characters")]
        public string UpdatedUser { get; set; }
    }
}