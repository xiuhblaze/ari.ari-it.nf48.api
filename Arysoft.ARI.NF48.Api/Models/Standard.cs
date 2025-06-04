using Arysoft.ARI.NF48.Api.Enumerations;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Standard : BaseModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? MaxReductionDays { get; set; }

        public int? SalesMaxReductionDays { get; set; }

        public StandardBaseType? StandardBase { get; set; }

        // RELATIONS

        // public virtual ICollection<Application> Applications { get; set; }

        public virtual ICollection<AuditorStandard> AuditorStandards { get; set; }

        public virtual ICollection<CatAuditorDocument> CatAuditorDocuments { get; set; }

        public virtual ICollection<Certificate> Certificates { get; set; }

        public virtual ICollection<OrganizationStandard> OrganizationStandards { get; set; }
    }
}