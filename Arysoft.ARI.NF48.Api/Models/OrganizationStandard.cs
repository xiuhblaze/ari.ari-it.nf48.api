using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class OrganizationStandard : BaseModel
    {
        public Guid OrganizationID { get; set; }

        public Guid? StandardID { get; set; }

        public string ExtraInfo { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual Standard Standard { get; set; }
    }
}