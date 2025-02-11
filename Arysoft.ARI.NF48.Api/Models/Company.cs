using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Company : BaseModel
    {
        public Guid OrganizationID { get; set; }

        public string Name { get; set; } // El nombre legal de la empresa

        public string LegalEntity { get; set; } // RFC o código de identificación fiscal del país

        public string COID { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }
    }
}