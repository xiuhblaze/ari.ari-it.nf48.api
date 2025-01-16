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
            var mainSite = item.Sites?
                .Where(s => s.IsMainSite)
                .FirstOrDefault();

            // Si no hay un contacto principal, pone el que sea
            if (mainContact == null) mainContact = item.Contacts?
                .Where(c => c.Status == StatusType.Active)
                .FirstOrDefault();

            return new OrganizationItemListDto
            {
                ID = item.ID,
                Name = item.Name,
                LegalEntity = item.LegalEntity,
                LogoFile = item.LogoFile,
                QRFile = item.QRFile,
                Website = item.Website,
                Phone = item.Phone,
                COID = item.COID,
                Status = item.Status,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                CertificatesCount = item.Certificates != null
                    ? item.Certificates.Where(i => i.Status != StatusType.Nothing).Count()
                    : 0,
                ContactsCount = item.Contacts != null 
                    ? item.Contacts.Where(i => i.Status != StatusType.Nothing).Count() 
                    : 0,
                ContactName = mainContact != null  
                    ? mainContact.FirstName 
                    : string.Empty,
                ContactEmail = mainContact != null 
                    ? mainContact.Email 
                    : string.Empty,
                ContactPhone = mainContact != null
                    ? mainContact.Phone 
                    : string.Empty,
                SitesCount = item.Sites != null 
                    ? item.Sites.Where(i => i.Status != StatusType.Nothing).Count() 
                    : 0,
                SiteDescription = mainSite != null 
                    ? mainSite.Description 
                    : string.Empty,
                SiteLocation = mainSite != null 
                    ? mainSite.Address 
                    : string.Empty,
                SiteLocationURL = mainSite != null
                    ? mainSite.LocationURL
                    : string.Empty,
                AuditCyclesCount = item.AuditCycles != null 
                    ? item.AuditCycles.Where(i => i.Status != StatusType.Nothing).Count() 
                    : 0,
                CertificatesValidityStatus = item.CertificatesValidityStatus
                    ?? CertificateValidityStatusType.Nothing
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
                QRFile = item.QRFile,
                Website = item.Website,
                Phone = item.Phone,
                COID = item.COID,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Applications = item.Applications != null
                    ? ApplicationMapping.ApplicationsToListDto(item.Applications
                        .Where(i => i.Status != ApplicationStatusType.Nothing))
                    : new List<ApplicationItemListDto>(),
                AuditCycles = item.AuditCycles != null
                    ? AuditCycleMapping.AuditCyclesToListDto(item.AuditCycles
                        .Where(i => i.Status != StatusType.Nothing)
                        .OrderByDescending(ac => ac.StartDate))
                    : new List<AuditCycleItemListDto>(),
                Certificates = item.Certificates != null
                    ? CertificateMapping.CertificatesToListDto(item.Certificates
                        .Where(i => i.Status != StatusType.Nothing))
                    : new List<CertificateItemListDto>(),
                Contacts = item.Contacts != null
                    ? ContactMapping.ContactToListDto(item.Contacts
                        .Where(i => i.Status != StatusType.Nothing))
                    : new List<ContactItemListDto>(),
                Sites = item.Sites != null
                    ? SiteMapping.SiteToListDto(item.Sites
                        .Where(i => i.Status != StatusType.Nothing))
                    : new List<SiteItemListDto>(),
                CertificatesValidityStatus = item.CertificatesValidityStatus
                    ?? CertificateValidityStatusType.Nothing
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
                Website = itemDto.Website,
                Phone= itemDto.Phone,
                COID = itemDto.COID,
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