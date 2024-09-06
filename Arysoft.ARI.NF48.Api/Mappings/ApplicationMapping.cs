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
                OrganizationName = item.Organization != null 
                    ? item.Organization.Name
                    : string.Empty,
                StandardName = item.Standard != null
                    ? item.Standard.Name
                    : string.Empty,
                // SPECIFIC
                NaceCodeName = item.NaceCode != null 
                    ? item.NaceCode.Description 
                    : string.Empty,
                Category22KName = item.Category22K != null 
                    ? Category22KMapping.Category22KToSummary(item.Category22K) 
                    : string.Empty,
                HACCP = item.HACCP,
                Scope = item.Scope,
                NumberScope = item.NumberScope,
                Seasonality = item.Seasonality,
                Services = item.Services,
                LegalRequirements = item.LegalRequirements,
                AnyCriticalComplaint = item.AnyCriticalComplaint,
                IsDesignResponsibility = item.IsDesignResponsibility,
                // GENERAL
                AuditLanguage = item.AuditLanguage,
                TotalEmployees = item.TotalEmployees,
                AnyConsultancy = item.AnyConsultancy,
                ReviewDate = item.ReviewDate,
                UsernameSales = item.UserSales != null
                    ? Tools.Strings.FullName(item.UserSales.FirstName, null, item.UserSales.LastName)
                    : string.Empty,
                UsernameReviewer = item.UserReviewer != null
                    ? Tools.Strings.FullName(item.UserReviewer.FirstName, null, item.UserReviewer.LastName)
                    : string.Empty,
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
                // SPECIFIC
                NaceCodeID = item.NaceCodeID,
                RiskLevelID = item.RiskLevelID,
                Category22KID = item.Category22KID,
                HACCP = item.HACCP,
                Scope = item.Scope,
                NumberScope = item.NumberScope,
                Seasonality = item.Seasonality,
                Services = item.Services,
                LegalRequirements = item.LegalRequirements,
                AnyCriticalComplaint = item.AnyCriticalComplaint,
                CriticalComplaintComments = item.CriticalComplaintComments,
                AutomationLevel = item.AutomationLevel,
                IsDesignResponsibility = item.IsDesignResponsibility,
                DesignResponsibilityJustify = item.DesignResponsibilityJustify,
                // GENERAL
                AuditLanguage = item.AuditLanguage,
                CurrentCertificationExpirationDate = item.CurrentCertificationExpirationDate,
                CurrentCertificationBy = item.CurrentCertificationBy,
                CurrentStandards = item.CurrentStandards,
                TotalEmployees = item.TotalEmployees,
                OutsourcedProcess = item.OutsourcedProcess,
                AnyConsultancy = item.AnyConsultancy,
                AnyConsultancyBy = item.AnyConsultancyBy,
                ReviewDate = item.ReviewDate,
                ReviewJustification = item.ReviewJustification,
                ReviewComments = item.ReviewComments,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                // Relations
                Organization = item.Organization != null
                    ? OrganizationMapping.OrganizationToItemListDto(item.Organization)
                    : null,
                Standard = item.Standard != null
                    ? StandardMapping.StandardToItemListDto(item.Standard)
                    : null,
                NaceCode = item.NaceCode != null 
                    ? NaceCodeMapping.NaceCodeToItemListDto(item.NaceCode)
                    : null,
                //RiskLevel = RiskLevelMapping.RiskLevelToItemListDto(item.RiskLevel),
                Category22K = item.Category22K != null
                    ? Category22KMapping.Category22KToItemListDto(item.Category22K)
                    : null,
            };
        } // ApplicationToItemDetailDto

        public static Application ItemEditDtoToApplication(ApplicationPutDto itemDto)
        {
            return new Application
            {
                ID = itemDto.ID,
                OrganizationID = itemDto.OrganizationID,
                StandardID = itemDto.StandardID,
                // SPECIFIC
                NaceCodeID = itemDto.NaceCodeID,
                RiskLevelID = itemDto.RiskLevelID,
                Category22KID = itemDto.Category22KID,
                HACCP = itemDto.HACCP,
                Scope = itemDto.Scope,
                NumberScope = itemDto.NumberScope,
                Seasonality = itemDto.Seasonality,
                Services = itemDto.Services,
                LegalRequirements = itemDto.LegalRequirements,
                AnyCriticalComplaint = itemDto.AnyCriticalComplaint,
                CriticalComplaintComments = itemDto.CriticalComplaintComments,
                AutomationLevel = itemDto.AutomationLevel,
                IsDesignResponsibility = itemDto.IsDesignResponsibility,
                DesignResponsibilityJustify = itemDto.DesignResponsibilityJustify,
                // GENERAL
                AuditLanguage = itemDto.AuditLanguage,
                CurrentCertificationExpirationDate = itemDto.CurrentCertificationExpirationDate,
                CurrentCertificationBy = itemDto.CurrentCertificationBy,
                CurrentStandards = itemDto.CurrentStandards,
                TotalEmployees = itemDto.TotalEmployees,
                OutsourcedProcess = itemDto.OutsourcedProcess,
                AnyConsultancy = itemDto.AnyConsultancy,
                AnyConsultancyBy = itemDto.AnyConsultancyBy,
                ReviewDate = itemDto.ReviewDate,
                ReviewJustification = itemDto.ReviewJustification,
                ReviewComments = itemDto.ReviewComments,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToApplication
    } 
}