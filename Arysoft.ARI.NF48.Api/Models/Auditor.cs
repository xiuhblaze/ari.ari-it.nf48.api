using Arysoft.ARI.NF48.Api.Enumerations;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Auditor : BaseModel
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string PhotoFilename { get; set; }

        public decimal FeePayment { get; set; }

        public bool IsLeadAuditor { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual ICollection<AuditorDocument> Documents { get; set;  }

        public virtual ICollection<NSSCAuditorActivity> NSSCAuditorActivities { get; set; }

    }
}