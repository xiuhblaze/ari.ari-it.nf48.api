using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class ContactMapping
    {
        public static IEnumerable<ContactItemListDto> ContactToListDto(IEnumerable<Contact> items)
        {
            var itemsDto = new List<ContactItemListDto>();  

            foreach (var item in items)
            {
                itemsDto.Add(ContactToItemListDto(item));
            }

            return itemsDto;
        } // ContactToListDto

        public static ContactItemListDto ContactToItemListDto(Contact item)
        {
            return new ContactItemListDto
            {
                ID = item.ID,
                OrganizationName = item.Organization != null ? item.Organization.Name : string.Empty,
                FullName = Strings.FullName(item.FirstName, item.MiddleName, item.LastName),
                Email = item.Email,
                Phone = item.Phone,
                PhoneAlt = item.PhoneAlt,
                Address = item.Address,
                Position = item.Position,
                PhotoFilename = item.PhotoFilename,
                IsMainContact = item.IsMainContact,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status
            };
        } // ContactToItemListDto

        public static async Task<ContactItemDetailDto> ContactToItemDetailDto(Contact item)
        {
            return new ContactItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                FirstName = item.FirstName,
                MiddleName = item.MiddleName,
                LastName = item.LastName,
                Email = item.Email,
                Phone = item.Phone,
                PhoneAlt = item.PhoneAlt,
                Address = item.Address,
                Position = item.Position,
                PhotoFilename = item.PhotoFilename,
                IsMainContact = item.IsMainContact,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Organization = item.Organization != null
                    ? await OrganizationMapping.OrganizationToItemListDto(item.Organization) 
                    : null
            };
        } // ContactToItemDetailDto

        public static Contact ItemAddDtoToContact(ContactPostDto itemDto)
        {
            return new Contact
            {
                OrganizationID = itemDto.OrganizationID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToContact

        public static Contact ItemEditDtoToContact(ContactPutDto itemDto)
        {
            return new Contact
            {
                ID = itemDto.ID,
                FirstName = itemDto.FirstName,
                MiddleName = itemDto.MiddleName,
                LastName = itemDto.LastName,
                Email = itemDto.Email,
                Phone = itemDto.Phone,
                PhoneAlt = itemDto.PhoneAlt,
                Address = itemDto.Address,
                Position = itemDto.Position,
                IsMainContact = itemDto.IsMainContact,
                ExtraInfo = itemDto.ExtraInfo,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToContact

        public static Contact ItemDeleteDtoToContact(ContactDeleteDto itemDto)
        {
            return new Contact
            { 
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToContact
    }
}