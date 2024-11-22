using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

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
            var mainContact = item.Contacts?
                .Where(c => c.IsMainContact && c.Status == StatusType.Active)
                .FirstOrDefault();
            var mainSite = item.Sites.OrderBy(s => s.IsMainSite).FirstOrDefault();

            // Si no hay un contacto principal, pon el que sea
            if (mainContact == null) mainContact = item.Contacts?
                .Where(c => c.Status == StatusType.Active)
                .FirstOrDefault();

            return new OrganizationItemListDto
            {
                ID = item.ID,
                Name = item.Name,
                LegalEntity = item.LegalEntity,
                LogoFile = item.LogoFile,
                Website = item.Website,
                Phone = item.Phone,
                Status = item.Status,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                NoContacts = item.Contacts != null ? item.Contacts.Count() : 0,
                ContactName = mainContact != null  
                    ? mainContact.FirstName 
                    : string.Empty,
                ContactEmail = mainContact != null 
                    ? mainContact.Email 
                    : string.Empty,
                ContactPhone = mainContact != null
                    ? mainContact.Phone 
                    : string.Empty,
                NoSites = item.Sites != null ? item.Sites.Count() : 0,
                SiteDescription = mainSite != null ? mainSite.Description : string.Empty,
                SiteLocation = mainSite != null ? mainSite.Address : string.Empty,
            };
        } // OrganizationToItemListDto

        public static OrganizationItemDetailDto OrganizationToItemDetailDto(Organization item)
        {
            return new OrganizationItemDetailDto
            {
                ID = item.ID,
                Name = item.Name,
                LegalEntity = item.LegalEntity,
                LogoFile = item.LogoFile,
                Website = item.Website,
                Phone = item.Phone,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Applications = item.Applications != null
                    ? ApplicationMapping.ApplicationsToListDto(item.Applications)
                    : new List<ApplicationItemListDto>(),
                Contacts = item.Contacts != null
                    ? ContactMapping.ContactToListDto(item.Contacts)
                    : new List<ContactItemListDto>(),
                Sites = item.Sites != null
                    ? SiteMapping.SiteToListDto(item.Sites)
                    : new List<SiteItemListDto>()
            };
        } // OrganizationToItemDetailDto

        public static Organization ItemAddDtoToOrganization(OrganizationPostDto itemDto)
        {
            return new Organization
            {
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToOrganization

        public static Organization ItemEditDtoToOrganization(OrganizationPutDto itemDto)
        {
            return new Organization
            {
                ID = itemDto.ID,
                Name = itemDto.Name,
                LegalEntity = itemDto.LegalEntity,
                LogoFile = itemDto.LogoFile,
                Website = itemDto.Website,
                Phone= itemDto.Phone,
                Status = itemDto.Status,
                UpdatedUser= itemDto.UpdatedUser
            };
        } // ItemEditDtoToOrganization

        public static Organization ItemDeleteDtoToOrganization(OrganizationDeleteDto itemDto)
        {
            return new Organization
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToOrganization
    }
}