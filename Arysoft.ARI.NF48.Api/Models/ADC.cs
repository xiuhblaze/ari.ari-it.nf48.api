using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class ADC : BaseModel
    {
        public Guid AppFormID { get; set; }

        public string Description { get; set; }

        public int? TotalEmployees { get; set; }

        public decimal? TotalInitial { get; set; }

        public decimal? TotalMD11 { get; set; }

        public decimal? TotalSurveillance { get; set; }

        public decimal? TotalRR { get; set; }

        public string UserCreates { get; set; }      // Usuario que crea el ADC

        public string UserReview { get; set; }    // Usuario que revisa y aprueba el ADC

        public DateTime? ReviewDate { get; set; }   // Fecha en que se envió a revisión por parte del creador

        public string ReviewComments { get; set; }  // Comentarios del revisor del ADC

        public DateTime? ActiveDate { get; set; }   // Fecha de activación del ADC por parte del revisor

        public string ExtraInfo { get; set; }

        public new ADCStatusType Status { get; set; }

        // RELATIONS

        public virtual AppForm AppForm { get; set; }

        public virtual ICollection<ADCSite> ADCSites { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        // NOT MAPPED

        public List<ADCAlertType> Alerts { get; set; }
    }
}