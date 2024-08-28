using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.Models
{
    public class Person : BaseModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string PhoneAlt { get; set; }

        public string LocationDescription { get; set; }

        public StatusType Status { get; set; }

        // RELATIONS

        // public virtual Auditor Auditor { get; set; }

        //public virtual Contact Contact { get; set; }

        //public virtual User User { get; set; }
    }
}