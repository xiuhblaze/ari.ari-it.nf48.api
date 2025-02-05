using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class AuditCycleStandardMapping
    {
        public static IEnumerable<AuditCycleStandardItemListDto> AuditCycleStandardsToListDto(IEnumerable<AuditCycleStandard> items)
        {
            var itemsDto = new List<AuditCycleStandardItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(AuditCycleStandardToItemListDto(item));
            }

            return itemsDto;
        } // AuditCycleStandardsToListDto

        public static AuditCycleStandardItemListDto AuditCycleStandardToItemListDto(AuditCycleStandard item)
        {
            return new AuditCycleStandardItemListDto
            {
                ID = item.ID,
                InitialStep = item.InitialStep,
                CycleType = item.CycleType,
                Status = item.Status,
                StandardName = item.Standard != null
                    ? item.Standard.Name
                    : string.Empty
            };
        } // AuditCycleStandardToItemListDto

        public static AuditCycleStandardItemDetailDto AuditCycleStandardToItemDetailDto(AuditCycleStandard item)
        {
            return new AuditCycleStandardItemDetailDto
            {
                ID = item.ID,
                AuditCycleID = item.AuditCycleID,
                StandardID = item.StandardID ?? Guid.Empty,
                InitialStep = item.InitialStep,
                CycleType = item.CycleType,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Standard = item.Standard != null
                    ? StandardMapping.StandardToItemListDto(item.Standard)
                    : null,
                AuditCycle = item.AuditCycle != null
                    ? AuditCycleMapping.AuditCycleToItemListDto(item.AuditCycle)
                    : null
            };
        } // AuditCycleStandardToItemDetailDto

        public static AuditCycleStandard ItemAddDtoToAuditCycleStandard(AuditCycleStandardPostDto itemDto)
        {
            return new AuditCycleStandard
            {
                AuditCycleID = itemDto.AuditCycleID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToAuditCycleStandard

        public static AuditCycleStandard ItemEditDtoToAuditCycleStandard(AuditCycleStandardPutDto itemDto)
        {
            return new AuditCycleStandard
            {
                ID = itemDto.ID,
                StandardID = itemDto.StandardID,
                InitialStep = itemDto.InitialStep,
                CycleType = itemDto.CycleType,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToAuditCycleStandard

        public static AuditCycleStandard ItemDeleteDtoToAuditCycleStandard(AuditCycleStandardDeleteDto itemDto)
        {
            return new AuditCycleStandard
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToAuditCycleStandard
    }
}