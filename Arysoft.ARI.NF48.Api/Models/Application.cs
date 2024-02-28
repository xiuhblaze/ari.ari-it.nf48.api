using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.Models
{

    public class Application : BaseModel
    {
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

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual Standard Standard { get; set; }

        public virtual NaceCode NaceCode { get; set; }
    }
}