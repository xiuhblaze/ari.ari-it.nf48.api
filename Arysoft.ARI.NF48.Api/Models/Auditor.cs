using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Auditor : BaseModel
    {
        public Guid PersonID { get; set; }

        public decimal FeePayment { get; set; }

        public bool IsLeadAuditor { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS
                
        // public virtual Person Person { get; set; }
    }
}