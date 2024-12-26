using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class NSSCAuditorActivity : BaseModel
    {
        public Guid AuditorID { get; set; }

        public Guid NSSCActivityID { get; set; }

        public string Education { get; set; }

        public string LegalRequirements { get; set; }

        public string SpecificTraining { get; set; }

        public string Comments { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual Auditor Auditor { get; set; }

        public virtual NSSCActivity NSSCActivity { get; set; }

        public virtual ICollection<NSSCJobExperience> NSSCJobExperiences { get; set; }

        public virtual ICollection<NSSCAuditExperience> NSSCAuditExperiences { get; set; }
    }
}