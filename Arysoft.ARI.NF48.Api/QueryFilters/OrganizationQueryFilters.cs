using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class OrganizationQueryFilters : BaseQueryFilters
    {
        public int? Folio { get; set; }

        public string Text { get; set; }

        //public Guid? StandardID { get; set; } // Para saber cuales certificados tiene la org, no aplicable aun

        public CertificateValidityStatusType? CertificatesValidityStatus { get; set; }

        public OrganizationStatusType? Status { get; set; }

        public OrganizationOrderType Order { get; set; }
    }
}