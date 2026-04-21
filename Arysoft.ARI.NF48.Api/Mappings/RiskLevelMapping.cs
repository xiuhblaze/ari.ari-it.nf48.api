using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class RiskLevelMapping
    {
        public static IEnumerable<RiskLevelItemListDto> RiskLevelToListDto(IEnumerable<RiskLevel> items)
        { 
            var itemsDto = new List<RiskLevelItemListDto>();

            foreach (var item in items)
            { 
                itemsDto.Add(RiskLevelToItemListDto(item));
            }

            return itemsDto;
        } // RiskLevelToListDto

        public static RiskLevelItemListDto RiskLevelToItemListDto(RiskLevel item)
        {
            return new RiskLevelItemListDto
            {
                ID = item.ID,
                StandardID = item.StandardID,
                Category = item.Category,
                BusinessSector = item.BusinessSector,
                Status = item.Status,
                StandardName = item.Standard?.Name
            };
        } // RiskLevelToItemListDto

        public static RiskLevelItemDetailDto RiskLevelToItemDetailDto(RiskLevel item)
        {
            return new RiskLevelItemDetailDto
            {
                ID = item.ID,
                StandardID = item.StandardID,
                Category = item.Category,
                BusinessSector = item.BusinessSector,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,

                Standard = item.Standard != null
                    ? StandardMapping.StandardToItemListDto(item.Standard)
                    : null
            };
        } // RiskLevelToItemDetailDto

        public static RiskLevel ItemCreateDtoToRiskLevel(RiskLevelCreateDto itemDto)
        {
            return new RiskLevel
            {   
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemCreateDtoToRiskLevel

        public static RiskLevel ItemUpdateDtoToRiskLevel(RiskLevelUpdateDto itemDto)
        {
            return new RiskLevel
            {
                ID = itemDto.ID,
                StandardID = itemDto.StandardID,
                Category = itemDto.Category,
                BusinessSector = itemDto.BusinessSector,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToRiskLevel

        public static RiskLevel ItemDeleteDtoToRiskLevel(RiskLevelDeleteDto itemDto)
        {
            return new RiskLevel
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToRiskLevel
    }
}