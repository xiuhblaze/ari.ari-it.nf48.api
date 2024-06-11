using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class StandardItemListDto
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? MaxReductionsDays { get; set; }

        public StatusType Status { get; set; }

        public int NoApplications { get; set; }
    } // StandardItemListDto

    public class StandardItemDetailDto
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? MaxReductionsDays { get; set; }

        public StatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public IEnumerable<ApplicationItemListDto> Applications { get; set; }
    } // StandardItemDetailDto

    public class StandardPostDto
    {
        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class StandardPutDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public int? MaxReductionsDays { get; set; }

        [Required]
        public StatusType Status { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }

    public class StandardDeleteDto
    {
        [Required]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedUser { get; set; }
    }
}