using Arysoft.ARI.NF48.Api.Enumerations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Organization : BaseModel
    {
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string LegalEntity { get; set; }

        [StringLength(250)]
        public string LogoFile { get; set; }

        [StringLength(250)]
        public string Website { get; set; }

        [StringLength(10)]
        public string Phone { get; set; }

        public OrganizationStatusType Status { get; set; }

        // RELATIONS

        public ICollection<Application> Applications { get; set; }

        public ICollection<Contact> Contacts { get; set; }

        public ICollection<Site> Sites { get; set; }

    }
}