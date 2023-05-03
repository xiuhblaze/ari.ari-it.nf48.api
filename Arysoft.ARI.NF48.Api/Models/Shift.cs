using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models
{
    [Table("Shifts")]
    public class Shift : BaseModel
    {
        [Key]
        public Guid ShiftID { get; set; }

        public Guid SiteID { get; set; }

        public ShiftType Type { get; set; }

        public int? NoEmployees { get; set; }

        /// <summary>
        /// Horario de turno, en horas (24hrs)
        /// </summary>
        public int? ShiftBegin { get; set; }

        public int? ShiftEnd { get; set; }

        [StringLength(500)]
        public string ActivitiesDescription { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public Site Site { get; set; }
    }
}