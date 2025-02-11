using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class NoteItemDto
    {
        public Guid ID { get; set; }

        public Guid? OwnerID { get; set; }

        public string Text { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }
    } // NoteItemDto

    public class NotePostDto
    {
        [Required]
        public Guid OwnerID { get; set; }

        [StringLength(250)]
        public string Text { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NotePostDto

    public class NotePutDto
    {
        [Required]
        public Guid ID { get; set; }

        [StringLength(250)]
        public string Text { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NotePutDto

    public class NoteDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // NoteDeleteDto
}