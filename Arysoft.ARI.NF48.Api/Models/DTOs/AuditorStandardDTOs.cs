using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class AuditorStandardItemListDto
    {
        public Guid ID { get; set; }

        public Guid AuditorID { get; set; }

        public Guid StandardID { get; set; }

        public string Comments { get; set; }

        public string AuditorFullName { get; set; }

        public string StandardName { get; set; }

        public string StandardDescription { get; set; }

        public StandardBaseType StandardBase { get; set; }

        public StatusType Status { get; set; }
    } // AuditorStandardItemListDto

    public class AuditorStandardItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid AuditorID { get; set; }

        public Guid StandardID { get; set; }

        public string Comments { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public AuditorItemListDto Auditor { get; set; }

        public StandardItemListDto Standard { get; set; }
    } // AuditorStandardItemDetailDto

    public class AuditorStandardPostDto
    {
        [Required]
        public Guid AuditorID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditorStandardPostDto 

    public class AuditorStandardPutDto
    {
        [Required]
        public Guid ID { get; set; }
             
        public Guid StandardID { get; set; }

        [StringLength(1000)]
        public string Comments { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditorStandardPutDto

    public class AuditorStandardDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditorStandardDeleteDto
}