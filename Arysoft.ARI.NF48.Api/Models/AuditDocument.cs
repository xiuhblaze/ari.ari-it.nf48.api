using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AuditDocument : BaseModel
    {
        public Guid AuditID { get; set; }

        public Guid? StandardID { get; set; }

        public string Filename { get; set; }

        public string Comments { get; set; }

        public AuditDocumentType DocumentType { get; set; }

        public string OtherDescription { get; set; }

        // RELATIONS

        public virtual Audit Audit { get; set; }

        public virtual Standard Standard { get; set; }
    }
}