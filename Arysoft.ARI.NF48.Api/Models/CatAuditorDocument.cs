using Arysoft.ARI.NF48.Api.Enumerations;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class CatAuditorDocument : BaseModel
    {
        public string Description { get; set; }

        public string LegalDescription { get; set; }

        public CatAuditorDocumentType? DocumentType { get; set; }

        public CatAuditorDocumentSubCategoryType? SubCategory { get; set; }

        public int? UpdateEvery { get; set; }

        public CatAuditorDocumentPeriodicityType? UpdatePeriodicity { get; set; }

        public int? WarningEvery { get; set; }

        public CatAuditorDocumentPeriodicityType? WarningPeriodicity { get; set; }

        public bool? IsRequired { get; set; } // Si el documento es obligatorio para el auditor

        public int? Order { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        public virtual ICollection<AuditorDocument> Documents { get; set; }
    }
}