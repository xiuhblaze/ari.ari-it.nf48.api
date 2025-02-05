using Arysoft.ARI.NF48.Api.Enumerations;
using System;

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

        public CertificateStatusType? Status { get; set; }

        public CertificateOrderType? Order { get; set; }
    }
}