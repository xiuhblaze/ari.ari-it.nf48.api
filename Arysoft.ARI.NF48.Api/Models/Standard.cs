using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models
{
    [Table("Standards")]
    public class Standard : BaseModel
    {
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public int? MaxReductionsDays { get; set; } 

        public StatusType Status { get; set; }
    }
}