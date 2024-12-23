using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class NSSCSubCategory : BaseModel
    {
        public Guid NSSCCategoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual NSSCCategory NSSCCategory { get; set; }

        public virtual ICollection<NSSCActivity> NSSCActivities { get; set; }
    }
}