using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

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
                AuditCycleID = item.AuditCycleID,
                AppFormID = item.AppFormID,
                ProposalID = item.ProposalID,
                Description = item.Description,
                TotalEmployees = item.TotalEmployees,
                TotalInitial = item.TotalInitial,
                TotalMD11 = item.TotalMD11,
                TotalSurveillance = item.TotalSurveillance,
                TotalRecertification = item.TotalRecertification,
                ReviewDate = item.ReviewDate,
                ActiveDate = item.ActiveDate,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                // INTERNAL
                HistoricalDataJSON = item.HistoricalDataJSON,
                // RELATIONS
                AuditCycleName = item.AuditCycle?.Name ?? string.Empty,
                AppFormOrganizationName = item.AppForm?.Organization?.Name ?? string.Empty,
                AppFormStandardID = item.AppForm?.StandardID ?? Guid.Empty,
                AppFormStandardName = item.AppForm?.Standard?.Name ?? string.Empty,
                NotesCount = item.Notes?.Count() ?? 0,
                ADCSitesCount = item.ADCSites?.Count() ?? 0,
                HasProposal = item.Proposal != null,
                // NOT MAPPED
                Alerts = item.Alerts
            };
        } // ADCToItemListDto

        public static ADCItemDetailDto ADCToItemDetailDto(ADC item)
        {
            return new ADCItemDetailDto
            {
                ID = item.ID,
                AuditCycleID = item.AuditCycleID,
                AppFormID = item.AppFormID,
                ProposalID = item.ProposalID,
                Description = item.Description,
                TotalEmployees = item.TotalEmployees,
                TotalInitial = item.TotalInitial,
                TotalMD11 = item.TotalMD11,
                TotalSurveillance = item.TotalSurveillance,
                TotalRecertification = item.TotalRecertification,
                ReviewDate = item.ReviewDate,
                ActiveDate = item.ActiveDate,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                // INTERNAL
                HistoricalDataJSON = item.HistoricalDataJSON,
                // RELATIONS
                AuditCycle = item.AuditCycle != null
                    ? AuditCycleMapping.AuditCycleToItemListDto(item.AuditCycle)
                    : null,
                AppForm = item.AppForm != null
                    ? AppFormMapping.AppFormToItemListDto(item.AppForm)
                    : null,
                ADCSites = item.ADCSites != null
                    ? ADCSiteMapping.ADCSiteToListDto(
                        item.ADCSites.OrderByDescending(x => x.Site?.IsMainSite)
                            .ThenBy(x => x.Site?.Description)
                        ).ToList()
                    : null,
                Proposal = item.Proposal != null
                    ? ProposalMapping.ProposalToItemListDto(item.Proposal)
                    : null,
                Notes = item.Notes != null
                    ? NoteMapping.NotesToListDto(
                        item.Notes.OrderByDescending(n => n.Created)
                        ).ToList()
                    : null,
                Alerts = item.Alerts
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
                TotalRecertification = itemDto.TotalRecertification,
                //ReviewComments = itemDto.ReviewComments,                
                ExtraInfo = itemDto.ExtraInfo,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemUpdateDtoToADC

        public static ADC ItemUpdateWithListDtoToADC(ADCWithSiteListUpdateDto itemDto)
        {
            var item = ItemUpdateDtoToADC(itemDto.ADC);

            if (itemDto.ADCSites != null)
            {
                foreach (var siteDto in itemDto.ADCSites)
                {
                    item.ADCSites.Add(ADCSiteMapping.ItemUpdateWithListDtoToADCSite(siteDto));
                }
            }

            return item;
        } // ItemUpdateAllDtoToADC

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