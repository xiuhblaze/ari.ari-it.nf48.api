using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class AuditDocumentQueryFilters : BaseQueryFilters
    {
        public Guid? AuditID { get; set; }

        public Guid? StandardID { get; set; }

        public string Text { get; set; }

        public AuditDocumentType? DocumentType { get; set; }

        public StatusType? Status { get; set; }

        public AuditDocumentOrderType? Order { get; set; }
    }
}