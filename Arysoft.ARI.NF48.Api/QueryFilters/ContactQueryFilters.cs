using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class ContactQueryFilters : BaseQueryFilters
    {
        public Guid? OrganizationID { get; set; }

        public string Text { get; set; }

        public ContactIsMainType? IsMain { get; set; }

        public StatusType? Status { get; set; }

        public ContactOrderType Order { get; set; }
    }
}