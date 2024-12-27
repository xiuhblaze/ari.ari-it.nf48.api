using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class NSSCAuditExperienceMapping
    {
        public static IEnumerable<NSSCAuditExperienceItemListDto> NSSCAuditExperienceToListDto(IEnumerable<NSSCAuditExperience> items)
        {
            var itemsDto = new List<NSSCAuditExperienceItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(NSSCAuditExperienceToItemListDto(item));
            }

            return itemsDto;
        } // NSSCAuditExperienceToListDto

        public static NSSCAuditExperienceItemListDto NSSCAuditExperienceToItemListDto(NSSCAuditExperience item)
        {
            return new NSSCAuditExperienceItemListDto
            {
                ID = item.ID,
                NSSCAuditorActivityID = item.NSSCAuditorActivityID,
                NSSCJobExperienceID = item.NSSCJobExperienceID,
                Description = item.Description,
                Status = item.Status
            };
        } // NSSCAuditExperienceToItemListDto

        public static NSSCAuditExperienceItemDetailDto NSSCAuditExperienceToItemDetailDto(NSSCAuditExperience item)
        {
            return new NSSCAuditExperienceItemDetailDto
            {
                ID = item.ID,
                NSSCAuditorActivityID = item.NSSCAuditorActivityID,
                NSSCJobExperienceID = item.NSSCJobExperienceID,
                Description = item.Description,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                NSSCAuditorActivity = item.NSSCAuditorActivity != null
                    ? NSSCAuditorActivityMapping.NSSCAuditorActivityToItemListDto(item.NSSCAuditorActivity)
                    : null,
                NSSCJobExperience = item.NSSCJobExperience != null
                    ? NSSCJobExperienceMapping.NSSCJobExperienceToItemListDto(item.NSSCJobExperience)
                    : null
            };
        } // NSSCAuditExperienceToItemDetailDto

        public static NSSCAuditExperience ItemAddDtoToNSSCAuditExperience(NSSCAuditExperiencePostDto itemDto)
        {
            return new NSSCAuditExperience
            {
                NSSCAuditorActivityID = itemDto.NSSCAuditorActivityID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToNSSCAuditExperience

        public static NSSCAuditExperience ItemEditDtoToNASSCAuditExperience(NSSCAuditExperiencePutDto itemDto)
        {
            return new NSSCAuditExperience
            {
                ID = itemDto.ID,
                NSSCJobExperienceID = itemDto.NSSCJobExperienceID,
                Description = itemDto.Description,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToNASSCAuditExperience

        public static NSSCAuditExperience ItemDeleteDtoToNSSCAuditExperience(NSSCAuditExperienceDeleteDto itemDto)
        {
            return new NSSCAuditExperience
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToNSSCAuditExperience
    }
}