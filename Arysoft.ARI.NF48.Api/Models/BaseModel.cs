using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public abstract class BaseModel
    {
        public Guid ID { get; set; }

        // Rest of properties in each model...

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }
    }
}