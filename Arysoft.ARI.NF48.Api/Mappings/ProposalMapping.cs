using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var auditCycles = new List<object>();
            var sitesCount = 0;
            var totalWorkers = 0;

            if (item.ADCs != null)
            {
                foreach(var adc in item.ADCs)
                {
                    if (adc.AuditCycle != null && !string.IsNullOrEmpty(adc.AuditCycle.Name))
                    {
                        auditCycles.Add(new { 
                            adc.AuditCycle.Name,
                            adc.AuditCycle.CycleType,
                            StandardName = adc.Standard.Name
                        });
                    }
                }

                var sites = item.ADCs.Where(adc => adc.ADCSites != null)
                    .SelectMany(adc => adc.ADCSites)
                    .Select(adc => adc.Site)
                    .Distinct();
                
                if (sites.Any())
                {
                    sitesCount = sites.Count();
                    totalWorkers += OrganizationCalculations.GetTotalWorkers(sites.ToList());
                    //foreach (var site in sites)
                    //{
                    //    employeesCount += site.Status == StatusType.Active && site.Shifts.Any()
                    //        ? site.Shifts
                    //            .Where(i => i.Status == StatusType.Active)
                    //            .Sum(i => i.NoEmployees) ?? 0
                    //        : 0;
                    //}
                }
            }

            return new ProposalItemListDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
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
                OrganizationName = item.Organization?.Name ?? string.Empty,
                ADCCount = item.ADCs?.Count ?? 0,
                ProposalAuditsCount = item.ProposalAudits?.Count ?? 0,
                NotesCount = item.Notes?.Count ?? 0,
                // CALCULATED
                SitesCount = sitesCount,
                //EmployeesCount = employeesCount,
                TotalWorkers = totalWorkers,
                AuditCycles = auditCycles,
                // NOT MAPPED
                Alerts = item.Alerts,
            };
        } // ProposalToItemListDto

        public static async Task<ProposalItemDetailDto> ProposalToItemDetailDto(Proposal item)
        {  

            return new ProposalItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                Justification = item.Justification,
                SignerName = item.SignerName,
                SignerPosition = item.SignerPosition,                
                SendToSignDate = item.SignRequestDate,
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
                Organization = item.Organization != null
                    ? await OrganizationMapping.OrganizationToItemListDto(item.Organization)
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
                Sites = item.ADCs != null       // Ya no es necesario
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
                Contacts = item.ADCs != null    // Ya no es necesario
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
                        .Where(adc => adc.TotalWorkers.HasValue)
                        .Select(adc => adc.TotalWorkers.Value)
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
                OrganizationID = itemDto.OrganizationID ?? Guid.Empty,
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

        public static Proposal ItemUpdateWithListDtoToProposal(ProposalWithAuditListDto itemDto)
        {
            var item = ItemUpdateDtoToProposal(itemDto.Proposal);

            if (itemDto.ProposalAudits != null && itemDto.ProposalAudits.Count > 0)
            {
                item.ProposalAudits = new List<ProposalAudit>();
                foreach (var paDto in itemDto.ProposalAudits)
                {
                    var pa = ProposalAuditMapping.ItemUpdateDtoToProposalAudit(paDto);
                    item.ProposalAudits.Add(pa);
                }
            }

            return item;
        } // ItemUpdateWithListDtoToProposal

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