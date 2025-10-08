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

        public string ActivitiesScope { get; set; } // Para detectar si se modifica el alcance

        public int? TotalEmployees { get; set; } // Para detectar si se modifica el número de empleados

        public string Justification { get; set; }

        public string SignerName { get; set; }

        public string SignerPosition { get; set; }

        public DateTime? SendToSignDate { get; set; }

        public string SigendFilename { get; set; }

        public CurrencyCodeType? CurrencyCode { get; set; }

        public string UserCreates { get; set; }     // Usuario que crea la Propuesta

        public string UserReview { get; set; }      // Usuario que revisa y aprueba la Propuesta

        public DateTime? ReviewDate { get; set; }   // Fecha en que se envió a revisión por parte del creador o editor

        public DateTime? ActiveDate { get; set; }   // Fecha de activación de la Propuesta por parte del revisor

        public new ProposalStatusType Status { get; set; }

        // INTERNAL

        public string HistoricalDataJSON { get; set; }

        // RELATIONS

        public virtual AuditCycle AuditCycle { get; set; }

        public virtual AppForm AppForm { get; set; }

        public virtual ADC ADC { get; set; }

        public virtual MD5 MD5 { get; set; }

        // public virtual ICollection<ProposalSite> ProposalSites { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        public List<ProposalAlertType> Alerts { get; set; }
    }
}