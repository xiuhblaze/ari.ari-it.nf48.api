using System;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Note : BaseModel
    {
        public Guid OwnerID { get; set; }

        public string Text { get; set; }
    } // Note
}