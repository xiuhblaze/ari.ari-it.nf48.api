using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class OrganizationMapping
    {


        public static OrganizationItemListDto OrganizationToItemListDto(Organization item)
        {
            return new OrganizationItemListDto
            {
                ID = item.ID,
                Name = item.Name,
                LegalEntity = item.LegalEntity,
                LogoFile = item.LogoFile,
                Website = item.Website,
                Phone = item.Phone,
                Status = item.Status
            };
        }
    }
}