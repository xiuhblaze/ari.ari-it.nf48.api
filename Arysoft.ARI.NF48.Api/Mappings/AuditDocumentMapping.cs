using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class AuditDocumentMapping
    {
        public static IEnumerable<AuditDocumentItemListDto> AuditDocumentToListDto(IEnumerable<AuditDocument> items)
        { 
            var itemsDto = new List<AuditDocumentItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(AuditDocumentToItemListDto(item));
            }

            return itemsDto;
        } // AuditDocumentToListDto

        public static AuditDocumentItemListDto AuditDocumentToItemListDto(AuditDocument item)
        { 
            return new AuditDocumentItemListDto
            {
                ID = item.ID,
                AuditID = item.AuditID,
                //StandardID = item.StandardID,
                Filename = item.Filename,
                Comments = item.Comments,
                DocumentType = item.DocumentType,
                OtherDescription = item.OtherDescription,
                UploadedBy = item.UploadedBy,
                IsWitnessIncluded = item.IsWitnessIncluded,
                Status = item.Status,
                AuditDescription = item.Audit != null
                    ? item.Audit.Description
                    : string.Empty,
                //StandardName = item.Standard != null
                //    ? item.Standard.Name
                //    : string.Empty
                StandardsNames = item.AuditStandards?
                    .Where(ads => ads.Status == StatusType.Active)
                    .Select(ads => ads.Standard.Name)
            };
        } // AuditDocumentToItemListDto

        public static AuditDocumentItemDetailDto AuditDocumentToItemDetailDto(AuditDocument item)
        {
            return new AuditDocumentItemDetailDto
            {
                ID = item.ID,
                AuditID = item.AuditID,
                //StandardID = item.StandardID,
                Filename = item.Filename,
                Comments = item.Comments,
                DocumentType = item.DocumentType,
                OtherDescription = item.OtherDescription,
                IsWitnessIncluded = item.IsWitnessIncluded,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Audit = item.Audit != null
                    ? AuditMapping.AuditToItemListDto(item.Audit)
                    : null,
                AuditStandards = item.AuditStandards?
                    .Where(ads => ads.Status != StatusType.Nothing)
                    .Select(ads => AuditStandardMapping.AuditStandardToItemListDto(ads))
                //Standard = item.Standard != null
                //    ? StandardMapping.StandardToItemListDto(item.Standard)
                //    : null
            };
        } // AuditDocumentToItemDetailDto

        public static AuditDocument ItemAddDtoToAuditDocument(AuditDocumentPostDto itemDto)
        {
            return new AuditDocument
            {
                AuditID = itemDto.AuditID,                
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToAuditDocument

        public static AuditDocument ItemEditDtoToAuditDocument(AuditDocumentPutDto itemDto)
        {
            return new AuditDocument
            {
                ID = itemDto.ID,
                // StandardID = itemDto.StandardID,
                Comments = itemDto.Comments,
                DocumentType = itemDto.DocumentType,
                OtherDescription = itemDto.OtherDescription,
                IsWitnessIncluded = itemDto.IsWitnessIncluded,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToAuditDocument

        public static AuditDocument ItemDeleteDtoToAuditDocument(AuditDocumentDeleteDto itemDto)
        {
            return new AuditDocument
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToAuditDocument
    }
}