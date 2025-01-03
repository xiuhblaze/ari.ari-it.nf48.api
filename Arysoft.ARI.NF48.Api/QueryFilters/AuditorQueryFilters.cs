using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class AuditorQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        public AuditorLeaderType? IsLeader { get; set; }

        public AuditorDocumentStatusType? DocumentStatus { get; set; }

        public StatusType? Status { get; set; }

        public AuditorOrderType? Order { get; set; }
    }
}