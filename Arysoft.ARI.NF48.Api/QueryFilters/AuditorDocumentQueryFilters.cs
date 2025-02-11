using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class AuditorDocumentQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public Guid? AuditorID { get; set; }

        public Guid? CatAuditorDocumentID { get; set; }

        public DateTime? DueDateStart { get; set; }

        public DateTime? DueDateEnd { get; set; }

        public StatusType? Status { get; set; }

        public AuditorDocumentOrderType? Order { get; set; }
    }
}