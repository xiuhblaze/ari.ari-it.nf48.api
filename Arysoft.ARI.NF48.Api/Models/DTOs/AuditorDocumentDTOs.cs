using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class AuditorDocumentItemListDto
    {
        public Guid ID { get; set; }

        public Guid AuditorID { get; set; }

        public Guid CatAuditorDocumentID { get; set; }

        public string Filename { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string Observations { get; set; }

        public AuditorDocumentType? Type { get; set; }

        public StatusType Status { get; set; }

        public AuditorDocumentValidityType ValidityStatus { get; set; }

        public string AuditorFullName { get; set; }

        public string CatDescription { get; set; }
    } // AuditorDocumentItemListDto

    public class AuditorDocumentItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid AuditorID { get; set; }

        public Guid CatAuditorDocumentID { get; set; }

        public string Filename { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string Observations { get; set; }

        public AuditorDocumentType? Type { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // Calculated

        public AuditorDocumentValidityType ValidityStatus { get; set; }

        // Relations

        public AuditorItemListDto Auditor { get; set; }

        public CatAuditorDocumentItemListDto CatAuditorDocument { get; set; }
    } // AuditorDocumentItemDetailDto

    public class AuditorDocumentPostDto
    {
        [Required]
        public Guid AuditorID { get; set; }

        [Required]
        public Guid CatAuditorDocumentID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditorDocumentPostDto

    public class AuditorDocumentPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [StringLength(500)]
        public string Observations { get; set; }

        public AuditorDocumentType? Type { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditorDocumentPutDto

    public class AuditorDocumentDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditorDocumentDeleteDto

}