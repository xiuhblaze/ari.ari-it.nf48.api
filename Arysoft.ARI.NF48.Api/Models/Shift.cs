using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Shift : BaseModel
    {
        public Guid SiteID { get; set; }

        public ShiftType Type { get; set; }

        public int? NoEmployees { get; set; }

        /// <summary>
        /// Horario de turno, en horas (24hrs)
        /// </summary>
        public int? ShiftBegin { get; set; }

        public int? ShiftEnd { get; set; }

        public string ActivitiesDescription { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public Site Site { get; set; }
    }
}