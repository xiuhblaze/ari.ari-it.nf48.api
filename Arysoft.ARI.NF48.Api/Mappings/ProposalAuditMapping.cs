using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class ProposalAuditMapping
    {
        public static IEnumerable<ProposalAuditItemDto> ProposalAuditToListDto(IEnumerable<ProposalAudit> items)
        {
            var itemsDto = new List<ProposalAuditItemDto>();

            foreach (var item in items)
            {
                itemsDto.Add(ProposalAuditToItemDto(item));
            }

            return itemsDto;
        } // ProposalAuditToListDto

        public static ProposalAuditItemDto ProposalAuditToItemDto(ProposalAudit item)
        { 
            return new ProposalAuditItemDto
            {
                ID = item.ID,
                ProposalID = item.ProposalID,
                AuditStep = item.AuditStep,
                TotalAuditDays = item.TotalAuditDays,
                SubTotal = item.SubTotal,
                CertificateIssue = item.CertificateIssue,
                TotalCost = item.TotalCost,
                TravelExpenses = item.TravelExpenses,
                TotalFinal = item.TotalFinal,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser
            };
        } // ProposalAuditToItemDto

        public static ProposalAudit ItemCreateDtoToProposalAudit(ProposalAuditCreateDto itemDto)
        {
            return new ProposalAudit
            {
                ProposalID = itemDto.ProposalID ?? Guid.Empty,
                AuditStep = itemDto.AuditStep,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemCreateDtoToProposalAudit

        public static ProposalAudit ItemUpdateDtoToProposalAudit(ProposalAuditUpdateDto itemDto)
        {
            return new ProposalAudit
            {
                ID = itemDto.ID ?? Guid.Empty,
                TotalAuditDays = itemDto.TotalAuditDays,
                SubTotal = itemDto.SubTotal,
                CertificateIssue = itemDto.CertificateIssue,
                TotalCost = itemDto.TotalCost,
                TravelExpenses = itemDto.TravelExpenses,
                TotalFinal = itemDto.TotalFinal,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemUpdateDtoToProposalAudit

        public static IEnumerable<ProposalAudit> ItemsUpdateDtoToProposalAuditList(IEnumerable<ProposalAuditUpdateDto> itemsDto)
        {
            var items = new List<ProposalAudit>();

            foreach (var itemDto in itemsDto)
            {
                items.Add(ItemUpdateDtoToProposalAudit(itemDto));
            }

            return items;
        } // ItemUpdateDtoToProposalAuditList

        public static ProposalAudit ItemDeleteDtoToProposalAudit(ProposalAuditDeleteDto itemDto)
        {
            return new ProposalAudit
            {
                ID = itemDto.ID ?? Guid.Empty,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToProposalAudit
    }
}