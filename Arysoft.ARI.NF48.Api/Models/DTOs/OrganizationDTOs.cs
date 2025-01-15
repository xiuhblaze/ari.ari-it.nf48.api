using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class OrganizationItemListDto
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string LegalEntity { get; set; }

        public string LogoFile { get; set; }

        public string QRFile { get; set; }

        public string Website { get; set; }

        public string Phone { get; set; }

        public string COID { get; set; }

        public OrganizationStatusType Status { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public int ContactsCount { get; set; }

        public string ContactName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public int SitesCount { get; set; }

        public string SiteDescription { get; set; }

        public string SiteLocation { get; set; }

        public int AuditCyclesCount { get; set; }

        public int CertificatesCount { get; set; }

        // CALCULATED

        public OrganizationCertificatesStatusType CertificatesStatus { get; set; }

    } // OrganizationItemListDto

    public class OrganizationItemDetailDto
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string LegalEntity { get; set; }

        public string LogoFile { get; set; }

        public string QRFile { get; set; }

        public string Website { get; set; }

        public string Phone { get; set; }

        public string COID { get; set; }

        public OrganizationStatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS
        
        public IEnumerable<ApplicationItemListDto> Applications { get; set; }

        public IEnumerable<AuditCycleItemListDto> AuditCycles { get; set; }

        // public IEnumerable<CertificateItemListDto> Certificates { get; set; }

        public IEnumerable<ContactItemListDto> Contacts { get; set; }

        public IEnumerable<SiteItemListDto> Sites { get; set; }

        // CALCULATED

        public OrganizationCertificatesStatusType CertificatesStatus { get; set; }
    }

    public class OrganizationPostDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class OrganizationPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string LegalEntity { get; set; }

        [StringLength(250)]
        public string Website { get; set; }

        [StringLength(10)]
        public string Phone { get; set; }

        [StringLength(20)]
        public string COID { get; set; }

        [Required]
        public OrganizationStatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class OrganizationDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}