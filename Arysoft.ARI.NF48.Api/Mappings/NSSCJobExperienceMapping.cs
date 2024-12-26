using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class NSSCJobExperienceMapping
    {
        public static IEnumerable<NSSCJobExperienceItemListDto> NSSCJobExperienceToListDto(IEnumerable<NSSCJobExperience> items)
        { 
            var itemsDto = new List<NSSCJobExperienceItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(NSSCJobExperienceToItemListDto(item));
            }

            return itemsDto;
        } // NSSCJobExperienceToListDto

        public static NSSCJobExperienceItemListDto NSSCJobExperienceToItemListDto(NSSCJobExperience item)
        {
            return new NSSCJobExperienceItemListDto
            {
                ID = item.ID,
                NSSCAuditorActivityID = item.NSSCAuditorActivityID,
                Description = item.Description,
                Status = item.Status
            };
        } // NSSCJobExperienceToItemListDto

        public static NSSCJobExperienceItemDetailDto NSSCJobExperienceToItemDetailDto(NSSCJobExperience item)
        {
            return new NSSCJobExperienceItemDetailDto
            {
                ID = item.ID,
                NSSCAuditorActivityID = item.NSSCAuditorActivityID,
                Description = item.Description,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                NSSCAuditorActivity = item.NSSCAuditorActivity != null
                    ? NSSCAuditorActivityMapping.NSSCAuditorActivityToItemListDto(item.NSSCAuditorActivity) 
                    : null
            };
        } // NSSCJobExperienceToItemDetailDto

        public static NSSCJobExperience ItemAddDtoToNSSCJobExperience(NSSCJobExperiencePostDto itemDto)
        {
            return new NSSCJobExperience
            {
                NSSCAuditorActivityID = itemDto.NSSCAuditorActivityID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToNSSCJobExperience

        public static NSSCJobExperience ItemEditDtoToNSSCJobExperience(NSSCJobExperiencePutDto itemDto)
        {
            return new NSSCJobExperience
            { 
                ID = itemDto.ID,
                Description = itemDto.Description,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEdiDtoToNSSCJobExperience

        public static NSSCJobExperience ItemDeleteDtoToNSSCJobExperience(NSSCJobExperienceDeleteDto itemDto)
        {
            return new NSSCJobExperience
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToNSSCJobExperience
    }
}