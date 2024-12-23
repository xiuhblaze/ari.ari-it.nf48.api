using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class NSSCActivity : BaseModel
    {
        public Guid NSSCSubCategoryID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual NSSCSubCategory NSSCSubCategory { get; set; }
    }
}