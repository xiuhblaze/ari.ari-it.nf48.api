using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arysoft.ARI.NF48.Api.Models
{

    public class Application : BaseModel
    {
        public Guid? OrganizationID { get; set; }

        public Guid? StandardID { get; set; }

        // SPECIFIC

        public Guid? NaceCodeID { get; set; }

        public Guid? RiskLevelID { get; set; }

        public Guid? Category22KID { get; set; }                // FSSC, 22K

        public Guid? UserSalesID { get; set; }

        public Guid? UserReviewerID { get; set; }

        public int? HACCP { get; set; }                         // FSSC, 22K

        public string Scope { get; set; }                       // FSSC, 22K

        public int? NumberScope { get; set; }                   // FSSC, 22K -> # lineas de producto

        public string Seasonality { get; set; }                 // FSSC, 22K

        public string Services { get; set; }

        public string LegalRequirements { get; set; }           // FSSC, 22K

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

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual Standard Standard { get; set; }

        public virtual NaceCode NaceCode { get; set; }

        public virtual Category22K Category22K { get; set; }

        [ForeignKey("UserSalesID")]
        public virtual User UserSales { get; set; }

        [ForeignKey("UserReviewerID")]
        public virtual User UserReviewer { get; set; }
    }
}