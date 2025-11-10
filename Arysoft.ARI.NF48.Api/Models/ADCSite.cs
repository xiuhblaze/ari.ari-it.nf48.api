using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class ADCSite : BaseModel
    {
        public Guid ADCID { get; set; }

        public Guid? SiteID { get; set; }

        public Guid? MD5ID { get; set; }

        public decimal? InitialMD5 { get; set; }    // Se obtiene de tabla MD5
            
        public int? NoEmployees { get; set; }       // Se obtiene de Sites

        public decimal? TotalInitial { get; set; }  // Siempre se redondea hacia arriba

        public decimal? MD11 { get; set; }          // Ahora se va a manejar como porcentaje

        public string MD11Filename { get; set; }    // Para el nombre del archivo de evidencia del calculo del MD11

        public string MD11UploadedBy { get; set; }

        public decimal? Total { get; set; }             // Total en días ya sea de TotalInitial o de MD11

        public decimal? Surveillance { get; set; }      // Total de días para auditorias de vigilancia

        public decimal? Recertification { get; set; }   // Total de días para auditorias de recertificación

        public string ExtraInfo { get; set; }

        // RELATIONS

        public virtual ADC ADC { get; set; }

        public virtual Site Site { get; set; }

        public virtual MD5 MD5 { get; set; }

        public virtual ICollection<ADCConceptValue> ADCConceptValues { get; set; }

        public virtual ICollection<ADCSiteAudit> ADCSiteAudits { get; set; }

        // NOT MAPPED

        public List<ADCSiteAlertType> Alerts { get; set; }

        public bool IsMultiStandard { get; set; }
    }
}