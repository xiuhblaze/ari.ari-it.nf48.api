using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AuditDocument : BaseModel
    {
        public Guid AuditID { get; set; }

        public Guid? StandardID { get; set; }

        public string Filename { get; set; }

        public string Comments { get; set; }

        public AuditDocumentType? DocumentType { get; set; }

        public string OtherDescription { get; set; }

        public string UploadedBy { get; set; }

        public bool? IsWitnessIncluded { get; set; }

        // RELATIONS

        public virtual Audit Audit { get; set; }

        // public virtual Standard Standard { get; set; }

        public virtual ICollection<AuditStandard> AuditStandards { get; set; } 
    }
} 