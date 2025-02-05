using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class OrganizationQueryFilters : BaseQueryFilters
    {
        public Guid? StandardID { get; set; }

        public int? Folio { get; set; }

        public string Text { get; set; }

        public CertificateValidityStatusType? CertificatesValidityStatus { get; set; }

        public OrganizationStatusType? Status { get; set; }

        public OrganizationOrderType Order { get; set; }
    }
}