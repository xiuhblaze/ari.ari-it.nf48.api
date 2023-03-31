using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class NaceCodePostDto
    {   
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class NaceCodePutDto
    {
        public Guid NaceCodeID { get; set; }

        public int? Sector { get; set; }

        public int? Division { get; set; }

        public int? Group { get; set; }

        public int? Class { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public StatusType Status { get; set; }

        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}