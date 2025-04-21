using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class OrganizationItemListDto
    {
        public Guid ID { get; set; }

        public int? Folio { get; set; }

        public string Name { get; set; }

        //public string LegalEntity { get; set; }

        public string LogoFile { get; set; }

        //public string QRFile { get; set; }

        public string Website { get; set; }

        public string Phone { get; set; }

        //public string COID { get; set; }

        public string ExtraInfo { get; set; }

        public int? FolderFolio { get; set; }

        public OrganizationStatusType Status { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public int AuditCyclesCount { get; set; }

        public int ContactsCount { get; set; }

        public string ContactName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public int SitesCount { get; set; }

        public string SiteDescription { get; set; }

        public string SiteLocation { get; set; }

        public string SiteLocationURL { get; set; }

        public int SitesEmployeesCount { get; set; }

        // public int CertificatesCount { get; set; }

        public int NotesCount { get; set; }

        public IEnumerable<CompanyItemListDto> Companies { get; set; }

        public IEnumerable<OrganizationStandardItemListDto> Standards { get; set; }

        // CALCULATED

        // public CertificateValidityStatusType CertificatesValidityStatus { get; set; }

    } // OrganizationItemListDto

    public class OrganizationItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid? OrganizationFamilyID { get; set; }

        public int? Folio { get; set; }

        public string Name { get; set; }

        // public string LegalEntity { get; set; }

        public string LogoFile { get; set; }

        // public string QRFile { get; set; }

        public string Website { get; set; }

        public string Phone { get; set; }

        // public string COID { get; set; }

        public string ExtraInfo { get; set; }

        public int? FolderFolio { get; set; }

        public OrganizationStatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        // public IEnumerable<ApplicationItemListDto> Applications { get; set; }

        public IEnumerable<AuditCycleItemListDto> AuditCycles { get; set; }

        public IEnumerable<CompanyItemListDto> Companies { get; set; }

        // public IEnumerable<CertificateItemListDto> Certificates { get; set; }

        public IEnumerable<ContactItemListDto> Contacts { get; set; }

        public IEnumerable<NoteItemDto> Notes { get; set; }

        public IEnumerable<SiteItemListDto> Sites { get; set; }

        public IEnumerable<OrganizationStandardItemListDto> Standards { get; set; }

        // CALCULATED

        // public CertificateValidityStatusType CertificatesValidityStatus { get; set; }
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

        [StringLength(250)]
        public string Website { get; set; }

        [StringLength(10)]
        public string Phone { get; set; }

        [StringLength(1000)]
        public string ExtraInfo { get; set; }

        public int? FolderFolio { get; set; }

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