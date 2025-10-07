using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class StandardTemplateItemListDto
    {
        public Guid ID { get; set; }

        public Guid StandardID { get; set; }

        public string Description { get; set; }

        public string Filename { get; set; }

        public string Version { get; set; }

        public StatusType Status { get; set; }

        public string StandardName { get; set; }

    } // StandardTemplateItemListDto

    public class StandardTemplateItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid StandardID { get; set; }

        public string Description { get; set; }

        public string Filename { get; set; }

        public string Version { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public StandardItemListDto Standard { get; set; }

    } // StandardTemplateItemDetailDto

    public class StandardTemplateCreateDto
    {
        [Required(ErrorMessage = "The Standard ID is required")]
        public Guid? StandardID { get; set; }

        [Required(ErrorMessage = "The User that creates is required")]
        [StringLength(50, ErrorMessage = "The User name must be less than 50 characters")]
        public string UpdatedUser { get; set; }
    } // StandardTemplateCreateDto

    public class StandardTemplateUpdateDto
    {
        [Required(ErrorMessage = "The ID is required to update")]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The Description is required")]
        [StringLength(500, ErrorMessage = "The Description value must be less than 500 characters")]
        public string Description { get; set; }

        //[StringLength(250, ErrorMessage = "The Filename value must be less than 250 characters")]
        //public string Filename { get; set; }

        [StringLength(10, ErrorMessage = "The Version value must be less than 10 characters")]
        public string Version { get; set; }

        [Required(ErrorMessage = "The Status value is required")]
        public StatusType? Status { get; set; } // nulable para que muestre el error personalizado

        [Required(ErrorMessage = "The User that updates is required")]
        [StringLength(50, ErrorMessage = "The User name must be less than 50 characters")]
        public string UpdatedUser { get; set; }
    } // StandardTemplateUpdateDto

    public class StandardTemplateDeleteDto
    {
        [Required(ErrorMessage = "The ID is required to delete")]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The User that deletes is required")]
        [StringLength(50, ErrorMessage = "The User name must be less than 50 characters")]
        public string UpdatedUser { get; set; }
    } // StandardTemplateDeleteDto
}