using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class UserSettingItemDto
    {
        public Guid ID { get; set; }

        public Guid UserID { get; set; }

        public string Settings { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }
    } // UserSettingItemDto

    public class UserSettingCreateDto
    {
        [Required]
        public Guid UserID { get; set; }

        [StringLength(1000)]
        public string Settings { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // UserSettingCreateDto

    public class UserSettingUpdateDto
    {
        [Required]
        public Guid ID { get; set; }
                
        [StringLength(1000)]
        public string Settings { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    } // UserSettingUpdateDto

    public class UserSettingDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}