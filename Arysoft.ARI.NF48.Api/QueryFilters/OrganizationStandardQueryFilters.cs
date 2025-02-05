using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class OrganizationStandardQueryFilters : BaseQueryFilters
    {
        public Guid? OrganizationID { get; set; }

        public Guid? StandardID { get; set; }

        public string Text { get; set; }

        public StatusType? Status { get; set; }

        public OrganizationStandardOrderType Order { get; set; }
    }
}