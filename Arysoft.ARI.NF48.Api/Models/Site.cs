using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models
{
    [Table("Sites")]
    public class Site : BaseModel
    {
        [Key]
        public Guid SiteID { get; set; }

        public Guid OrganizationID { get; set; }

        //public Guid LocationID { get; set; } // próximamente

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(500)]
        public string LocationDescription { get; set; }

        /// <summary>
        /// Order in the organization's hierarchy, 1 is the main site.
        /// </summary>
        public int Order { get; set; }

        // RELATIONS

        public Organization Organization { get; set; }

        //public Location Location { get; set; } // próximamente

        public ICollection<Shift> Shifts { get; set; }

    }
}