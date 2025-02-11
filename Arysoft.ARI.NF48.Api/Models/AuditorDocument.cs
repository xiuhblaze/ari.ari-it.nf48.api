using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AuditorDocument : BaseModel
    {
        public Guid AuditorID { get; set; }

        public Guid CatAuditorDocumentID { get; set; }

        public string Filename { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string Observations { get; set; }

        public AuditorDocumentType? Type { get; set; }

        //public StatusType Status { get; set; }

        // RELATIONS

        public virtual Auditor Auditor { get; set; }

        public virtual CatAuditorDocument CatAuditorDocument { get; set; }

        // NOT MAPPED

        public AuditorDocumentValidityType? ValidityStatus { get; set; }
    }
}