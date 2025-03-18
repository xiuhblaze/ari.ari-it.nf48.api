using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AppForm : BaseModel
    {
        public Guid OrganizationID { get; set; }

        public Guid? AuditCycleID { get; set; }

        public Guid? StandardID { get; set; }

        public Guid? UserSalesID { get; set; }

        public Guid? UserReviewerID { get; set; }

        // ISO 9001

        public string ActivitiesScope { get; set; }

        public int? ProcessServiceCount { get; set; }

        public string ProcessServiceDescription { get; set; }

        public string LegalRequirements { get; set; }

        public bool? AnyCriticalComplaint { get; set; }

        public string CriticalComplaintComments { get; set; }

        public string AutomationLevel { get; set; }

        public bool? IsDesignResponsibility { get; set; }

        public string DesignResponsibilityJustify { get; set; }

        // GENERAL

        public LanguageType? AuditLanguage { get; set; }

        public string OutsourcedProcess { get; set; }

        public bool? AnyConsultancy { get; set; }

        public string AnyConsultancyBy { get; set; }

        // RELATIONS

        public virtual ICollection<Contact> Contacts { get; set; }

        // public virtual ICollection<AppFormCurrentCertification> CurrentCertifications { get; set; }

        public virtual ICollection<NaceCode> NaceCodes { get; set; }

        public virtual ICollection<Site> Sites { get; set; }
    }
}