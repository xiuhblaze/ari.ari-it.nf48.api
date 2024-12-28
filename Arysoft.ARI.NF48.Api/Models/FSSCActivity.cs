using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class FSSCActivity : BaseModel
    {
        public Guid FSSCSubCategoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        //public StatusType Status { get; set; }

        // RELATIONS

        public virtual FSSCSubCategory FSSCSubCategory { get; set; }

        public virtual ICollection<FSSCAuditorActivity> FSSCAuditorActivities { get; set; }
    }
}