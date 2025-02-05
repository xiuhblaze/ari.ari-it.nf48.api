using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class ContactItemListDto
    {
        public Guid ID { get; set; }

        public string OrganizationName { get; set; }

        public string FullName { get; set; }
        
        public string Email { get; set; }

        public string Phone { get; set; }

        public string PhoneAlt { get; set; }

        public string Address { get; set; }

        public string Position { get; set; }

        public string PhotoFilename { get; set; }

        public bool IsMainContact { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

    } // ContactItemListDto

    public class ContactItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid OrganizationID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string PhoneAlt { get; set; }

        public string Address { get; set; }

        public string Position { get; set; }

        public string PhotoFilename { get; set; }

        public bool IsMainContact { get; set; }

        public string ExtraInfo { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public OrganizationItemListDto Organization { get; set; }
    } // ContactItemDetailDto

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

        [StringLength(25)]
        public string PhoneAlt { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        [StringLength(250)]
        public string Position { get; set; }

        [Required]
        public bool IsMainContact { get; set; }

        [StringLength(1000)]
        public string ExtraInfo { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class ContactDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}