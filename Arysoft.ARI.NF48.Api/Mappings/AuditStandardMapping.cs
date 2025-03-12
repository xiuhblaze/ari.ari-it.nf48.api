using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class AuditStandardMapping
    {
        public static IEnumerable<AuditStandardItemListDto> AuditStandardToListDto(IEnumerable<AuditStandard> items)
        {
            var itemsDto = new List<AuditStandardItemListDto>();
            foreach (var item in items)
            {
                itemsDto.Add(AuditStandardToItemListDto(item));
            }
            return itemsDto;
        } // AuditStandarToListDto

        public static AuditStandardItemListDto AuditStandardToItemListDto(AuditStandard item)
        {
            return new AuditStandardItemListDto
            {
                ID = item.ID,
                AuditID = item.AuditID,
                StandardID = item.StandardID,
                Step = item.Step,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                AuditDescription = item.Audit != null
                    ? item.Audit.Description
                    : string.Empty,
                StandardName = item.Standard != null
                    ? item.Standard.Name
                    : string.Empty,
                AuditorsCount = item.AuditAuditors != null
                    ? item.AuditAuditors.Count
                    : 0,
                DocumentsCount = item.AuditDocuments != null
                    ? item.AuditDocuments.Count
                    : 0
            };
        } // AuditStandardToItemListDto

        public static AuditStandardItemDetailDto AuditStandardToItemDetailDto(AuditStandard item)
        {
            return new AuditStandardItemDetailDto
            {
                ID = item.ID,
                AuditID = item.AuditID,
                StandardID = item.StandardID,
                Step = item.Step,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Audit = item.Audit != null
                    ? AuditMapping.AuditToItemListDto(item.Audit)
                    : null,
                Standard = item.Standard != null
                    ? StandardMapping.StandardToItemListDto(item.Standard)
                    : null,
                Auditors = item.AuditAuditors != null
                    ? AuditAuditorMapping.AuditAuditorToListDto(item.AuditAuditors
                        .Where(aa => aa.Status != StatusType.Nothing))
                    : null,
                Documents = item.AuditDocuments != null
                    ? AuditDocumentMapping.AuditDocumentToListDto(item.AuditDocuments
                        .Where(ad => ad.Status != StatusType.Nothing))
                    : null
            };
        } // AuditStandardToItemDetailDto

        public static AuditStandard ItemAddDtoToAuditStandard(AuditStandardPostDto itemDto)
        {
            return new AuditStandard
            {
                AuditID = itemDto.AuditID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToAuditStandard

        public static AuditStandard ItemEditDtoToAuditStandard(AuditStandardPutDto itemDto)
        {
            return new AuditStandard
            {
                ID = itemDto.ID,
                StandardID = itemDto.StandardID,
                Step = itemDto.Step,
                ExtraInfo = itemDto.ExtraInfo,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToAuditStandard

        public static AuditStandard ItemDeleteDtoToAuditStandard(AuditStandardDeleteDto itemDto)
        {
            return new AuditStandard
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToAuditStandard
    }
}