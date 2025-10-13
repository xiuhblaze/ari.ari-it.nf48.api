using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Services;
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
                NoEmployees = item.NoEmployees,
                TotalInitial = item.TotalInitial,
                MD11 = item.MD11,
                MD11Filename = item.MD11Filename,
                MD11UploadedBy = item.MD11UploadedBy,
                Total = item.Total,
                Surveillance = item.Surveillance,
                Recertification = item.Recertification,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                ADCDescription = item.ADC != null 
                    ? item.ADC.Description 
                    : string.Empty,
                SiteDescription = item.Site != null 
                    ? item.Site.Description 
                    : string.Empty,
                SiteAddress = item.Site != null
                    ? item.Site.Address
                    : string.Empty,
                ADCConceptValues = item.ADCConceptValues != null
                    ? ADCConceptValueMapping.ADCConceptValueToListDto(item.ADCConceptValues).ToList()
                    : null,
                ADCSiteAudits = item.ADCSiteAudits != null
                    ? ADCSiteAuditMapping.ADCSiteAuditToListDto(item.ADCSiteAudits).ToList()
                    : null,
                Alerts = ADCSiteService.GetAlerts(item),
                IsMultiStandard = ADCSiteService.IsMultiStandard(item.ID)
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
                NoEmployees = item.NoEmployees,
                TotalInitial = item.TotalInitial,
                MD11 = item.MD11,
                MD11Filename = item.MD11Filename,
                MD11UploadedBy = item.MD11UploadedBy,
                Total = item.Total,
                Surveillance = item.Surveillance,
                Recertification = item.Recertification,
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
                    : null,
                ADCSiteAudits = item.ADCSiteAudits != null
                    ? ADCSiteAuditMapping.ADCSiteAuditToListDto(item.ADCSiteAudits
                        .OrderBy(asa => asa.AuditStep))
                    .ToList()
                    : null,
                Alerts = ADCSiteService.GetAlerts(item),
                IsMultiStandard = ADCSiteService.IsMultiStandard(item.ID)
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
                TotalInitial = itemDto.TotalInitial,
                MD11 = itemDto.MD11,
                Total = itemDto.Total,
                Surveillance = itemDto.Surveillance,
                Recertification = itemDto.Recertification,
                ExtraInfo = itemDto.ExtraInfo,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemUpdateDtoToADCSite

        public static IEnumerable<ADCSite> UpdateListDtoToADCSite(ADCSiteListUpdateDto itemsUpdateDto)
        { 
            var items = new List<ADCSite>();

            foreach (var itemDto in itemsUpdateDto.Items)
            {
                items.Add(ItemUpdateDtoToADCSite(itemDto));
            }

            return items;
        } // UpdateListDtoToADCSite

        public static ADCSite ItemUpdateWithListDtoToADCSite(ADCSiteWithCVListUpdateDto itemDto)
        {   
            var item = ItemUpdateDtoToADCSite(itemDto.ADCSite);

            if (itemDto.ADCConceptValues != null) 
            {
                foreach (var cvDto in itemDto.ADCConceptValues)
                {
                    item.ADCConceptValues.Add(ADCConceptValueMapping.ItemUpdateDtoToADCConceptValue(cvDto));
                }
            }

            return item;
        } // ListUpdateDtoToADCSite

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