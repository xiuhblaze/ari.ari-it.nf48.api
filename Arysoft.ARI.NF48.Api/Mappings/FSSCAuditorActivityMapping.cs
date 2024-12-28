using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class FSSCAuditorActivityMapping
    {
        public static IEnumerable<FSSCAuditorActivityItemListDto> FSSCAuditorActivityToListDto(IEnumerable<FSSCAuditorActivity> items)
        {
            var itemsDto = new List<FSSCAuditorActivityItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(FSSCAuditorActivityToItemListDto(item));
            }

            return itemsDto;
        } // FSSCAuditorActivityToListDto

        public static FSSCAuditorActivityItemListDto FSSCAuditorActivityToItemListDto(FSSCAuditorActivity item)
        {
            string auditorFullName = string.Empty;

            if (item.Auditor != null)
            {
                auditorFullName = item.Auditor.FirstName;
                auditorFullName += string.IsNullOrEmpty(item.Auditor.MiddleName)
                    ? string.Empty
                    : $" {item.Auditor.MiddleName}";
                auditorFullName += string.IsNullOrEmpty(item.Auditor.LastName)
                    ? string.Empty
                    : $" {item.Auditor.LastName}";
            }

            return new FSSCAuditorActivityItemListDto
            {
                ID = item.ID,
                AuditorID = item.AuditorID,
                FSSCActivityID = item.FSSCActivityID,
                Education = item.Education,
                LegalRequirements = item.LegalRequirements,
                SpecificTraining = item.SpecificTraining,
                Comments = item.Comments,
                Status = item.Status,
                AuditorFullName = auditorFullName,
                Activity = item.FSSCActivity != null 
                    ? item.FSSCActivity.Name 
                    : string.Empty,
                FSSCJobExperiencesCount = item.FSSCJobExperiences != null
                    ? item.FSSCJobExperiences.Count
                    : 0,
                FSSCAuditExperienceSCount = item.FSSCAuditExperiences != null
                    ? item.FSSCAuditExperiences.Count
                    : 0
            };
        } // FSSCAuditorActivityToItemListDto

        public static FSSCAuditorActivityItemDetailDto FSSCAuditorActivityToItemDetailDto(FSSCAuditorActivity item)
        {
            return new FSSCAuditorActivityItemDetailDto
            {
                ID = item.ID,
                AuditorID = item.AuditorID,
                FSSCActivityID = item.FSSCActivityID,
                Education = item.Education,
                LegalRequirements = item.LegalRequirements,
                SpecificTraining = item.SpecificTraining,
                Comments = item.Comments,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,

                Auditor = item.Auditor != null
                    ? AuditorMapping.AuditorToItemListDto(item.Auditor) 
                    : null,
                FSSCActivity = item.FSSCActivity != null
                    ? FSSCActivityMapping.FSSCActivityToItemListDto(item.FSSCActivity)
                    : null,
                FSSCJobExperiences = item.FSSCJobExperiences != null
                    ? FSSCJobExperienceMapping.FSSCJobExperienceToListDto(item.FSSCJobExperiences)
                    : null,
                FSSCAuditExperiences = item.FSSCAuditExperiences != null
                    ? FSSCAuditExperienceMapping.FSSCAuditExperienceToListDto(item.FSSCAuditExperiences)
                    : null
            };
        } // FSSCAuditorActivityToItemDetailDto

        public static FSSCAuditorActivity ItemAddDtoToFSSCAuditorActivity(FSSCAuditorActivityPostDto itemDto)
        {
            return new FSSCAuditorActivity
            {
                AuditorID = itemDto.AuditorID,
                FSSCActivityID = itemDto.FSSCActivityID,
                UpdatedUser = itemDto.UpdatedUser

            };
        } // ItemAddDtoToFSSCAuditorActivity

        public static FSSCAuditorActivity ItemEditDtoToFSSCAuditorActivity(FSSCAuditorActivityPutDto itemDto)
        {
            return new FSSCAuditorActivity
            { 
                ID = itemDto.ID,
                Education = itemDto.Education,
                LegalRequirements = itemDto.LegalRequirements,
                SpecificTraining = itemDto.SpecificTraining,
                Comments = itemDto.Comments,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToFSSCAuditorActivity

        public static FSSCAuditorActivity ItemDeleteDtoToFSSCAuditorActivity(FSSCAuditorActivityDeleteDto itemDto)
        {
            return new FSSCAuditorActivity
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToFSSCAuditorActivity
    }
}