using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class ADCSiteMapping
    {
        public static IEnumerable<ADCSiteItemListDto> ADCSiteToListDto(IEnumerable<ADCSite> items)
        {
            var itemsDto = new List<ADCSiteItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(ADCSiteToItemListDto(item));
            }

            return itemsDto;
        } // ADCSiteToListDto

        public static ADCSiteItemListDto ADCSiteToItemListDto(ADCSite item)
        {
            return new ADCSiteItemListDto
            {
                ID = item.ID,
                ADCID = item.ADCID,
                SiteID = item.SiteID,
                InitialMD5 = item.InitialMD5,
                Employees = item.Employees,
                TotalInitial = item.TotalInitial,
                MD11 = item.MD11,
                Surveillance = item.Surveillance,
                RR = item.RR,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                ADCDescription = item.ADC != null 
                    ? item.ADC.Description 
                    : string.Empty,
                SiteDescription = item.Site != null 
                    ? item.Site.Description 
                    : string.Empty,
                ADCConceptValuesCount = item.ADCConceptValues != null
                    ? item.ADCConceptValues.Count
                    : 0,
            };
        } // ADCSiteToItemListDto

        public static ADCSiteItemDetailDto ADCSiteToItemDetailDto(ADCSite item)
        {
            return new ADCSiteItemDetailDto
            {
                ID = item.ID,
                ADCID = item.ADCID,
                SiteID = item.SiteID,
                InitialMD5 = item.InitialMD5,
                Employees = item.Employees,
                TotalInitial = item.TotalInitial,
                MD11 = item.MD11,
                Surveillance = item.Surveillance,
                RR = item.RR,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Site = item.Site != null
                    ? SiteMapping.SiteToItemListDto(item.Site)
                    : null,
                ADC = item.ADC != null
                    ? ADCMapping.ADCToItemListDto(item.ADC)
                    : null,
                ADCConceptValues = item.ADCConceptValues != null
                    ? ADCConceptValueMapping.ADCConceptValueToListDto(item.ADCConceptValues).ToList()
                    : null
            };
        } // ADCSiteToItemDetailDto

        public static ADCSite ItemCreateDtoToADCSite(ADCSiteItemCreateDto itemDto)
        {
            return new ADCSite
            {
                ADCID = itemDto.ADCID,
                UpdatedUser = itemDto.UpdatedUser,
            };
        } // ItemCreateDtoToADCSite

        public static ADCSite ItemUpdateDtoToADCSite(ADCSiteItemUpdateDto itemDto)
        {
            return new ADCSite
            {
                ID = itemDto.ID,
                SiteID = itemDto.SiteID,
                MD11 = itemDto.MD11,
                Surveillance = itemDto.Surveillance,
                RR = itemDto.RR,
                ExtraInfo = itemDto.ExtraInfo,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemUpdateDtoToADCSite

        public static ADCSite ItemDeleteDtoToADCSite(ADCSiteItemDeleteDto itemDto)
        {
            return new ADCSite
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToADCSite
    }
}