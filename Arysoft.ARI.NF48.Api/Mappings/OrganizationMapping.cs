using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class OrganizationMapping
    {
        public static async Task<IEnumerable<OrganizationItemListDto>> OrganizationToListDto(IEnumerable<Organization> items)
        {
            var itemsDto = new List<OrganizationItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(await OrganizationToItemListDto(item));
            }

            return itemsDto;
        } // OrganizationToListDto 

        public static async Task<OrganizationItemListDto> OrganizationToItemListDto(Organization item)
        {
            var auditRepository = new AuditRepository();
            var nextAudit = auditRepository
                .GetNextAudit(item.ID, null, AuditNextAuditOwnerType.Organization);
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

            var employeesCount = item.Sites != null
                ? item.Sites
                    .Where((Site i) => i.Status == StatusType.Active)
                    .Sum((Site i) => i.Shifts
                        .Where((Shift s) => s.Status == StatusType.Active)
                        .Sum((Shift s) => s.NoEmployees)) ?? 0
                : 0;

            // Buscar dentro de todas las auditorias de la organizacion y sus ciclos,
            // la más próxima a ejecutarse.

            return new OrganizationItemListDto
            {
                ID = item.ID,
                // OrganizationFamilyID = item.OrganizationFamilyID,
                Folio = item.Folio,
                Name = item.Name,
                LogoFile = item.LogoFile,
                Website = item.Website,
                Phone = item.Phone,
                ExtraInfo = item.ExtraInfo,
                FolderFolio = item.FolderFolio,
                Status = item.Status,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                //CertificatesCount = item.Certificates != null
                //    ? item.Certificates.Where(i => i.Status != CertificateStatusType.Nothing).Count()
                //    : 0,
                MainContact = mainContact != null
                    ? ContactMapping.ContactToItemListDto(mainContact)
                    : null,
                MainSite = mainSite != null
                    ? SiteMapping.SiteToItemListDto(mainSite)
                    : null,
                NextAudit = nextAudit != null
                    ? AuditMapping.AuditToItemListDto(nextAudit)
                    : null,
                AuditCyclesCount = item.AuditCycles != null
                    ? item.AuditCycles.Where(i => 
                        i.Status != StatusType.Nothing && i.Status != StatusType.Deleted)
                        .Count()
                    : 0,
                Companies = item.Companies != null
                    ? CompanyMapping.CompanyToListDto(item.Companies
                        .Where(c => c.Status != StatusType.Nothing))
                    : null,
                ContactsCount = item.Contacts != null 
                    ? item.Contacts.Where(i => i.Status == StatusType.Active).Count() 
                    : 0,
                ContactName = mainContact != null  
                    ? Tools.Strings.FullName(mainContact.FirstName, mainContact.MiddleName, mainContact.LastName)
                    : string.Empty,
                ContactEmail = mainContact != null 
                    ? mainContact.Email 
                    : string.Empty,
                ContactPhone = mainContact != null
                    ? mainContact.Phone 
                    : string.Empty,
                NotesCount = item.Notes != null 
                    ? item.Notes.Count() 
                    : 0,
                SitesCount = item.Sites != null 
                    ? item.Sites.Where(i => i.Status == StatusType.Active).Count() 
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
                SitesEmployeesCount = employeesCount,
                Standards = item.OrganizationStandards != null
                    ? await OrganizationStandardMapping.OrganizationStandardToListDto(item.OrganizationStandards
                        .Where(os => os.Status != StatusType.Nothing))
                    : null
                //CertificatesValidityStatus = item.CertificatesValidityStatus
                //    ?? CertificateValidityStatusType.Nothing
            };
        } // OrganizationToItemListDto

        public static async Task<OrganizationItemDetailDto> OrganizationToItemDetailDto(Organization item)
        {
            return new OrganizationItemDetailDto
            {
                ID = item.ID,
                Folio = item.Folio,
                Name = item.Name,
                LogoFile = item.LogoFile,
                Website = item.Website,
                Phone = item.Phone,
                ExtraInfo = item.ExtraInfo,
                FolderFolio = item.FolderFolio,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,                
                //Certificates = item.Certificates != null
                //    ? CertificateMapping.CertificatesToListDto(item.Certificates
                //        .Where(i => i.Status != CertificateStatusType.Nothing))
                //    : new List<CertificateItemListDto>(),
                AuditCycles = item.AuditCycles != null
                    ? AuditCycleMapping.AuditCyclesToListDto(item.AuditCycles
                        .Where(i => i.Status != StatusType.Nothing)
                        .OrderByDescending(i => i.Created))
                    : null,
                Companies = item.Companies != null
                    ? CompanyMapping.CompanyToListDto(item.Companies)
                    : null,
                Contacts = item.Contacts != null
                    ? ContactMapping.ContactToListDto(item.Contacts
                        .Where(i => i.Status != StatusType.Nothing))
                    : new List<ContactItemListDto>(),
                Notes = item.Notes != null
                    ? NoteMapping.NotesToListDto(item.Notes
                        .Where(i => i.Status != StatusType.Nothing))
                    : new List<NoteItemDto>(),
                Sites = item.Sites != null
                    ? SiteMapping.SiteToListDto(item.Sites
                        .Where(i => i.Status != StatusType.Nothing))
                    : new List<SiteItemListDto>(),
                Standards = item.OrganizationStandards != null
                    ? await OrganizationStandardMapping.OrganizationStandardToListDto(item.OrganizationStandards
                        .Where(os => os.Status != StatusType.Nothing))
                    : null,                
                //CertificatesValidityStatus = item.CertificatesValidityStatus
                //    ?? CertificateValidityStatusType.Nothing,
            };
        } // OrganizationToItemDetailDto

        public static OrganizationItemProposalDto OrganizationToItemProposalDto(Organization item)
        {
            return new OrganizationItemProposalDto
            {
                ID = item.ID,
                Name = item.Name,
                Website = item.Website,
                Phone = item.Phone,
                Companies = CompanyMapping.CompanyToListDto(item.Companies
                    .Where(c => c.Status != StatusType.Nothing))
            };
        } // OrganizationToItemProposalDto

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
                Website = itemDto.Website,
                Phone= itemDto.Phone,
                ExtraInfo = itemDto.ExtraInfo,
                FolderFolio = itemDto.FolderFolio,
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