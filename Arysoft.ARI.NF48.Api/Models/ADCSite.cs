using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class ADCSite : BaseModel
    {
        public Guid ADCID { get; set; }

        public Guid? SiteID { get; set; }

        public decimal? InitialMD5 { get; set; }    // Se obtiene de tabla MD5
            
        public int? NoEmployees { get; set; }       // Se obtiene de Sites

        public decimal? TotalInitial { get; set; } // Siempre se redondea hacia arriba

        public decimal? MD11 { get; set; }
        
        public decimal? Surveillance { get; set; }

        public decimal? RR { get; set; }

        public string ExtraInfo { get; set; }

        // RELATIONS

        public virtual ADC ADC { get; set; }

        public virtual Site Site { get; set; }

        public virtual ICollection<ADCConceptValue> ADCConceptValues { get; set; }

        // NOT MAPPED

        public List<ADCSiteAlertType> Alerts { get; set; }
    }
}