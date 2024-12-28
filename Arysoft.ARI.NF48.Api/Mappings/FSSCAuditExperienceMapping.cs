using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class FSSCAuditExperienceMapping
    {
        public static IEnumerable<FSSCAuditExperienceItemListDto> FSSCAuditExperienceToListDto(IEnumerable<FSSCAuditExperience> items)
        {
            var itemsDto = new List<FSSCAuditExperienceItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(FSSCAuditExperienceToItemListDto(item));
            }

            return itemsDto;
        } // FSSCAuditExperienceToListDto

        public static FSSCAuditExperienceItemListDto FSSCAuditExperienceToItemListDto(FSSCAuditExperience item)
        {
            return new FSSCAuditExperienceItemListDto
            {
                ID = item.ID,
                FSSCAuditorActivityID = item.FSSCAuditorActivityID,
                FSSCJobExperienceID = item.FSSCJobExperienceID,
                Description = item.Description,
                Status = item.Status
            };
        } // FSSCAuditExperienceToItemListDto

        public static FSSCAuditExperienceItemDetailDto FSSCAuditExperienceToItemDetailDto(FSSCAuditExperience item)
        {
            return new FSSCAuditExperienceItemDetailDto
            {
                ID = item.ID,
                FSSCAuditorActivityID = item.FSSCAuditorActivityID,
                FSSCJobExperienceID = item.FSSCJobExperienceID,
                Description = item.Description,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                FSSCAuditorActivity = item.FSSCAuditorActivity != null
                    ? FSSCAuditorActivityMapping.FSSCAuditorActivityToItemListDto(item.FSSCAuditorActivity)
                    : null,
                FSSCJobExperience = item.FSSCJobExperience != null
                    ? FSSCJobExperienceMapping.FSSCJobExperienceToItemListDto(item.FSSCJobExperience)
                    : null
            };
        } // FSSCAuditExperienceToItemDetailDto

        public static FSSCAuditExperience ItemAddDtoToFSSCAuditExperience(FSSCAuditExperiencePostDto itemDto)
        {
            return new FSSCAuditExperience
            {
                FSSCAuditorActivityID = itemDto.FSSCAuditorActivityID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToFSSCAuditExperience

        public static FSSCAuditExperience ItemEditDtoToNASSCAuditExperience(FSSCAuditExperiencePutDto itemDto)
        {
            return new FSSCAuditExperience
            {
                ID = itemDto.ID,
                FSSCJobExperienceID = itemDto.FSSCJobExperienceID,
                Description = itemDto.Description,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToNASSCAuditExperience

        public static FSSCAuditExperience ItemDeleteDtoToFSSCAuditExperience(FSSCAuditExperienceDeleteDto itemDto)
        {
            return new FSSCAuditExperience
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToFSSCAuditExperience
    }
}