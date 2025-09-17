using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class UserSettingQueryFilters : BaseQueryFilters
    {
        public Guid? UserID { get; set; }

        public string Text { get; set; }

        //public string Key { get; set; }

        //public string Value { get; set; }

        public StatusType? Status { get; set; }

        public UserSettingOrderType? Order { get; set; }
    }
}