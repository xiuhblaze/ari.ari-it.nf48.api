using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Proposal : BaseModel
    {
        public Guid AuditCycleID { get; set; }

        public Guid AppFormID { get; set; }

        public Guid ADCID { get; set; }

        public Guid? MD5ID { get; set; }

        public string ActivitiesScope { get; set; }

        public int? TotalEmployees { get; set; }

        public string Justification { get; set; }

        public string SignerName { get; set; }

        public string SignerPosition { get; set; }

        public string SigendFilename { get; set; }

        public CurrencyCodeType? CurrencyCode { get; set; }

        public string HistoricalDataJSON { get; set; }

        public new ProposalStatusType Status { get; set; }

        // RELATIONS

        public virtual AuditCycle AuditCycle { get; set; }

        public virtual AppForm AppForm { get; set; }

        public virtual ADC ADC { get; set; }

        public virtual MD5 MD5 { get; set; }

        public virtual ICollection<ProposalSite> ProposalSites { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
    }
}