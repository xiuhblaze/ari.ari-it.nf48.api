using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class ApplicationQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public Guid? StandardID { get; set; }

        public Guid? NaceCodeID { get; set; }

        public Guid? RiskLevelID { get; set; }

        public LanguageType? AuditLanguage { get; set; }

        public int? TotalEmployesStart { get; set; }

        public int? TotalEmployesEnd { get; set; }

        public ApplicationStatusType? Status { get; set; }

        public ApplicationOrderType Order { get; set; }
    }
}