using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class NSSCAuditorActivityMapping
    {
        public static IEnumerable<NSSCAuditorActivityItemListDto> NSSCAuditorActivityToListDto(IEnumerable<NSSCAuditorActivity> items)
        {
            var itemsDto = new List<NSSCAuditorActivityItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(NSSCAuditorActivityToItemListDto(item));
            }

            return itemsDto;
        } // NSSCAuditorActivityToListDto

        public static NSSCAuditorActivityItemListDto NSSCAuditorActivityToItemListDto(NSSCAuditorActivity item)
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

            return new NSSCAuditorActivityItemListDto
            {
                ID = item.ID,
                AuditorID = item.AuditorID,
                NSSCActivityID = item.NSSCActivityID,
                Education = item.Education,
                LegalRequirements = item.LegalRequirements,
                SpecificTraining = item.SpecificTraining,
                Comments = item.Comments,
                Status = item.Status,
                AuditorFullName = auditorFullName,
                Activity = item.NSSCActivity != null 
                    ? item.NSSCActivity.Name 
                    : string.Empty,
                NSSCJobExperiencesCount = item.NSSCJobExperiences != null
                    ? item.NSSCJobExperiences.Count
                    : 0,
                NSSCAuditExperienceSCount = item.NSSCAuditExperiences != null
                    ? item.NSSCAuditExperiences.Count
                    : 0
            };
        } // NSSCAuditorActivityToItemListDto

        public static NSSCAuditorActivityItemDetailDto NSSCAuditorActivityToItemDetailDto(NSSCAuditorActivity item)
        {
            return new NSSCAuditorActivityItemDetailDto
            {
                ID = item.ID,
                AuditorID = item.AuditorID,
                NSSCActivityID = item.NSSCActivityID,
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
                NSSCActivity = item.NSSCActivity != null
                    ? NSSCActivityMapping.NSSCActivityToItemListDto(item.NSSCActivity)
                    : null
            };
        } // NSSCAuditorActivityToItemDetailDto

        public static NSSCAuditorActivity ItemAddDtoToNSSCAuditorActivity(NSSCAuditorActivityPostDto itemDto)
        {
            return new NSSCAuditorActivity
            {
                AuditorID = itemDto.AuditorID,
                NSSCActivityID = itemDto.NSSCActivityID,
                UpdatedUser = itemDto.UpdatedUser

            };
        } // ItemAddDtoToNSSCAuditorActivity

        public static NSSCAuditorActivity ItemEditDtoToNSSCAuditorActivity(NSSCAuditorActivityPutDto itemDto)
        {
            return new NSSCAuditorActivity
            { 
                ID = itemDto.ID,
                Education = itemDto.Education,
                LegalRequirements = itemDto.LegalRequirements,
                SpecificTraining = itemDto.SpecificTraining,
                Comments = itemDto.Comments,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToNSSCAuditorActivity

        public static NSSCAuditorActivity ItemDeleteDtoToNSSCAuditorActivity(NSSCAuditorActivityDeleteDto itemDto)
        {
            return new NSSCAuditorActivity
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToNSSCAuditorActivity
    }
}