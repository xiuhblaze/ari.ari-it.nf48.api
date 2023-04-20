using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class OrganizationPostDto
    {
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class OrganizationPutDto
    {
        public Guid OrganizationID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string LegalEntity { get; set; }

        [StringLength(250)]
        public string LogoFile { get; set; }

        [StringLength(250)]
        public string Website { get; set; }

        [StringLength(10)]
        public string Phone { get; set; }

        public OrganizationStatusType Status { get; set; }

        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}