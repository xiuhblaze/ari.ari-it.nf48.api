using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class ADCSiteAudit : BaseModel
    {
        public Guid ADCSiteID { get; set; }

        public bool? Value { get; set; }

        public AuditStepType? AuditStep { get; set; }

        // RELATIONS

        public virtual ADCSite ADCSite { get; set; }
    }
}