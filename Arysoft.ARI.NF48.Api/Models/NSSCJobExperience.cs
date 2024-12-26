using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class NSSCJobExperience : BaseModel
    {
        public Guid NSSCAuditorActivityID { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual NSSCAuditorActivity NSSCAuditorActivity { get; set; }

        public virtual ICollection<NSSCAuditExperience> NSSCAuditExperiences { get; set; }
    }
}