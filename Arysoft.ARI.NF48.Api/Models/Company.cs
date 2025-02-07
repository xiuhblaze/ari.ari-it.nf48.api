using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Company : BaseModel
    {
        public Guid OrganizationID { get; set; }

        public string Name { get; set; }

        public string LegalEntity { get; set; }

        public string COID { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }
    }
}