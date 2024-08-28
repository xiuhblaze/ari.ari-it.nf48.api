using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class ApplicationItemListDto
    {
        public Guid ID { get; set; }

        // ORGANIZATION

        public string OrganizationName { get; set; }

        public string Sites { get; set; }

        public string LogoFile { get; set; }

        public string ContactName { get; set; }

        public string StandardName { get; set; }

        // SPECIFIC

        public string NaceCodeName { get; set; }

        public string Category22KName { get; set; }

        public int? HACCP { get; set; }

        public string Scope { get; set; }

        public int? NumberScope { get; set; }

        public string Seasonality { get; set; }

        public string Services { get; set; }

        public string LegalRequirements { get; set; }

        public bool? AnyCriticalComplaint { get; set; }

        public bool? IsDesignResponsibility { get; set; }

        // General

        public LanguageType? AuditLanguage { get; set; }

        public int? TotalEmployees { get; set; }

        public bool? AnyConsultancy { get; set; }

        public DateTime? ReviewDate { get; set; }

        public string UsernameSales { get; set; }

        public string UsernameReviewer { get; set; }

        public ApplicationStatusType Status { get; set; }

        public DateTime Created { get; set; }
    } // ApplicationListItemDto

    
    public class ApplicationItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid? OrganizationID { get; set; }

        public Guid? StandardID { get; set; }

        // SPECIFIC

        public Guid? NaceCodeID { get; set; }

        public Guid? RiskLevelID { get; set; }

        public Guid? Category22KID { get; set; }

        public Guid? UserSalesID { get; set; }

        public Guid? UserReviewerID { get; set; }

        public int? HACCP { get; set; }                         // FSSC, 22K

        public string Scope { get; set; }                       // FSSC, 22K

        public int? NumberScope { get; set; }                   // FSSC, 22K -> # lineas de producto

        public string Seasonality { get; set; }                 // FSSC, 22K

        public string Services { get; set; }

        public string LegalRequirements { get; set; }

        public bool? AnyCriticalComplaint { get; set; }

        public string CriticalComplaintComments { get; set; }

        public string AutomationLevel { get; set; }

        public bool? IsDesignResponsibility { get; set; }

        public string DesignResponsibilityJustify { get; set; }

        // GENERAL

        public LanguageType? AuditLanguage { get; set; }

        public DateTime? CurrentCertificationExpirationDate { get; set; }

        public string CurrentCertificationBy { get; set; }

        public string CurrentStandards { get; set; }

        public int? TotalEmployees { get; set; }

        public string OutsourcedProcess { get; set; }

        public bool? AnyConsultancy { get; set; }

        public string AnyConsultancyBy { get; set; }

        public DateTime? ReviewDate { get; set; }

        public string ReviewJustification { get; set; }

        public string ReviewComments { get; set; }

        public ApplicationStatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public OrganizationItemListDto Organization { get; set; }

        public StandardItemListDto Standard { get; set; }

        public NaceCodeItemListDto NaceCode { get; set; }

        // public RiskLevelItemListDto RiskLevel { get; set; }

        public Category22KItemListDto Category22K { get; set; }

    } // ApplicationItemDetailDto


    public class ApplicationPostDto
    {
        public string UpdatedUser { get; set; }
    } // ApplicationPostDto


    public class ApplicationPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public Guid? OrganizationID { get; set; }

        [Required]
        public Guid? StandardID { get; set; }

        // SPECIFIC

        public Guid? NaceCodeID { get; set; }

        public Guid? RiskLevelID { get; set; }

        public Guid? Category22KID { get; set; }                // FSSC, 22K

        public Guid? UserSalesID { get; set; }

        public Guid? UserReviewerID { get; set; }

        public int? HACCP { get; set; }                         // FSSC, 22K

        [StringLength(1000)]
        public string Scope { get; set; }                       // FSSC, 22K

        public int? NumberScope { get; set; }                   // FSSC, 22K -> # lineas de producto

        [StringLength(500)]
        public string Seasonality { get; set; }                 // FSSC, 22K

        [StringLength(1000)]
        public string Services { get; set; }

        [StringLength(1000)]
        public string LegalRequirements { get; set; }

        public bool? AnyCriticalComplaint { get; set; }

        [StringLength(1000)]
        public string CriticalComplaintComments { get; set; }

        [StringLength(1000)]
        public string AutomationLevel { get; set; }

        public bool? IsDesignResponsibility { get; set; }

        [StringLength(1000)]
        public string DesignResponsibilityJustify { get; set; }

        // GENERAL

        public LanguageType? AuditLanguage { get; set; }

        public DateTime? CurrentCertificationExpirationDate { get; set; }

        [StringLength(50)]
        public string CurrentCertificationBy { get; set; }

        [StringLength(250)]
        public string CurrentStandards { get; set; }

        public int? TotalEmployees { get; set; }

        [StringLength(1000)]
        public string OutsourcedProcess { get; set; }

        public bool? AnyConsultancy { get; set; }

        [StringLength(250)]
        public string AnyConsultancyBy { get; set; }

        public DateTime? ReviewDate { get; set; }

        public string ReviewJustification { get; set; }

        public string ReviewComments { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ApplicationPutDto


    public class ApplicationStatusDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public ApplicationStatusType Status { get; set; }

        [StringLength(250)]
        public string Note { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ApplicationStatusDto


    public class ApplicationDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // ApplicationDeleteDto
}