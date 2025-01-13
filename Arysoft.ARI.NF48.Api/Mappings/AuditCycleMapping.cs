using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class AuditCycleMapping
    {
        public static IEnumerable<AuditCycleItemListDto> AuditCyclesToListDto(IEnumerable<AuditCycle> items)
        {
            var itemsDto = new List<AuditCycleItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(AuditCycleToItemListDto(item));
            }

            return itemsDto;
        } // AuditCycleToListDto

        public static AuditCycleItemListDto AuditCycleToItemListDto(AuditCycle item)
        {
            return new AuditCycleItemListDto
            {
                ID = item.ID,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                Status = item.Status,
                OrganizationName = item.Organization != null
                    ? item.Organization.Name
                    : string.Empty,
                AuditsCount = item.Audits != null
                    ? item.Audits.Count
                    : 0,
                //AuditCycleStandards = item.AuditCycleStandards != null
                //    ? AuditCycleStandardMapping.AuditCycleStandardToListDto(item.AuditCycleStandards)
                //    : null,
                DocumentsCount = item.AuditCycleDocuments != null
                    ? item.AuditCycleDocuments.Count
                    : 0
            };
        } // AuditCycleToItemListDto

        public static AuditCycleItemDetailDto AuditCycleToItemDetailDto(AuditCycle item)
        {
            return new AuditCycleItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Organization = item.Organization != null
                    ? OrganizationMapping.OrganizationToItemListDto(item.Organization)
                    : null,
                //Audits = item.Audits != null
                //    ? AuditMapping.AuditToListDto(item.Audits)
                //    : null,
                AuditCycleStandards = item.AuditCycleStandards != null
                    ? AuditCycleStandardMapping.AuditCycleStandardsToListDto(
                        item.AuditCycleStandards.Where(acs => acs.Status != StatusType.Nothing))
                    : null,
                //AuditCycleDocuments = item.AuditCycleDocuments != null
                //    ? AuditCycleDocumentMapping.AuditCycleDocumentToListDto(item.AuditCycleDocuments)
                //    : null,
            };
        } // AuditCycleToItemDetailDto

        public static AuditCycle ItemAddDtoToAuditCycle(AuditCyclePostDto itemDto)
        {
            return new AuditCycle
            {
                OrganizationID = itemDto.OrganizationID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToAuditCycle

        public static AuditCycle ItemEditDtoToAuditCycle(AuditCyclePutDto itemDto)
        {
            return new AuditCycle
            {
                ID = itemDto.ID,                
                StartDate = itemDto.StartDate,
                EndDate = itemDto.EndDate,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToAuditCycle

        public static AuditCycle ItemDeleteDtoToAuditCycle(AuditCycleDeleteDto itemDto)
        {
            return new AuditCycle
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToAuditCycle
    }
}