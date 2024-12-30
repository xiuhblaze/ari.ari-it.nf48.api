using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AuditorStandard : BaseModel
    {
        public Guid AuditorID { get; set; }

        public Guid? StandardID { get; set; }

        public string Comments { get; set; }

        // RELATIONS

        public virtual Auditor Auditor { get; set; }

        public virtual Standard Standard { get; set; }
    }
}