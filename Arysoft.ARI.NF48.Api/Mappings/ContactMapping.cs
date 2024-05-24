using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

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
                FirstName = item.FirstName,
                LastName = item.LastName,
                Phone = item.Phone,
                PhoneExtensions = item.PhoneExtensions,
                Email = item.Email,
                Position = item.Position,
                Status = item.Status
            };
        } // ContactToItemListDto

        public static ContactItemDetailDto ContactToItemDetailDto(Contact item)
        {
            return new ContactItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Phone = item.Phone,
                PhoneExtensions = item.PhoneExtensions,
                Email = item.Email,
                Position = item.Position,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Organization = item.Organization ?? new Organization(), // HACK: Cambiar por OrganizationItemListDto
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
                LastName = itemDto.LastName,
                Phone = itemDto.Phone,
                PhoneExtensions= itemDto.PhoneExtensions,
                Email = itemDto.Email,
                Position = itemDto.Position,
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