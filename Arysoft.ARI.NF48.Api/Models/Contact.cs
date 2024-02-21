using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arysoft.ARI.NF48.Api.Models
{
    [Table("Contacts")]
    public class Contact : BaseModel
    {
        public Guid OrganizationID { get; set; }

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

        // RELATIONS

        public Organization Organization { get; set; }

    }
}