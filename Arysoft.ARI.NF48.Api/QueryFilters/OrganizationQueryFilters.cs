using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class OrganizationQueryFilters : BaseQueryFilters
    {
        public string Text { get; set; }

        //public Guid? StandardID { get; set; } // Para saber cuales certificados tiene la org, no aplicable aun

        public OrganizationStatusType? Status { get; set; }

        public OrganizationOrderType Order { get; set; }
    }
}