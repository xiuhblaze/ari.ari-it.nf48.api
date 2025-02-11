using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class FSSCAuditorActivity : BaseModel
    {
        public Guid AuditorID { get; set; }

        public Guid FSSCActivityID { get; set; }

        public string Education { get; set; }

        public string LegalRequirements { get; set; }

        public string SpecificTraining { get; set; }

        public string Comments { get; set; }

        //public StatusType Status { get; set; }

        // RELATIONS

        public virtual Auditor Auditor { get; set; }

        public virtual FSSCActivity FSSCActivity { get; set; }

        public virtual ICollection<FSSCJobExperience> FSSCJobExperiences { get; set; }

        public virtual ICollection<FSSCAuditExperience> FSSCAuditExperiences { get; set; }
    }
}