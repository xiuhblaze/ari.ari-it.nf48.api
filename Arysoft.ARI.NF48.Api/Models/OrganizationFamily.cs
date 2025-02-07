using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class OrganizationFamily : BaseModel
    {
        public string Description { get; set; }

        // RELATIONS

        public virtual ICollection<Organization> Organizations { get; set; }
    }
}