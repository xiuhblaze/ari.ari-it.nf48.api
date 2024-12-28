using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class FSSCJobExperienceMapping
    {
        public static IEnumerable<FSSCJobExperienceItemListDto> FSSCJobExperienceToListDto(IEnumerable<FSSCJobExperience> items)
        { 
            var itemsDto = new List<FSSCJobExperienceItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(FSSCJobExperienceToItemListDto(item));
            }

            return itemsDto;
        } // FSSCJobExperienceToListDto

        public static FSSCJobExperienceItemListDto FSSCJobExperienceToItemListDto(FSSCJobExperience item)
        {
            return new FSSCJobExperienceItemListDto
            {
                ID = item.ID,
                FSSCAuditorActivityID = item.FSSCAuditorActivityID,
                Description = item.Description,
                Status = item.Status
            };
        } // FSSCJobExperienceToItemListDto

        public static FSSCJobExperienceItemDetailDto FSSCJobExperienceToItemDetailDto(FSSCJobExperience item)
        {
            return new FSSCJobExperienceItemDetailDto
            {
                ID = item.ID,
                FSSCAuditorActivityID = item.FSSCAuditorActivityID,
                Description = item.Description,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                FSSCAuditorActivity = item.FSSCAuditorActivity != null
                    ? FSSCAuditorActivityMapping.FSSCAuditorActivityToItemListDto(item.FSSCAuditorActivity) 
                    : null
            };
        } // FSSCJobExperienceToItemDetailDto

        public static FSSCJobExperience ItemAddDtoToFSSCJobExperience(FSSCJobExperiencePostDto itemDto)
        {
            return new FSSCJobExperience
            {
                FSSCAuditorActivityID = itemDto.FSSCAuditorActivityID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToFSSCJobExperience

        public static FSSCJobExperience ItemEditDtoToFSSCJobExperience(FSSCJobExperiencePutDto itemDto)
        {
            return new FSSCJobExperience
            { 
                ID = itemDto.ID,
                Description = itemDto.Description,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEdiDtoToFSSCJobExperience

        public static FSSCJobExperience ItemDeleteDtoToFSSCJobExperience(FSSCJobExperienceDeleteDto itemDto)
        {
            return new FSSCJobExperience
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToFSSCJobExperience
    }
}