using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Shift : BaseModel
    {
        public Guid SiteID { get; set; }

        public ShiftType? Type { get; set; }

        public int? NoEmployees { get; set; }

        public TimeSpan? ShiftStart { get; set; }

        public TimeSpan? ShiftEnd { get; set; }

        public string ActivitiesDescription { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual Site Site { get; set; }
    }
}