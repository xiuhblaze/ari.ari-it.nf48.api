using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class DayCalculationConceptMapping
    {
        public static IEnumerable<DayCalculationConceptItemListDto> DayCalculationConceptToListDto(IEnumerable<DayCalculationConcept> items)
        {
            var itemsDto = new List<DayCalculationConceptItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(DayCalculationConceptToItemListDto(item));
            }

            return itemsDto;
        } // DayCalculationConceptToListDto

        public static DayCalculationConceptItemListDto DayCalculationConceptToItemListDto(DayCalculationConcept item)
        {
            return new DayCalculationConceptItemListDto
            {
                ID = item.ID,
                StandardName = item.Standard != null 
                    ? item.Standard.Name
                    : string.Empty,
                Description = item.Description,
                Increase = item.Increase,
                Decrease = item.Decrease,
                Unit = item.Unit,
                Status = item.Status
            };
        } // DayCalculationConceptToItemListDto

        public static DayCalculationConceptItemDetailDto DayCalculationConceptToItemDetailDto(DayCalculationConcept item)
        {   
            return new DayCalculationConceptItemDetailDto
            {
                ID = item.ID,
                StandardID = item.StandardID,
                Description = item.Description,
                Increase = item.Increase,
                Decrease = item.Decrease,
                Unit = item.Unit,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,

                Standard = item.Standard != null
                    ? StandardMapping.StandardToItemListDto(item.Standard) 
                    : null
            };
        } // DayCalculationConceptToItemDetailDto

        public static DayCalculationConcept ItemAddDtoToDayCalculationConcept(DayCalculationConceptPostDto itemDto)
        {
            return new DayCalculationConcept
            {
                UpdatedUser = itemDto.UpdatedUser
            };
        }

        public static DayCalculationConcept ItemEditDtoToDayCalculationConcept(DayCalculationConceptPutDto itemDto)
        {
            return new DayCalculationConcept
            {
                ID = itemDto.ID,
                StandardID = itemDto.StandardID,
                Description = itemDto.Description,
                Increase = itemDto.Increase,
                Decrease = itemDto.Decrease,
                Unit = itemDto.Unit,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        }

        public static DayCalculationConcept ItemDeleteToDayCalculationconcept(DayCalculationConceptDeleteDto itemDto)
        {
            return new DayCalculationConcept
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        }
    }
}