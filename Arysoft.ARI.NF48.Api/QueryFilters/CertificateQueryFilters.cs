using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class CertificateQueryFilters : BaseQueryFilters
    {
        public Guid? OrganizationID { get; set; }

        public Guid? StandardID { get; set; }

        public string Text { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public CertificateFilterDateType? DateType { get; set; }

        public CertificateValidityStatusType? ValidityStatus { get; set; }

        public StatusType? Status { get; set; }

        public CertificateOrderType? Order { get; set; }
    }
}