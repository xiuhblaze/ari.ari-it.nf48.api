using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class AuditCycleDocumentMapping
    {
        public static IEnumerable<AuditCycleDocumentItemListDto> AuditCycleDocumentsToListDto(IEnumerable<AuditCycleDocument> items)
        {
            var itemsDto = new List<AuditCycleDocumentItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(AuditCycleDocumentToItemListDto(item));
            }

            return itemsDto;
        } // AuditCycleDocumentsToListDto

        public static AuditCycleDocumentItemListDto AuditCycleDocumentToItemListDto(AuditCycleDocument item)
        {
            return new AuditCycleDocumentItemListDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                Filename = item.Filename,
                Version = item.Version,
                Comments = item.Comments,
                DocumentType = item.DocumentType ?? AuditCycleDocumentType.Nothing,
                OtherDescription = item.OtherDescription,
                UploadedBy = item.UploadedBy,
                Status = item.Status,

                OrganizationName = item.OrganizationID != null
                    ? item.Organization.Name
                    : string.Empty,
                AuditCycles = item.AuditCycles != null
                    ? AuditCycleMapping.AuditCyclesToListDto(item.AuditCycles
                        .Where(ac =>
                            ac.Status != StatusType.Nothing
                            && ac.Status != StatusType.Deleted)
                        .ToList())
                    : null
            };
        } // AuditCycleDocumentToItemListDto

        public static AuditCycleDocumentItemDetailDto AuditCycleDocumentToItemDetailDto(AuditCycleDocument item)
        {
            return new AuditCycleDocumentItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                //AuditCycleID = item.AuditCycleID,
                //StandardID = item.StandardID,
                Filename = item.Filename,
                Version = item.Version,
                Comments = item.Comments,
                DocumentType = item.DocumentType ?? AuditCycleDocumentType.Nothing,
                OtherDescription = item.OtherDescription,
                UploadedBy = item.UploadedBy,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,

                OrganizationName = item.Organization != null
                    ? item.Organization.Name
                    : string.Empty,
                AuditCycles = item.AuditCycles != null
                    ? AuditCycleMapping.AuditCyclesToListDto(item.AuditCycles
                        .Where(ac =>
                            ac.Status != StatusType.Nothing
                            && ac.Status != StatusType.Deleted)
                        .ToList())
                    : null
                //Standard = item.Standard != null
                //    ? StandardMapping.StandardToItemListDto(item.Standard)
                //    : null
            };
        } // AuditCycleDocumentToItemDetailDto

        public static AuditCycleDocument ItemAddDtoToAuditCycleDocument(AuditCycleDocumentPostDto itemDto)
        {
            return new AuditCycleDocument
            {
                //AuditCycleID = itemDto.AuditCycleID,
                OrganizationID = itemDto.OrganizationID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToAuditCycleDocument

        public static AuditCycleDocument ItemEditDtoToAuditCycleDocument(AuditCycleDocumentPutDto itemDto)
        {
            return new AuditCycleDocument
            {
                ID = itemDto.ID,
                //StandardID = itemDto.StandardID,
                Version = itemDto.Version,
                Comments = itemDto.Comments,
                DocumentType = itemDto.DocumentType,
                OtherDescription = itemDto.OtherDescription,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToAuditCycleDocument

        public static AuditCycleDocument ItemDeleteDtoToAuditCycleDocument(AuditCycleDocumentDeleteDto itemDto)
        {
            return new AuditCycleDocument
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToAuditCycleDocument
    }
}