using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class AppForm : BaseModel
    {
        public Guid OrganizationID { get; set; }

        public Guid AuditCycleID { get; set; }

        public Guid? StandardID { get; set; }

        // ISO 9K

        public string ActivitiesScope { get; set; }

        public int? ProcessServicesCount { get; set; }

        public string ProcessServicesDescription { get; set; }

        public string LegalRequirements { get; set; }

        public bool? AnyCriticalComplaint { get; set; }

        public string CriticalComplaintComments { get; set; }

        public int? AutomationLevelPercent { get; set; } // Porcentaje de automatización del proceso

        public string AutomationLevelJustification { get; set; }

        public bool? IsDesignResponsibility { get; set; }

        public string DesignResponsibilityJustify { get; set; }

        // GENERAL

        public string Description { get; set; } // Descripción corta del appform

        public string AuditLanguage { get; set; } // Siglas del idioma en base al ISO 639-1

        public string CurrentCertificationsExpiration { get; set; } // Fechas separadas por coma

        public string CurrentStandards { get; set; } // Estandards separados por coma

        public string CurrentCertificationsBy { get; set; } // Empresas que certificaron, separadas por coma

        public string OutsourcedProcess { get; set; }

        public bool? AnyConsultancy { get; set; }

        public string AnyConsultancyBy { get; set; }

        // INTERNAL

        public DateTime? SalesDate { get; set; }        // Última fecha en que Ventas (sales) aprueba o rechaza el appform

        public string SalesComments { get; set; }       // Comentarios de Ventas (sales) de la última aprobación o cambio de status

        public DateTime? ReviewDate { get; set; }       // Última fecha en que el revisor del appform aprueba o rechaza

        public string ReviewJustification { get; set; } // 22K: Justification of the reasons why the application is declining

        public string ReviewComments { get; set; }      // 22K: Additonal comments by application reviewer

        public string UserSales { get; set; }

        public string UserReviewer { get; set; }

        public string HistoricalDataJSON { get; set; } 

        public new AppFormStatusType Status { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual AuditCycle AuditCycle { get; set; }

        public virtual Standard Standard { get; set; }

        public virtual ICollection<ADC> ADCs { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }

        // public virtual ICollection<AppFormCurrentCertification> CurrentCertifications { get; set; }

        public virtual ICollection<NaceCode> NaceCodes { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        public virtual ICollection<Site> Sites { get; set; }
    }
}