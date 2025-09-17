using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class UserSetting : BaseModel
    {
        public Guid UserID { get; set; }

        public string Settings { get; set; }

        // RELATIONS

        public virtual User User { get; set; }
    }
}