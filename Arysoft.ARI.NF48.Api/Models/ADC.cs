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

        public string UserCreate { get; set; }

        public string UserReviewer { get; set; }

        public DateTime? ReviewDate { get; set; }

        public string ReviewComments { get; set; }

        public DateTime? ActiveDate { get; set; }

        public string ExtraInfo { get; set; }

        public new ADCStatusType Status { get; set; }

        // RELATIONS

        public virtual AppForm AppForm { get; set; }

        public virtual ICollection<ADCSite> ADCSites { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
    }
}