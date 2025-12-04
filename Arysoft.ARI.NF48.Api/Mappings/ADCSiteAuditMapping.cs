using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class ADCSiteAuditMapping
    {
        public static IEnumerable<ADCSiteAuditItemDto> ADCSiteAuditToListDto(IEnumerable<ADCSiteAudit> items)
        {
            var itemsDto = new List<ADCSiteAuditItemDto>();

            foreach (var item in items)
            {
                itemsDto.Add(ADCSiteAuditToItemDto(item));
            }

            return itemsDto;
        } // ADCSiteAuditToListDto

        public static ADCSiteAuditItemDto ADCSiteAuditToItemDto(ADCSiteAudit item)
        {
            return new ADCSiteAuditItemDto
            {
                ID = item.ID,
                ADCSiteID = item.ADCSiteID,
                Value = item.Value,
                AuditStep = item.AuditStep,
                PreAuditDays = item.PreAuditDays,
                Stage1Days = item.Stage1Days,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser
            };
        } // ADCSiteAuditToItemDto

        public static ADCSiteAudit ItemCreateDtoToADCSiteAudit(ADCSiteAuditCreateDto itemDto)
        {
            return new ADCSiteAudit
            {
                ADCSiteID = itemDto.ADCSiteID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemCreateDtoToADCSiteAudit

        public static ADCSiteAudit ItemUpdateDtoToADCSiteAudit(ADCSiteAuditUpdateDto itemDto)
        {
            return new ADCSiteAudit
            {
                ID = itemDto.ID,
                Value = itemDto.Value,
                AuditStep = itemDto.AuditStep,
                PreAuditDays = itemDto.PreAuditDays,
                Stage1Days = itemDto.Stage1Days,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemUpdateDtoToADCSiteAudit

        public static IEnumerable<ADCSiteAudit> UpdateListDtoToADCSiteAudit(ADCSiteAuditListUpdateDto itemsDto)
        {
            var items = new List<ADCSiteAudit>();

            foreach (var itemDto in itemsDto.Items)
            {
                items.Add(ItemUpdateDtoToADCSiteAudit(itemDto));
            }

            return items;
        } // UpdateListDtoToADCSiteAudit

        public static ADCSiteAudit ItemDeleteDtoToADCSiteAudit(ADCSiteAuditDeleteDto itemDto)
        {
            return new ADCSiteAudit
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToADCSiteAudit
    }
}