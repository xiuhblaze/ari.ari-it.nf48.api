using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class CalendarBlock : BaseModel
    {
        public Guid? OwnerID { get; set; }

        public CalendarBlockType BlockType { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public CalendarBlockOwnerType OwnerType { get; set; }

        public bool AllowOverride { get; set; }

    } // CalendarBlock
}