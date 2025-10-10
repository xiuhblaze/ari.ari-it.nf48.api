using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class ProposalAudit : BaseModel
    {
        public Guid ProposalID { get; set; }

        public AuditStepType? AuditStep { get; set; }

        public decimal? TotalAuditDays { get; set; }

        public decimal? CertificateIssue { get; set; }

        public decimal? TotalCost { get; set; }

        // RELATIONS

        public virtual Proposal Proposal { get; set; }
    }
}