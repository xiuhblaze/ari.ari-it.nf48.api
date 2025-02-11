using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class CompanyQueryFilters : BaseQueryFilters
    {
        public Guid? OrganizationID { get; set; }

        public string Text { get; set; }

        public StatusType? Status { get; set; }

        public CompanyOrderType? Order { get; set; }
    }
}