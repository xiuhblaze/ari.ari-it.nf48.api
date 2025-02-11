using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class AuditCycleDocumentQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public Guid? AuditCycleID { get; set; }

        public Guid? StandardID { get; set; }

        public AuditCycleDocumentType? DocumentType { get; set; }

        public StatusType? Status { get; set; }

        public AuditCycleDocumentOrderType? Order { get; set; }
    }
}