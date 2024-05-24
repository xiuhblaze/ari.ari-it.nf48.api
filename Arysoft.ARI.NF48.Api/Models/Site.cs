using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Site : BaseModel
    {
        public Guid OrganizationID { get; set; }

        //public Guid LocationID { get; set; } // next

        public string Description { get; set; }

        public string LocationDescription { get; set; }

        /// <summary>
        /// Order in the organization's hierarchy, 1 is the main site.
        /// </summary>
        public int Order { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual ICollection<Shift> Shifts { get; set; }

        //public Location Location { get; set; } // próximamente
    }
}