using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class AuthLoginDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(32)]
        public string Password { get; set; }
    }

    public class AuthChangePasswordDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(32)]
        public string NewPassword { get; set; }
    }
}