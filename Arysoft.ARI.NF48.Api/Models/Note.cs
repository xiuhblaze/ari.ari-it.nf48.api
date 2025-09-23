using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Note : BaseModel
    {
        [Required]
        public Guid OwnerID { get; set; }

        [StringLength(500)]
        public string Text { get; set; }
    } // Note
}