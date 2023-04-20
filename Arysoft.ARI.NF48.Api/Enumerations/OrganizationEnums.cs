using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum OrganizationStatusType
    {
        Nothing,
        New,
        Aproved,
        Active,
        Inactive,
        Deleted
    }

    public enum OrganizationOrderType
    { 
        Nothing,
        Name,
        LegalEntity,
        Status,
        Updated,
        NameDesc,
        LegalEntityDesc,
        StatusDesc,
        UpdatedDesc
    }
}