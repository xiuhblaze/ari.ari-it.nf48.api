using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Site : BaseModel
    {
        public Guid OrganizationID { get; set; }

        public string Description { get; set; }

        public bool IsMainSite { get; set; }

        public string Address { get; set; }

        public DbGeography LocationGPS { get; set; }

        public string LocationURL { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual ICollection<Shift> Shifts { get; set; }
    }
}