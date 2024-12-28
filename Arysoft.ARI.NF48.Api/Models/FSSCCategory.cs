using Arysoft.ARI.NF48.Api.Enumerations;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class FSSCCategory : BaseModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        //public StatusType Status { get; set; }

        // RELATIONS

        public virtual ICollection<FSSCSubCategory> FSSCSubCategories { get; set; }
    }
}