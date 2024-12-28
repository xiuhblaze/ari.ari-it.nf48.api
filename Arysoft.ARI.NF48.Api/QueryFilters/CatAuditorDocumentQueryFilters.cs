using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class CatAuditorDocumentQueryFilters : BaseQueryFilters
    {
        public Guid? StandardID { get; set; } // Si es Guid.Empty son documentos generales

        public string Text { get; set; }

        public CatAuditorDocumentType? DocumentType { get; set; }

        public CatAuditorDocumentSubCategoryType? SubCategory { get; set; }

        public StatusType? Status { get; set; }

        public CatAuditorDocumentOrderType? Order { get; set; }
    }
}