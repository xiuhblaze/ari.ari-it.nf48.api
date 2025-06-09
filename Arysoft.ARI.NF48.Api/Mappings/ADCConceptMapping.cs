using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class ADCConceptMapping
    {
        public static IEnumerable<ADCConceptItemListDto> ADCConceptToListDto(IEnumerable<ADCConcept> items)
        {
            var itemsDto = new List<ADCConceptItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(ADCConceptToItemListDto(item));
            }

            return itemsDto;
        } // ADCConceptToListDto

        public static ADCConceptItemListDto ADCConceptToItemListDto(ADCConcept item)
        {
            return new ADCConceptItemListDto
            {
                ID = item.ID,
                StandardID = item.StandardID,
                Description = item.Description,
                IndexSort = item.IndexSort,
                WhenTrue = item.WhenTrue,
                Increase = item.Increase,
                Decrease = item.Decrease,
                IncreaseUnit = item.IncreaseUnit,
                DecreaseUnit = item.DecreaseUnit,
                ToFinalTiming = item.ToFinalTiming,
                Status = item.Status,
                StandardName = item.Standard != null
                    ? item.Standard.Name
                    : string.Empty
            };
        } // ADCConceptToItemListDto

        public static ADCConceptItemDetailDto ADCConceptToItemDetailDto(ADCConcept item)
        {
            return new ADCConceptItemDetailDto
            {
                ID = item.ID,
                StandardID = item.StandardID,
                Description = item.Description,
                IndexSort = item.IndexSort,
                WhenTrue = item.WhenTrue,
                Increase = item.Increase,
                Decrease = item.Decrease,
                IncreaseUnit = item.IncreaseUnit,
                DecreaseUnit = item.DecreaseUnit,
                ToFinalTiming = item.ToFinalTiming,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Standard = item.Standard != null
                    ? StandardMapping.StandardToItemListDto(item.Standard)
                    : null
            };
        } // ADCConceptToItemDetailDto

        public static ADCConcept ItemCreateDtoToADCConcept(ADCConceptItemCreateDto itemDto)
        {
            return new ADCConcept
            {
                StandardID = itemDto.StandardID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemCreateDtoToADCConcept

        public static ADCConcept ItemUpdateDtoToADCConcept(ADCConceptItemUpdateDto itemDto)
        {
            return new ADCConcept
            {
                ID = itemDto.ID,
                IndexSort = itemDto.IndexSort,
                Description = itemDto.Description,
                WhenTrue = itemDto.WhenTrue,
                Increase = itemDto.Increase,
                Decrease = itemDto.Decrease,
                IncreaseUnit = itemDto.IncreaseUnit,
                DecreaseUnit = itemDto.DecreaseUnit,
                ToFinalTiming = itemDto.ToFinalTiming,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemUpdateDtoToADCConcept

        public static ADCConcept ItemDeleteDtoToADCConcept(ADCConceptItemDeleteDto itemDto)
        {
            return new ADCConcept
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToADCConcept
    }
}