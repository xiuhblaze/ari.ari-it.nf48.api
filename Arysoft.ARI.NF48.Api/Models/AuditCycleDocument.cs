using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AuditCycleDocument : BaseModel
    {
        public Guid OrganizationID { get; set; }

        //public Guid AuditCycleID { get; set; }

        //public Guid? StandardID { get; set; } // Este es posible que no se necesite o solo informativo

        public string Filename { get; set; }

        public string Version { get; set; } // Para la versión del ISO, como ejemplo FSSC 5.1 y 6.0

        public string Comments { get; set; }

        public AuditCycleDocumentType? DocumentType { get; set; }

        public string OtherDescription { get; set; }

        public string UploadedBy { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        // public virtual AuditCycle AuditCycle { get; set; }

        // public virtual Standard Standard { get; set; }

        public virtual ICollection<AuditCycle> AuditCycles { get; set; }
    } // AuditCycleDocument
}