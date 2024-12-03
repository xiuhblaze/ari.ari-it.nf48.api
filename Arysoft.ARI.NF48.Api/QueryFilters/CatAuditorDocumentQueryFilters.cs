using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class CatAuditorDocumentQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public CatAuditorDocumentType? DocumentType { get; set; }

        public CatAuditorDocumentSubCategoryType? SubCategory { get; set; }

        public StatusType? Status { get; set; }

        public CatAuditorDocumentOrderType? Order { get; set; }
    }
}