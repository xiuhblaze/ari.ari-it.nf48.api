using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class AuditorItemListDto
    {
        public Guid ID { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string PhotoFilename { get; set; }

        public decimal FeePayment { get; set; }

        public bool IsLeadAuditor { get; set; }

        public StatusType Status { get; set; }

        // CALCULATED

        public AuditorDocumentValidityType ValidityStatus { get; set; }

        public AuditorDocumentRequiredType RequiredStatus { get; set; }

        // RELATIONS

        public int StandardsCount { get; set; }

        public int DocumentsCount { get; set; }
    } // AuditorItemListDto

    public class AuditorItemDetailDto
    {
        public Guid ID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string PhotoFilename { get; set; }

        public decimal FeePayment { get; set; }

        public bool IsLeadAuditor { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // CALCULATED

        public AuditorDocumentValidityType ValidityStatus { get; set; }

        public AuditorDocumentRequiredType RequiredStatus { get; set; }

        // RELATIONS

        public IEnumerable<AuditorDocumentItemListDto> Documents { get; set; }

        public IEnumerable<AuditorStandardItemListDto> Standards { get; set; }
    } // AuditorItemDetailDto

    public class AuditorPostDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditorPostDto

    public class AuditorPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(250)]
        public string Email { get; set; }

        [StringLength(25)]
        public string Phone { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        public decimal FeePayment { get; set; }

        public bool IsLeadAuditor { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditorPutDto

    public class AuditorDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // AuditorDeleteDto
}