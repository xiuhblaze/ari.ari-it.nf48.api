using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class CatAuditorDocumentItemListDto
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public CatAuditorDocumentType? DocumentType { get; set; }

        public CatAuditorDocumentSubCategoryType? SubCategory { get; set; }

        public int? UpdateEvery { get; set; }

        public CatAuditorDocumentPeriodicityType? UpdatePeriodicity { get; set; }

        public int? WarningEvery { get; set; }

        public CatAuditorDocumentPeriodicityType? WarningPeriodicity { get; set; }

        public bool? IsRequired { get; set; } // Si el documento es obligatorio para el auditor

        public int? Order { get; set; }

        public StatusType Status { get; set; }

        public int DocumentsCount { get; set; }
    }

    public class CatAuditorDocumentItemDetailDto
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public CatAuditorDocumentType? DocumentType { get; set; }

        public CatAuditorDocumentSubCategoryType? SubCategory { get; set; }

        public int? UpdateEvery { get; set; }

        public CatAuditorDocumentPeriodicityType? UpdatePeriodicity { get; set; }

        public int? WarningEvery { get; set; }

        public CatAuditorDocumentPeriodicityType? WarningPeriodicity { get; set; }

        public bool? IsRequired { get; set; } // Si el documento es obligatorio para el auditor

        public int? Order { get; set; } // Orden en que se acomoden en la lista del auditor

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public IEnumerable<AuditorDocumentItemListDto> Documents { get; set; }
    } // CatAuditorDocumentItemDetailDto

    public class CatAuditorDocumentPostDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // CatAuditorDocumentPostDto

    public class CatAuditorDocumentPutDto
    {
        public Guid ID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public CatAuditorDocumentType DocumentType { get; set; }

        public CatAuditorDocumentSubCategoryType? SubCategory { get; set; }

        [Required]
        public int UpdateEvery { get; set; }

        [Required]
        public CatAuditorDocumentPeriodicityType UpdatePeriodicity { get; set; }

        [Required]
        public int WarningEvery { get; set; }

        [Required]
        public CatAuditorDocumentPeriodicityType WarningPeriodicity { get; set; }

        [Required]
        public bool IsRequired { get; set; } // Si el documento es obligatorio para el auditor

        public int? Order { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // CatAuditorDocumentPutDto

    public class CatAuditorDocumentDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // CatAuditorDocumentDeleteDto
}