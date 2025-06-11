using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class ADCMapping
    {
        public static IEnumerable<ADCItemListDto> ADCToListDto(IEnumerable<ADC> items)
        {
            var itemsDto = new List<ADCItemListDto>();
            foreach (var item in items)
            {
                itemsDto.Add(ADCToItemListDto(item));
            }
            return itemsDto;
        } // ADCToListDto

        public static ADCItemListDto ADCToItemListDto(ADC item)
        {
            return new ADCItemListDto
            {
                ID = item.ID,
                AppFormID = item.AppFormID,
                Description = item.Description,
                TotalEmployees = item.TotalEmployees,
                TotalInitial = item.TotalInitial,
                TotalMD11 = item.TotalMD11,
                TotalSurveillance = item.TotalSurveillance,
                TotalRR = item.TotalRR,
                UserCreate = item.UserCreate,
                UserReviewer = item.UserReviewer,
                ReviewDate = item.ReviewDate,
                ReviewComments = item.ReviewComments,
                ActiveDate = item.ActiveDate,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                // RELATIONS
                AppFormOrganizationName = item.AppForm?.Organization?.Name ?? string.Empty,
                AppFormAuditCycleName = item.AppForm?.AuditCycle?.Name ?? string.Empty,
                AppFormStandardName = item.AppForm?.Standard?.Name ?? string.Empty,
                NotesCount = item.Notes?.Count() ?? 0,
                ADCSitesCount = item.ADCSites?.Count() ?? 0
            };
        } // ADCToItemListDto

        public static ADCItemDetailDto ADCToItemDetailDto(ADC item)
        {
            return new ADCItemDetailDto
            {
                ID = item.ID,
                AppFormID = item.AppFormID,
                Description = item.Description,
                TotalEmployees = item.TotalEmployees,
                TotalInitial = item.TotalInitial,
                TotalMD11 = item.TotalMD11,
                TotalSurveillance = item.TotalSurveillance,
                TotalRR = item.TotalRR,
                UserCreate = item.UserCreate,
                UserReviewer = item.UserReviewer,
                ReviewDate = item.ReviewDate,
                ReviewComments = item.ReviewComments,
                ActiveDate = item.ActiveDate,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                // RELATIONS
                AppForm = item.AppForm != null
                    ? AppFormMapping.AppFormToItemListDto(item.AppForm)
                    : null,
                ADCSites = item.ADCSites != null
                    ? ADCSiteMapping.ADCSiteToListDto(item.ADCSites).ToList()
                    : null,
                Notes = item.Notes != null
                    ? NoteMapping.NotesToListDto(item.Notes).ToList()
                    : null
            };
        } // ADCToItemDetailDto

        public static ADC ItemCreateDtoToADC(ADCCreateDto itemDto)
        {
            return new ADC
            {
                AppFormID = itemDto.AppFormID,
                UpdatedUser = itemDto.UpdatedUser,
            };
        } // ItemCreateDtoToADC

        public static ADC ItemUpdateDtoToADC(ADCUpdateDto itemDto)
        {
            return new ADC
            {
                ID = itemDto.ID,
                Description = itemDto.Description,                
                TotalInitial = itemDto.TotalInitial,
                TotalMD11 = itemDto.TotalMD11,
                TotalSurveillance = itemDto.TotalSurveillance,
                TotalRR = itemDto.TotalRR,
                ReviewComments = itemDto.ReviewComments,                
                ExtraInfo = itemDto.ExtraInfo,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemUpdateDtoToADC

        public static ADC ItemDeleteDtoToADC(ADCDeleteDto itemDto)
        {
            return new ADC
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToADC
    }
}