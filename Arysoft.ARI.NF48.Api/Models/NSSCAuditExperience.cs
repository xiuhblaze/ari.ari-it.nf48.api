using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class NSSCAuditExperience : BaseModel
    {
        public Guid NSSCAuditorActivityID { get; set; }

        public Guid? NSSCJobExperienceID { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual NSSCAuditorActivity NSSCAuditorActivity { get; set; }

        public virtual NSSCJobExperience NSSCJobExperience { get; set; }
    }
}