using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Category22K : BaseModel
    {
        public string Cluster { get; set; }

        public string Category { get; set; }

        public string CategoryDescription { get; set; }

        public string SubCategory { get; set; }

        public string SubCategoryDescription { get; set; }

        public string Examples { get; set; }

        public bool IsAccredited { get; set; }

        public StatusType Status { get; set; }
    }
}