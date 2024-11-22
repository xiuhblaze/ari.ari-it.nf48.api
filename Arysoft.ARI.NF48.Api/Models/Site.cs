using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Site : BaseModel
    {
        public Guid OrganizationID { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public bool IsMainSite { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual ICollection<Shift> Shifts { get; set; }
    }
}