using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Shift : BaseModel
    {
        public Guid SiteID { get; set; }

        public ShiftType? Type { get; set; }

        public int? NoEmployees { get; set; }

        public DateTime? ShiftBegin { get; set; }

        public DateTime? ShiftEnd { get; set; }

        public string ActivitiesDescription { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual Site Site { get; set; }
    }
}