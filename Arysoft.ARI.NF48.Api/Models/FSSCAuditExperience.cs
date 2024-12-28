using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class FSSCAuditExperience : BaseModel
    {
        public Guid FSSCAuditorActivityID { get; set; }

        public Guid? FSSCJobExperienceID { get; set; }

        public string Description { get; set; }

        //public StatusType Status { get; set; }

        // RELATIONS

        public virtual FSSCAuditorActivity FSSCAuditorActivity { get; set; }

        public virtual FSSCJobExperience FSSCJobExperience { get; set; }
    }
}