using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class FSSCSubCategory : BaseModel
    {
        public Guid FSSCCategoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        //public StatusType Status { get; set; }

        // RELATIONS

        public virtual FSSCCategory FSSCCategory { get; set; }

        public virtual ICollection<FSSCActivity> FSSCActivities { get; set; }
    }
}