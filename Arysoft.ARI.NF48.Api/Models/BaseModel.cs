using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models
{
    public abstract class BaseModel
    {
        [DataType(DataType.DateTime)]
        public DateTime Updated { get; set; }

        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}