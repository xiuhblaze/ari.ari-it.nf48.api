using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class AppFormItemListDto
    {
        public Guid ID { get; set; }

        public Guid OrganizationID { get; set; }

        public Guid? AuditCycleID { get; set; }

        public Guid? StandardID { get; set; }

        // ISO 9K

        public string ActivitiesScope { get; set; }

        public int? ProcessServicesCount { get; set; }

        public string ProcessServicesDescription { get; set; }

        public string LegalRequirements { get; set; }

        public bool? AnyCriticalComplaint { get; set; }

        public string CriticalComplaintComments { get; set; }

        public int? AutomationLevelPercent { get; set; }

        public string AutomationLevelJustification { get; set; }

        public bool? IsDesignResponsibility { get; set; }

        public string DesignResponsibilityJustify { get; set; }

        // GENERAL

        public string Description { get; set; }

        public string AuditLanguage { get; set; }

        public CycleYearType? CycleYear { get; set; }

        public string CurrentCertificationsExpiration { get; set; }

        public string CurrentStandards { get; set; }

        public string CurrentCertificationsBy { get; set; }

        public string OutsourcedProcess { get; set; }

        public bool? AnyConsultancy { get; set; }

        public string AnyConsultancyBy { get; set; }

        // INTERNAL

        public DateTime? SalesDate { get; set; }

        public DateTime? ReviewDate { get; set; }

        public string ReviewJustification { get; set; }

        public string UserSales { get; set; }

        public string UserReviewer { get; set; }

        public string HistoricalDataJSON { get; set; }

        public AppFormStatusType Status { get; set; }

        // RELATIONS

        public string OrganizationName { get; set; }

        public string AuditCycleName { get; set; }

        public string StandardName { get; set; }

        public string UserSalesName { get; set; }

        public string UserReviewerName { get; set; }

        public ICollection<string> Nacecodes { get; set; }

        public ICollection<string> Contacts { get; set; }

        public ICollection<string> Sites { get; set; }

        public int EmployeesCount { get; set; }

        public int NotesCount { get; set; }

        public int ADCCount { get; set; }
    } // AppFormItemListDto

    public class AppFormItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid OrganizationID { get; set; }

        public Guid? AuditCycleID { get; set; }

        public Guid? StandardID { get; set; }

        // ISO 9K

        public string ActivitiesScope { get; set; }

        public int? ProcessServicesCount { get; set; }

        public string ProcessServicesDescription { get; set; }

        public string LegalRequirements { get; set; }

        public bool? AnyCriticalComplaint { get; set; }

        public string CriticalComplaintComments { get; set; }

        public int? AutomationLevelPercent { get; set; }

        public string AutomationLevelJustification { get; set; }

        public bool? IsDesignResponsibility { get; set; }

        public string DesignResponsibilityJustify { get; set; }

        // GENERAL

        public string Description { get; set; }

        public string AuditLanguage { get; set; }

        public CycleYearType? CycleYear { get; set; }

        public string CurrentCertificationsExpiration { get; set; }

        public string CurrentStandards { get; set; }

        public string CurrentCertificationsBy { get; set; }

        public string OutsourcedProcess { get; set; }

        public bool? AnyConsultancy { get; set; }

        public string AnyConsultancyBy { get; set; }

        // INTERNAL

        public DateTime? SalesDate { get; set; }

        public DateTime? ReviewDate { get; set; }
 // 
        // public string ReviewJustification { get; set; } // 22K: Justification of the reasons why the application is declining

        public string UserSales { get; set; }

        public string UserReviewer { get; set; }

        public string HistoricalDataJSON { get; set; }

        public AppFormStatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public OrganizationItemListDto Organization { get; set; }

        public AuditCycleItemListDto AuditCycle { get; set; }

        public StandardItemListDto Standard { get; set; }

        public ICollection<ADCItemListDto> ADCs { get; set; }

        public ICollection<NaceCodeItemListDto> Nacecodes { get; set; }

        public ICollection<ContactItemListDto> Contacts { get; set; }

        public ICollection<SiteItemListDto> Sites { get; set; }

        public ICollection<NoteItemDto> Notes { get; set; }

    } // AppFormItemDetailDto

    public class AppFormCreateDto
    {
        [Required]
        public Guid OrganizationID { get; set; }

        [Required]
        public Guid AuditCycleID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AppFormCreateDto

    public class AppFormUpdateDto
    {
        public Guid ID { get; set; }

        public Guid StandardID { get; set; }

        // ISO 9K

        [StringLength(1000)]
        public string ActivitiesScope { get; set; }

        public int? ProcessServicesCount { get; set; }

        [StringLength(1000)]
        public string ProcessServicesDescription { get; set; }

        [StringLength(1000)]
        public string LegalRequirements { get; set; }

        public bool? AnyCriticalComplaint { get; set; }

        [StringLength(1000)]
        public string CriticalComplaintComments { get; set; }

        public int? AutomationLevelPercent { get; set; }

        [StringLength(1000)]
        public string AutomationLevelJustification { get; set; }

        public bool? IsDesignResponsibility { get; set; }

        [StringLength(1000)]
        public string DesignResponsibilityJustify { get; set; }

        // GENERAL

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(2)]
        public string AuditLanguage { get; set; }

        [StringLength(100)]
        public string CurrentCertificationsExpiration { get; set; }

        [StringLength(100)]
        public string CurrentStandards { get; set; }

        [StringLength(100)]
        public string CurrentCertificationsBy { get; set; }

        [StringLength(1000)]
        public string OutsourcedProcess { get; set; }

        public bool? AnyConsultancy { get; set; }

        [StringLength(250)]
        public string AnyConsultancyBy { get; set; }

        // INTERNAL

        public AppFormStatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AppFormUpdateDto

    public class AppFormDuplicateDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AppFormDuplicateDto

    public class AppFormDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AppFormDeleteDto

    public class AppFormNaceCodeDto
    {
        [Required]
        public Guid AppFormID { get; set; }

        [Required]
        public Guid NaceCodeID { get; set; }
    } // AppFormNaceCodeDto

    public class AppFormContactDto
    {
        [Required]
        public Guid AppFormID { get; set; }

        [Required]
        public Guid ContactID { get; set; }
    } // AppFormContactDto

    public class AppFormSiteDto
    {
        [Required]
        public Guid AppFormID { get; set; }

        [Required]
        public Guid SiteID { get; set; }
    } // AppFormSiteDto
}