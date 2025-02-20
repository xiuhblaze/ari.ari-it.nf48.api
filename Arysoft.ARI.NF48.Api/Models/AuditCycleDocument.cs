using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AuditCycleDocument : BaseModel
    {
        public Guid AuditCycleID { get; set; }

        public Guid? StandardID { get; set; }

        public string Filename { get; set; }

        public string Version { get; set; } // Para la versión del ISO, como ejemplo FSSC 5.1 y 6.0

        public string Comments { get; set; }

        public AuditCycleDocumentType? DocumentType { get; set; }

        public string OtherDescription { get; set; }

        // RELATIONS

        public virtual AuditCycle AuditCycle { get; set; }

        public virtual Standard Standard { get; set; }
    } // AuditCycleDocument
}