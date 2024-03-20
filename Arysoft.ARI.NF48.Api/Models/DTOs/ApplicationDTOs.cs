using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class ApplicationItemListDto
    {
        public Guid ID { get; set; }

        public string OrtanizationName { get; set; }

        public string StandardName { get; set; }

        public string NaceCodeName { get; set; }

        public string ProcessScope { get; set; }

        public string Services { get; set; }

        public bool? AnyCriticalComplaint { get; set; }

        public bool? IsDesignResponsibility { get; set; }

        public LanguageType? AuditLanguage { get; set; }

        public bool? AnyConsultancy { get; set; }

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

        public string ProcessScope { get; set; }

        public int? NumProcess { get; set; }

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

        public int? TotalEmployes { get; set; }

        public string OutsourcedProcess { get; set; }

        public bool? AnyConsultancy { get; set; }

        public string AnyConsultancyBy { get; set; }

        public ApplicationStatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public OrganizationItemListDto Organization { get; set; }

        // public StandardItemListDto Standard { get; set; }

        // public NaceCodeItemListDto NaceCode { get; set; }

        // public RiskLevelItemListDto RiskLevel { get; set; }

    } // ApplicationItemDetailDto

    public class ApplicationPostDto
    {
        public string UpdatedUser { get; set; }
    }

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

        [StringLength(1000)]
        public string ProcessScope { get; set; }

        public int? NumProcess { get; set; }

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

        public int? TotalEmployes { get; set; }

        [StringLength(1000)]
        public string OutsourcedProcess { get; set; }

        public bool? AnyConsultancy { get; set; }

        [StringLength(250)]
        public string AnyConsultancyBy { get; set; }

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