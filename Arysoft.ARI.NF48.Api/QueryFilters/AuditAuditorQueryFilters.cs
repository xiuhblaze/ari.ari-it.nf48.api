using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class AuditAuditorQueryFilters : BaseQueryFilters
    {
        public Guid? AuditID { get; set; }

        public BoolType? IsLeader { get; set; }

        public BoolType? IsWitness { get; set; }

        public StatusType? Status { get; set; }

        public AuditAuditorOrderType Order { get; set; }
    }
}