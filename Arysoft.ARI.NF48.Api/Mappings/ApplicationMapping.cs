using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class ApplicationMapping
    {
        public static IEnumerable<ApplicationItemListDto> ApplicationsToListDto(IEnumerable<Application> items)
        {
            var itemsDto = new List<ApplicationItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(ApplicationToItemListDto(item));
            }

            return itemsDto;
        } // ApplicationsToListDto

        public static ApplicationItemListDto ApplicationToItemListDto(Application item)
        {
            return new ApplicationItemListDto
            {
                ID = item.ID,
                OrganizationName = item.Organization.Name,
                StandardName = item.Standard.Name,
                // Specific
                NaceCodeName = item.NaceCode != null ? item.NaceCode.Description : string.Empty,
                ProcessScope = item.ProcessScope,
                Services = item.Services,
                AnyCriticalComplaint = item.AnyCriticalComplaint,
                IsDesignResponsibility = item.IsDesignResponsibility,
                // General
                AuditLanguage = item.AuditLanguage,
                AnyConsultancy = item.AnyConsultancy,
                Status = item.Status,
                Created = item.Created
            };
        } // ApplicationToItemListDto

        public static ApplicationItemDetailDto ApplicationToItemDetailDto(Application item)
        {   
            return new ApplicationItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                StandardID = item.StandardID,
                // Specific
                NaceCodeID = item.NaceCodeID,
                RiskLevelID = item.RiskLevelID,
                ProcessScope = item.ProcessScope,
                NumProcess = item.NumProcess,
                Services = item.Services,
                LegalRequirements = item.LegalRequirements,
                AnyCriticalComplaint = item.AnyCriticalComplaint,
                CriticalComplaintComments = item.CriticalComplaintComments,
                AutomationLevel = item.AutomationLevel,
                IsDesignResponsibility = item.IsDesignResponsibility,
                DesignResponsibilityJustify = item.DesignResponsibilityJustify,
                // General
                AuditLanguage = item.AuditLanguage,
                CurrentCertificationExpirationDate = item.CurrentCertificationExpirationDate,
                CurrentCertificationBy = item.CurrentCertificationBy,
                CurrentStandards = item.CurrentStandards,
                TotalEmployes = item.TotalEmployes,
                OutsourcedProcess = item.OutsourcedProcess,
                AnyConsultancy = item.AnyConsultancy,
                AnyConsultancyBy = item.AnyConsultancyBy,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                // Relations
                Organization = OrganizationMapping.OrganizationToItemListDto(item.Organization),
            };
        } // ApplicationToItemDetailDto

        public static Application ItemEditDtoToApplication(ApplicationPutDto itemDto)
        {
            return new Application
            {
                ID = itemDto.ID,
                OrganizationID = itemDto.OrganizationID,
                StandardID = itemDto.StandardID,
                // Specific
                NaceCodeID = itemDto.NaceCodeID,
                RiskLevelID = itemDto.RiskLevelID,
                ProcessScope = itemDto.ProcessScope,
                NumProcess = itemDto.NumProcess,
                Services = itemDto.Services,
                LegalRequirements = itemDto.LegalRequirements,
                AnyCriticalComplaint = itemDto.AnyCriticalComplaint,
                CriticalComplaintComments = itemDto.CriticalComplaintComments,
                AutomationLevel = itemDto.AutomationLevel,
                IsDesignResponsibility = itemDto.IsDesignResponsibility,
                DesignResponsibilityJustify = itemDto.DesignResponsibilityJustify,
                // General
                AuditLanguage = itemDto.AuditLanguage,
                CurrentCertificationExpirationDate = itemDto.CurrentCertificationExpirationDate,
                CurrentCertificationBy = itemDto.CurrentCertificationBy,
                CurrentStandards = itemDto.CurrentStandards,
                TotalEmployes = itemDto.TotalEmployes,
                OutsourcedProcess = itemDto.OutsourcedProcess,
                AnyConsultancy = itemDto.AnyConsultancy,
                AnyConsultancyBy = itemDto.AnyConsultancyBy,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToApplication
    } 
}