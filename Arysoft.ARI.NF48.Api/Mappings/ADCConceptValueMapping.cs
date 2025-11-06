using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class ADCConceptValueMapping
    {
        public static IEnumerable<ADCConceptValueItemListDto> ADCConceptValueToListDto(IEnumerable<ADCConceptValue> items)
        {
            var itemsDto = new List<ADCConceptValueItemListDto>();
            foreach (var item in items)
            {
                itemsDto.Add(ADCConceptValueToItemListDto(item));
            }
            return itemsDto;
        } // ADCConceptValueToListDto

        public static ADCConceptValueItemListDto ADCConceptValueToItemListDto(ADCConceptValue item)
        {
            return new ADCConceptValueItemListDto
            {
                ID = item.ID,
                ADCConceptID = item.ADCConceptID,
                ADCSiteID = item.ADCSiteID,
                CheckValue = item.CheckValue,
                Value = item.Value,
                Justification = item.Justification,
                //ValueApproved = item.ValueApproved,
                //JustificationApproved = item.JustificationApproved,
                ValueUnit = item.ValueUnit,
                Status = item.Status,
                ADCConceptDescription = item.ADCConcept != null 
                    ? item.ADCConcept.Description 
                    : string.Empty,
                ADCConceptWhenTrue = item.ADCConcept?.WhenTrue,
                ADCSiteDescription = item.ADCSite?.Site?.Description ?? string.Empty
            };
        } // ADCConceptValueToItemListDto

        public static ADCConceptValueItemDetailDto ADCConceptValueToItemDetailDto(ADCConceptValue item)
        {
            return new ADCConceptValueItemDetailDto
            {
                ID = item.ID,
                ADCConceptID = item.ADCConceptID,
                ADCSiteID = item.ADCSiteID,
                CheckValue = item.CheckValue,
                Value = item.Value,
                Justification = item.Justification,
                ValueApproved = item.ValueApproved,
                JustificationApproved = item.JustificationApproved,
                ValueUnit = item.ValueUnit,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                ADCConcept = item.ADCConcept != null
                    ? ADCConceptMapping.ADCConceptToItemListDto(item.ADCConcept)
                    : null,
                ADCSite = item.ADCSite != null
                    ? ADCSiteMapping.ADCSiteToItemListDto(item.ADCSite)
                    : null
            };
        } // ADCConceptValueToItemDetailDto

        public static ADCConceptValue ItemCreateDtoToADCConceptValue(ADCConceptValueItemCreateDto itemCreateDto)
        {
            return new ADCConceptValue
            {
                ADCConceptID = itemCreateDto.ADCConceptID,
                ADCSiteID = itemCreateDto.ADCSiteID,                
                UpdatedUser = itemCreateDto.UpdatedUser
            };
        } // ItemCreateDtoToADCConceptValue

        public static ADCConceptValue ItemUpdateDtoToADCConceptValue(ADCConceptValueItemUpdateDto itemUpdateDto)
        {
            return new ADCConceptValue
            {
                ID = itemUpdateDto.ID,
                CheckValue = itemUpdateDto.CheckValue,
                Value = itemUpdateDto.Value,
                Justification = itemUpdateDto.Justification,
                ValueApproved = itemUpdateDto.ValueApproved,
                JustificationApproved = itemUpdateDto.JustificationApproved,
                ValueUnit = itemUpdateDto.ValueUnit,
                Status = itemUpdateDto.Status,
                UpdatedUser = itemUpdateDto.UpdatedUser
            };
        } // ItemUpdateDtoToADCConceptValue

        public static IEnumerable<ADCConceptValue> UpdateListDtoToADCConceptValues(ADCConceptValueListUpdateDto itemsUpdateDto)
        {
            var items = new List<ADCConceptValue>();

            foreach (var itemUpdateDto in itemsUpdateDto.Items)
            {
                items.Add(ItemUpdateDtoToADCConceptValue(itemUpdateDto));
            }

            return items;
        } // UpdateListDtoToADCConceptValues

        public static ADCConceptValue ItemDeleteDtoToADCConceptValue(ADCConceptValueItemDeleteDto itemDeleteDto)
        {
            return new ADCConceptValue
            {
                ID = itemDeleteDto.ID,
                UpdatedUser = itemDeleteDto.UpdatedUser
            };
        } // ItemDeleteDtoToADCConceptValue
    }
}