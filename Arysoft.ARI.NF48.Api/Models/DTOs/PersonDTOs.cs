using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class PersonItemListDto
    {
        public Guid ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string PhoneAlt { get; set; }

        public string LocationDescription { get; set; }

        public StatusType Status { get; set; }

        public bool IsAuditor { get; set; }

        public bool IsContact { get; set; }

        public bool IsUser { get; set; }

    }

    public class PersonItemDetailDto
    {
        public Guid ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string PhoneAlt { get; set; }

        public string LocationDescription { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        public Auditor Auditor { get; set; } // HACK: Cambiarlo por AuditorItemListDto

        public ContactItemListDto Contact { get; set; }

        public UserListItemDto User { get; set; }
    }

    public class PersonPostDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class PersonPutDto
    {
        [Required(ErrorMessage = "ID is required")]
        public Guid ID { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "First name is requiered")]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(250)]
        [Required(ErrorMessage = "E-Mail is requiered")]
        public string Email { get; set; }

        [StringLength(25)]
        public string Phone { get; set; }

        [StringLength(25)]
        public string PhoneAlt { get; set; }

        [StringLength(1000)]
        public string LocationDescription { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class PersonDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}