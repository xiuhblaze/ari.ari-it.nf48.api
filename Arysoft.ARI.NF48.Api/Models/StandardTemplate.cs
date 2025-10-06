using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class StandardTemplate : BaseModel
    {
        public Guid StandardID { get; set; }

        public string Description { get; set; }

        public string Filename { get; set; }

        public string Version { get; set; }

        // RELATIONS

        public virtual Standard Standard { get; set; }

    } // StandardTemplate
}