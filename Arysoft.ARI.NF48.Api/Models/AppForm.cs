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

        public Guid? Category22KID { get; set; }                // Solo para ISO 22K

        // ISO Varios

        public string ActivitiesScope { get; set; }             // 9K, 14K, 22K, 37K (scope of certification)

        public int? ProcessServicesCount { get; set; }          // 9K, 14K, 22K (lines of product), 37K (process/activities)

        public string ProcessServicesDescription { get; set; }  // 9K, 14K, 22K (lines of product description), 37K (process/activities description)

        public string LegalRequirements { get; set; }           // 9K, 14K, 22K, 37K (Anti-bribery controls)

        public bool? AnyCriticalComplaint { get; set; }         // 9K, 14K, 37K (Organization Involved in a bribery)

        public string CriticalComplaintComments { get; set; }   // 9K, 14K, 37K (Organization Involved in a bribery - comments)

        public int? AutomationLevelPercent { get; set; }            // 9K, 27K Porcentaje de automatización del proceso

        public string AutomationLevelJustification { get; set; }    // 9K, 27K

        // ISO 9K

        public bool? IsDesignResponsibility { get; set; }

        public string DesignResponsibilityJustify { get; set; }

        // ISO 14K

        public string OperationalControls { get; set; }

        // ISO 22K

        public int? HACCPCount { get; set; }            // 22K: Indica el numero de procesos HACCP que se tienen, puede entrar más de una linea de producción en un proceso HACCP

        public string SeasonalityJSON { get; set; }     // 22K: Indica si la organización tiene estacionalidad en su producción o ventas

        public string ReviewJustification { get; set; } // 22K: Justification of the reasons why the application is declining

        // ISO 27K

        public string AssetsISO27KJSON { get; set; }    // 27K: Cadena JSON con la información de los activos

        // ISO 45K

        public string OHSHazardRisk45KJSON { get; set; }        // 45K: Cadena JSON con la información de los riesgos o peligros OHS

        public string HazardousMaterials45KJSON { get; set; }   // 45K: Cadena JSON con la información de los materiales peligrosos

        public string AccidentRate45KJSON { get; set; }         // 45K: Cadena JSON con la información de las tasas de accidentes

        public string IndirectHSRisk45KJSON { get; set; }       // 45K: Cadena JSON con la información de los riesgos OHS indirectos

        public string HighLevelRisks45K { get; set; }           // 45K: Cadena con una lista de los riesgos OHS de alto nivel separados por coma

        // GENERAL

        public string Description { get; set; }         // Descripción corta del appform

        public string AuditLanguage { get; set; }       // Siglas del idioma en base al ISO 639-1

        public CycleYearType? CycleYear { get; set; }   // Año al que pertenece el appForm dentro del ciclo de auditoría

        public string CurrentCertificationsExpiration { get; set; } // Fechas separadas por coma

        public string CurrentStandards { get; set; }    // Estandards separados por coma

        public string CurrentCertificationsBy { get; set; } // Empresas que certificaron, separadas por coma

        public string OutsourcedProcess { get; set; }

        public bool? AnyConsultancy { get; set; }

        public string AnyConsultancyBy { get; set; }

        // INTERNAL

        public DateTime? SalesDate { get; set; }        // Última fecha en que Ventas (sales) aprueba o rechaza el appform

        public DateTime? ReviewDate { get; set; }       // Última fecha en que el revisor del appform aprueba o rechaza

        public string UserSales { get; set; }

        public string UserReviewer { get; set; }

        public string HistoricalDataJSON { get; set; } 

        public new AppFormStatusType Status { get; set; }

        // RELATIONS

        public virtual Organization Organization { get; set; }

        public virtual AuditCycle AuditCycle { get; set; }

        public virtual Standard Standard { get; set; }

        public virtual Category22K Category22K { get; set; }

        public virtual ICollection<ADC> ADCs { get; set; } // Solo va a ser un ADC

        public virtual ICollection<Contact> Contacts { get; set; }

        public virtual ICollection<NaceCode> NaceCodes { get; set; }

        public virtual ICollection<RiskLevel> RiskLevels { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        public virtual ICollection<Site> Sites { get; set; }

        // NOT MAPPED

        public List<AppFormAlertType> Alerts { get; set; }
    }
}