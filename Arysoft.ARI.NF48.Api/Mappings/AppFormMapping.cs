using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class AppFormMapping
    {
        public static IEnumerable<AppFormItemListDto> AppFormToListDto(IEnumerable<AppForm> items)
        {
            var itemsDto = new List<AppFormItemListDto>();
            foreach (var item in items)
            {
                itemsDto.Add(AppFormToItemListDto(item));
            }
            return itemsDto;
        } // AppFormToListDto

        public static AppFormItemListDto AppFormToItemListDto(AppForm item)
        {
            return new AppFormItemListDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                AuditCycleID = item.AuditCycleID,
                StandardID = item.StandardID,
                // 9K
                ActivitiesScope = item.ActivitiesScope,
                ProcessServicesCount = item.ProcessServicesCount,
                ProcessServicesDescription = item.ProcessServicesDescription,
                LegalRequirements = item.LegalRequirements,
                AnyCriticalComplaint = item.AnyCriticalComplaint,
                CriticalComplaintComments = item.CriticalComplaintComments,
                AutomationLevelPercent = item.AutomationLevelPercent,
                AutomationLevelJustification = item.AutomationLevelJustification,
                IsDesignResponsibility = item.IsDesignResponsibility,
                DesignResponsibilityJustify = item.DesignResponsibilityJustify,
                // GENERAL
                Description = item.Description,
                AuditLanguage = item.AuditLanguage,
                CurrentCertificationsExpiration = item.CurrentCertificationsExpiration,
                CurrentStandards = item.CurrentStandards,
                CurrentCertificationsBy = item.CurrentCertificationsBy,
                OutsourcedProcess = item.OutsourcedProcess,
                AnyConsultancy = item.AnyConsultancy,
                AnyConsultancyBy = item.AnyConsultancyBy,
                // INTERNAL
                SalesDate = item.SalesDate,
                SalesComments = item.SalesComments,
                ReviewDate = item.ReviewDate,
                ReviewJustification = item.ReviewJustification,
                ReviewComments = item.ReviewComments,
                UserSales = item.UserSales,
                UserReviewer = item.UserReviewer,
                HistoricalDataJSON = item.HistoricalDataJSON,
                Status = item.Status,
                // RELATIONS
                OrganizationName = item.Organization != null 
                    ? item.Organization.Name 
                    : string.Empty,
                AuditCycleName = item.AuditCycle != null
                    ? item.AuditCycle.Name 
                    : string.Empty,
                StandardName = item.Standard != null
                    ? item.Standard.Name
                    : string.Empty,
                ADCCount = item.ADCs != null
                    ? item.ADCs.Count
                    : 0,
                Nacecodes = item.NaceCodes != null
                    ? item.NaceCodes.Select(n => n.Description).ToList()
                    : new List<string>(),
                Contacts = item.Contacts != null
                    ? item.Contacts
                        .OrderByDescending(c => c.IsMainContact)
                            .ThenBy(c => c.FirstName)
                        .Select(c => Tools.Strings.FullName(c.FirstName, c.MiddleName, c.LastName)).ToList()
                    : new List<string>(),
                Sites = item.Sites != null
                    ? item.Sites
                        .OrderByDescending(s => s.IsMainSite)
                            .ThenBy(s => s.Description)
                        .Select(s => s.Description)
                        .ToList()
                    : new List<string>(),
                EmployeesCount = item.Sites != null
                    ? item.Sites.Where(i => i.Status == StatusType.Active)
                        .Sum(i =>
                        {
                            Func<Shift, int?> selector = s => s.NoEmployees;
                            return i.Shifts.Where(s => s.Status == StatusType.Active).Sum(selector) ?? 0;
                        })
                    : 0,
                NotesCount = item.Notes != null
                    ? item.Notes.Count
                    : 0
            };
        } // AppFormToItemListDto

        public static AppFormItemDetailDto AppFormToItemDetailDto(AppForm item)
        {
            return new AppFormItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                AuditCycleID = item.AuditCycleID,
                StandardID = item.StandardID,
                // 9K
                ActivitiesScope = item.ActivitiesScope,
                ProcessServicesCount = item.ProcessServicesCount,
                ProcessServicesDescription = item.ProcessServicesDescription,
                LegalRequirements = item.LegalRequirements,
                AnyCriticalComplaint = item.AnyCriticalComplaint,
                CriticalComplaintComments = item.CriticalComplaintComments,
                AutomationLevelPercent = item.AutomationLevelPercent,
                AutomationLevelJustification = item.AutomationLevelJustification,
                IsDesignResponsibility = item.IsDesignResponsibility,
                DesignResponsibilityJustify = item.DesignResponsibilityJustify,
                // GENERAL
                Description = item.Description,
                AuditLanguage = item.AuditLanguage,
                CurrentCertificationsExpiration = item.CurrentCertificationsExpiration,
                CurrentStandards = item.CurrentStandards,
                CurrentCertificationsBy = item.CurrentCertificationsBy,
                OutsourcedProcess = item.OutsourcedProcess,
                AnyConsultancy = item.AnyConsultancy,
                AnyConsultancyBy = item.AnyConsultancyBy,
                // INTERNAL
                SalesDate = item.SalesDate,
                SalesComments = item.SalesComments,
                ReviewDate = item.ReviewDate,
                ReviewJustification = item.ReviewJustification,
                ReviewComments = item.ReviewComments,
                UserSales = item.UserSales,
                UserReviewer = item.UserReviewer,
                HistoricalDataJSON = item.HistoricalDataJSON,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                // RELATIONS
                Organization = item.Organization != null
                    ? OrganizationMapping.OrganizationToItemListDto(item.Organization)
                    : null,
                AuditCycle = item.AuditCycle != null
                    ? AuditCycleMapping.AuditCycleToItemListDto(item.AuditCycle)
                    : null,
                Standard = item.Standard != null
                    ? StandardMapping.StandardToItemListDto(item.Standard)
                    : null,
                ADCs = item.ADCs != null
                    ? ADCMapping.ADCToListDto(item.ADCs).ToList()
                    : null,
                Nacecodes = item.NaceCodes != null
                    ? NaceCodeMapping.NaceCodeToListDto(item.NaceCodes).ToList()
                    : null,
                Contacts = item.Contacts != null
                    ? ContactMapping.ContactToListDto(
                        item.Contacts.OrderByDescending(c => c.IsMainContact)
                            .ThenBy(c => c.FirstName)
                        ).ToList()
                    : null,
                Sites = item.Sites != null
                    ? SiteMapping.SiteToListDto(
                        item.Sites.OrderByDescending(s => s.IsMainSite)
                            .ThenBy(s => s.Description)
                        ).ToList()
                    : null,
                Notes = item.Notes != null
                    ? NoteMapping.NotesToListDto(
                        item.Notes.OrderByDescending(n => n.Created)
                        ).ToList()
                    : null
            };
        } // AppFormToItemDetailDto

        public static AppForm ItemCreateDtoToAppForm(AppFormCreateDto item)
        {
            return new AppForm
            {
                OrganizationID = item.OrganizationID,
                AuditCycleID = item.AuditCycleID,
                UpdatedUser = item.UpdatedUser,
            };
        } // ItemCreateDtoToAppForm

        public static AppForm ItemUpdateDtoToAppForm(AppFormUpdateDto item)
        {
            return new AppForm
            {
                ID = item.ID,                
                StandardID = item.StandardID,
                // 9K
                ActivitiesScope = item.ActivitiesScope,
                ProcessServicesCount = item.ProcessServicesCount,
                ProcessServicesDescription = item.ProcessServicesDescription,
                LegalRequirements = item.LegalRequirements,
                AnyCriticalComplaint = item.AnyCriticalComplaint,
                CriticalComplaintComments = item.CriticalComplaintComments,
                AutomationLevelPercent = item.AutomationLevelPercent,
                AutomationLevelJustification = item.AutomationLevelJustification,
                IsDesignResponsibility = item.IsDesignResponsibility,
                DesignResponsibilityJustify = item.DesignResponsibilityJustify,
                // GENERAL
                Description = item.Description,
                AuditLanguage = item.AuditLanguage,
                CurrentCertificationsExpiration = item.CurrentCertificationsExpiration,
                CurrentStandards = item.CurrentStandards,
                CurrentCertificationsBy = item.CurrentCertificationsBy,
                OutsourcedProcess = item.OutsourcedProcess,
                AnyConsultancy = item.AnyConsultancy,
                AnyConsultancyBy = item.AnyConsultancyBy,
                //SalesDate = item.SalesDate,
                SalesComments = item.SalesComments,
                //ReviewDate = item.ReviewDate,
                ReviewJustification = item.ReviewJustification,
                ReviewComments = item.ReviewComments,
                Status = item.Status,
                UpdatedUser = item.UpdatedUser
            };
        } // ItemUpdateDtoToAppForm

        public static AppForm ItemDeleteDtoToAppForm(AppFormDeleteDto item)
        {
            return new AppForm
            {
                ID = item.ID,
                UpdatedUser = item.UpdatedUser
            };
        } // ItemDeleteDtoToAppForm

    }
}