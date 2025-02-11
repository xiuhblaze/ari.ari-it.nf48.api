using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class DayCalculationConceptApplicationMapping
    {
        public static IEnumerable<DayCalculationConceptApplicationItemListDto> DayCalculationConceptApplicationToListDto(IEnumerable<DayCalculationConceptApplication> items)
        { 
            var itemsDto = new List<DayCalculationConceptApplicationItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(DayCalculationConceptApplicationToItemListDto(item));
            }

            return itemsDto;
        } // DayCalculationConceptApplicationToListDto

        public static DayCalculationConceptApplicationItemListDto DayCalculationConceptApplicationToItemListDto(DayCalculationConceptApplication item)
        {
            return new DayCalculationConceptApplicationItemListDto
            {
                ID = item.ID,
                //DayCalculationConceptID = item.DayCalculationConceptID,
                //ApplicationID = item.ApplicationID,
                Value = item.Value,
                Justification = item.Justification,
                ValueApproved = item.ValueApproved,
                JustificationApproved = item.JustificationApproved,
                Unit = item.Unit,
                Status = item.Status,
                DayCalculationConceptDescription = item.DayCalculationConcept != null
                    ? item.DayCalculationConcept.Description 
                    : string.Empty
            };
        } // DayCalculationConceptApplicationToItemListDto

        public static DayCalculationConceptApplicationItemDetailDto DayCalculationConceptApplicationToItemDetailDto(DayCalculationConceptApplication item)
        {
            return new DayCalculationConceptApplicationItemDetailDto
            {
                ID = item.ID,
                DayCalculationConceptID = item.DayCalculationConceptID,
                ApplicationID = item.ApplicationID,
                Value = item.Value,
                Justification = item.Justification,
                ValueApproved = item.ValueApproved,
                JustificationApproved = item.JustificationApproved,
                Unit = item.Unit,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,

                DayCalculationConcept = item.DayCalculationConcept != null
                    ? DayCalculationConceptMapping.DayCalculationConceptToItemListDto(item.DayCalculationConcept)
                    : null,
                Application = item.Application != null
                    ? ApplicationMapping.ApplicationToItemListDto(item.Application) 
                    : null
            };
        } // DayCalculationConceptApplicationToItemDetailDto

        public static DayCalculationConceptApplication ItemAddDtoToDayCalculationConceptApplication(DayCalculationConceptApplicationPostDto itemDto)
        {
            return new DayCalculationConceptApplication
            {
                DayCalculationConceptID = itemDto.DayCalculationConceptID,
                ApplicationID = itemDto.ApplicationID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToDayCalculationConceptApplication

        public static DayCalculationConceptApplication ItemEditDtoToDayCalculationConceptApplication(DayCalculationConceptApplicationPutDto itemDto)
        {
            return new DayCalculationConceptApplication
            {
                ID = itemDto.ID,                
                Value = itemDto.Value,
                Justification = itemDto.Justification,
                ValueApproved = itemDto.ValueApproved,
                JustificationApproved = itemDto.JustificationApproved,
                Unit = itemDto.Unit,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToDayCalculationConceptApplication

        public static DayCalculationConceptApplication ItemDeleteToDayCalculationConceptApplication(DayCalculationConceptApplicationDeleteDto itemDto)
        {
            return new DayCalculationConceptApplication
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        }
    }
}