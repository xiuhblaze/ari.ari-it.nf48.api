using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class ContactPostDto
    {
        [Required]
        public Guid OrganizationID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class ContactPutDto
    {
        [Required]
        public Guid ContactID { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(10)]
        public string Phone { get; set; }

        [StringLength(20)]
        public string PhoneExtensions { get; set; }

        [EmailAddress]
        [StringLength(250)]
        public string Email { get; set; }

        [StringLength(250)]
        public string Position { get; set; }

        public StatusType Status { get; set; }

        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}