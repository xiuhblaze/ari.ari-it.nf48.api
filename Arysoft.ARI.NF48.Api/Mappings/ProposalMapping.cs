using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

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
                Justification = item.Justification,
                SignerName = item.SignerName,
                SignerPosition = item.SignerPosition,
                SignedFilename = item.SignedFilename,
                CurrencyCode = item.CurrencyCode,
                ExchangeRate = item.ExchangeRate,
                TaxRate = item.TaxRate,
                IncludeTravelExpenses = item.IncludeTravelExpenses,
                ExtraInfo = item.ExtraInfo,
                // INTERNAL
                CreatedBy = item.CreatedBy,
                ReviewDate = item.ReviewDate,
                ApprovalDate = item.ApprovalDate,
                SignRequestDate = item.SignRequestDate,
                HistoricalDataJSON = item.HistoricalDataJSON,
                Status = item.Status,
                // RELATIONS
                OrganizationName = item.AuditCycle?.Organization?.Name ?? string.Empty,
                AuditCycleName = item.AuditCycle?.Name ?? string.Empty,
                ADCCount = item.ADCs?.Count ?? 0,
                ProposalAuditsCount = item.ProposalAudits?.Count ?? 0,
                NotesCount = item.Notes?.Count ?? 0,
                Standards = item.ADCs != null
                    ? item.ADCs
                        .Select(asd => asd.AppForm.Standard.Name)
                        .ToList()
                    : new List<string>(),
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
                Justification = item.Justification,
                SignerName = item.SignerName,
                SignerPosition = item.SignerPosition,                
                SignedFilename = item.SignedFilename,
                CurrencyCode = item.CurrencyCode,
                ExchangeRate = item.ExchangeRate,
                TaxRate = item.TaxRate,
                IncludeTravelExpenses = item.IncludeTravelExpenses,
                ExtraInfo = item.ExtraInfo,
                // INTERNAL
                CreatedBy = item.CreatedBy,
                ReviewDate = item.ReviewDate,
                ApprovalDate = item.ApprovalDate,
                SignRequestDate = item.SignRequestDate,
                HistoricalDataJSON = item.HistoricalDataJSON,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                // RELATIONS
                AuditCycle = item.AuditCycle != null
                    ? AuditCycleMapping.AuditCycleToItemListDto(item.AuditCycle)
                    : null,
                ADCs = item.ADCs != null
                    ? ADCMapping.ADCToListDto(
                        item.ADCs.OrderByDescending(adc => adc.Created)
                        ).ToList()
                    : null,
                ProposalAudits = item.ProposalAudits != null
                    ? ProposalAuditMapping.ProposalAuditToListDto(
                        item.ProposalAudits.OrderByDescending(pa => pa.AuditStep)
                        ).ToList()
                    : null,
                Notes = item.Notes != null
                    ? NoteMapping.NotesToListDto(
                        item.Notes.OrderByDescending(n => n.Created)
                        ).ToList()
                    : null,
                // RELATIONS EXTRA FIELDS
                Organization = item.AuditCycle?.Organization != null
                    ? OrganizationMapping.OrganizationToItemProposalDto(item.AuditCycle.Organization)
                    : null,
                Sites = item.ADCs != null
                    ? SiteMapping.SiteToListDto(
                            item.ADCs
                                .Where(adc => adc.ADCSites != null)
                                .SelectMany(adc => adc.ADCSites)
                                .Select(ads => ads.Site)
                                .Distinct()
                            )
                        .OrderByDescending(s => s.IsMainSite)
                        .ThenBy(s => s.Description)
                        .ToList()
                    : new List<SiteItemListDto>(),
                Contacts = item.ADCs != null
                    ? ContactMapping.ContactToListDto(
                            item.ADCs
                                .Where(adc => adc.AppForm != null && adc.AppForm.Contacts != null)
                                .SelectMany(adc => adc.AppForm.Contacts)
                                .Distinct()
                            )
                        .OrderByDescending(c => c.IsMainContact)
                        .ThenBy(c => c.FullName)
                        .ToList()
                    : new List<ContactItemListDto>(),
                Scopes = item.ADCs != null
                    ? item.ADCs
                        .Where(adc => adc.AppForm != null)
                        .Select(adc => adc.AppForm.ActivitiesScope)
                        .ToList()
                    : new List<string>(),
                TotalEmployees = item.ADCs != null
                    ? item.ADCs
                        .Where(adc => adc.TotalEmployees.HasValue)
                        .Select(adc => adc.TotalEmployees.Value)
                        .ToList()
                    : new List<int>(),
                ADCSites = item.ADCs != null
                    ? ADCSiteMapping.ADCSiteToListDto(item.ADCs
                        .Where(adc => adc.ADCSites != null)
                        .SelectMany(adc => adc.ADCSites)).ToList()
                    : new List<ADCSiteItemListDto>(),

                // NOT MAPPED
                Alerts = item.Alerts
            };
        } // ProposalToItemDetailDto

        public static Proposal ItemCreateDtoToProposal(ProposalCreateDto itemDto)
        {
            return new Proposal
            {
                AuditCycleID = itemDto.AuditCycleID ?? Guid.Empty,
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
                ExchangeRate = itemDto.ExchangeRate,
                TaxRate = itemDto.TaxRate,
                IncludeTravelExpenses = itemDto.IncludeTravelExpenses,
                ExtraInfo = itemDto.ExtraInfo,
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