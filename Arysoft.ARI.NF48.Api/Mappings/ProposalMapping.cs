using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class ProposalMapping
    {
        public static IEnumerable<ProposalItemListDto> ProposalToListDto(IEnumerable<Proposal> items)
        {
            var itemsDto = new List<ProposalItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(ProposalToItemListDto(item));
            }
            return itemsDto;
        } // ProposalToListDto

        public static ProposalItemListDto ProposalToItemListDto(Proposal item)
        {
            return new ProposalItemListDto
            {
                ID = item.ID,
                AuditCycleID = item.AuditCycleID,
                AppFormID = item.AppFormID,
                ADCID = item.ADCID,
                MD5ID = item.MD5ID,
                ActivitiesScope = item.ActivitiesScope,
                TotalEmployees = item.TotalEmployees,
                Justification = item.Justification,
                SignerName = item.SignerName,
                SignerPosition = item.SignerPosition,
                SendToSignDate = item.SendToSignDate,
                SigendFilename = item.SigendFilename,
                CurrencyCode = item.CurrencyCode,
                UserCreates = item.UserCreates,
                UserReview = item.UserReview,
                ReviewDate = item.ReviewDate,
                ActiveDate = item.ActiveDate,
                Status = item.Status,
                // INTERNAL
                HistoricalDataJSON = item.HistoricalDataJSON,
                // RELATIONS
                OrganizationName = item.AuditCycle?.Organization?.Name ?? string.Empty,
                AuditCycleName = item.AuditCycle?.Name ?? string.Empty,
                AppFormStandardName = item.AppForm?.Standard?.Name ?? string.Empty,
                MD5Range = item.MD5 != null 
                    ? $"{item.MD5?.StartValue ?? 0}-${item.MD5?.EndValue ?? 0}"
                    : string.Empty,
                NotesCount = item.Notes.Count,
                // NOT MAPPED
                Alerts = item.Alerts,
            };
        } // ProposalToItemListDto

        public static ProposalItemDetailDto ProposalToItemDetailDto(Proposal item)
        {
            return new ProposalItemDetailDto
            {
                ID = item.ID,
                AuditCycleID = item.AuditCycleID,
                AppFormID = item.AppFormID,
                ADCID = item.ADCID,
                MD5ID = item.MD5ID,
                ActivitiesScope = item.ActivitiesScope,
                TotalEmployees = item.TotalEmployees,
                Justification = item.Justification,
                SignerName = item.SignerName,
                SignerPosition = item.SignerPosition,
                SendToSignDate = item.SendToSignDate,
                SigendFilename = item.SigendFilename,
                CurrencyCode = item.CurrencyCode,
                UserCreates = item.UserCreates,
                UserReview = item.UserReview,
                ReviewDate = item.ReviewDate,
                ActiveDate = item.ActiveDate,
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
                ADC = item.ADC != null
                    ? ADCMapping.ADCToItemListDto(item.ADC)
                    : null,
                MD5 = item.MD5 != null
                    ? MD5Mapping.MD5ToItemListDto(item.MD5)
                    : null,
                // PostalAudits
                Notes = item.Notes != null
                    ? NoteMapping.NotesToListDto(
                        item.Notes.OrderByDescending(n => n.Created)
                        ).ToList()
                    : null,
                // NOT MAPPED
                Alerts = item.Alerts
            };
        } // ProposalToItemDetailDto

        public static Proposal ItemCreateDtoToProposal(ProposalCreateDto itemDto)
        {
            return new Proposal
            {
                AuditCycleID = itemDto.AuditCycleID ?? Guid.Empty,
                AppFormID = itemDto.AppFormID ?? Guid.Empty,
                ADCID = itemDto.ADCID ?? Guid.Empty,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemCreateDtoToProposal

        public static Proposal ItemUpdateDtoToProposal(ProposalUpdateDto itemDto)
        {
            return new Proposal
            {
                ID = itemDto.ID ?? Guid.Empty,
                Justification = itemDto.Justification,
                SignerName = itemDto.SignerName,
                SignerPosition = itemDto.SignerPosition,
                CurrencyCode = itemDto.CurrencyCode,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemUpdateDtoToProposal

        public static Proposal ItemDeleteDtoToProposal(ProposalDeleteDto itemDto)
        {
            return new Proposal
            { 
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToProposal
    }
}