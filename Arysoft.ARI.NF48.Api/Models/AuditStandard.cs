using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AuditStandard : BaseModel
    {
        public Guid AuditID { get; set; }

        public Guid? StandardID { get; set; }

        public AuditStepType? Step { get; set; }

        public string ExtraInfo { get; set; }

        // RELATIONS

        public virtual Audit Audit { get; set; }

        public virtual Standard Standard { get; set; }

        public virtual ICollection<AuditAuditor> AuditAuditors { get; set; }

        public virtual ICollection<AuditDocument> AuditDocuments { get; set; }
    }
}