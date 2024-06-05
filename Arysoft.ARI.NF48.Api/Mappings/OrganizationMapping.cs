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
        public static IEnumerable<OrganizationItemListDto> OrganizationToListDto(IEnumerable<Organization> items)
        {
            var itemsDto = new List<OrganizationItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(OrganizationToItemListDto(item));
            }

            return itemsDto;
        } // OrganizationToListDto 

        public static OrganizationItemListDto OrganizationToItemListDto(Organization item)
        {
            var firstContact = item.Contacts.FirstOrDefault();

            return new OrganizationItemListDto
            {
                ID = item.ID,
                Name = item.Name,
                LegalEntity = item.LegalEntity,
                LogoFile = item.LogoFile,
                Website = item.Website,
                Phone = item.Phone,
                Status = item.Status,
                ContactName = firstContact != null ? firstContact.FirstName : string.Empty,
                ContactEmail = firstContact != null ? firstContact.Email : string.Empty,
                ContactPhone = firstContact != null ? firstContact.Phone : string.Empty
                // TODO: Aqui me quedé, estoy mejorando los DTOs
            }
                    
            };
        } // OrganizationToItemListDto


    }
}