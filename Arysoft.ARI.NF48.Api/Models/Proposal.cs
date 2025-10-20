using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Proposal : BaseModel
    {
        public Guid AuditCycleID { get; set; }

        public string Justification { get; set; }

        public string SignerName { get; set; }

        public string SignerPosition { get; set; }

        public string SignedFilename { get; set; }

        public CurrencyCodeType? CurrencyCode { get; set; }

        // INTERNAL

        public string CreatedBy { get; set; }       // Usuario que crea la Propuesta
        
        public DateTime? ReviewDate { get; set; }   // Fecha en que se envió a revisión por parte del creador o editor

        public DateTime? ApprovalDate { get; set; } // Fecha en que se aprueba la Propuesta por parte del revisor y esta lista para enviarse a firma

        public DateTime? SignRequestDate { get; set; }  // Fecha en que se envió la solicitud de firma al cliente

        public string HistoricalDataJSON { get; set; }

        public new ProposalStatusType Status { get; set; }

        // RELATIONS

        public virtual AuditCycle AuditCycle { get; set; }

        public virtual ICollection<ADC> ADCs { get; set; }

        public virtual ICollection<ProposalAudit> ProposalAudits { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        public List<ProposalAlertType> Alerts { get; set; }
    }
}