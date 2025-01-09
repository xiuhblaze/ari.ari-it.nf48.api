using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AuditCycleStandard : BaseModel
    {
        public Guid AuditCycleID { get; set; }

        public Guid StandardID { get; set; }


    }
}