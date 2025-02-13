using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AuditAuditor : BaseModel
    {
        public Guid AuditID { get; set; }

        public Guid? AuditorID { get; set; }

        public bool? IsLeader { get; set; } // Para saber si es el auditor lider de la auditoria

        public bool? IsWitness { get; set; }

        public string Comments { get; set; }

        // RELATIONS

        public virtual Audit Audit { get; set; }

        public virtual Auditor Auditor { get; set; }
    }
}