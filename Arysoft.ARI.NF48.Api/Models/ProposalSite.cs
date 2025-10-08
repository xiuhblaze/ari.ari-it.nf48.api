using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class ProposalSite : BaseModel 
    {
        public Guid ProposalID { get; set; }

        public Guid SiteID { get; set; }

        public int NoEmployees { get; set; }

        public decimal? CertificateIssue { get; set; }

        public decimal? TotalCost { get; set; }

        public string AuditSteps { get; set; }

        // RELATIONS

        //public virtual Proposal Proposal { get; set; }

        public virtual Site Site { get; set; }
    }
}