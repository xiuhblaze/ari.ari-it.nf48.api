using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class CatAuditorDocumentMapping
    {
        public static IEnumerable<CatAuditorDocumentItemListDto> CatAuditorDocumentToListDto(IEnumerable<CatAuditorDocument> items)
        { 
            var itemsDto = new List<CatAuditorDocumentItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(CatAuditorDocumentToItemListDto(item));
            }

            return itemsDto;
        } // CatAuditorDocumentToListDto

        public static CatAuditorDocumentItemListDto CatAuditorDocumentToItemListDto(CatAuditorDocument item)
        {
            return new CatAuditorDocumentItemListDto
            {
                ID = item.ID,
                Name = item.Name,
                Description = item.Description,
                DocumentType = item.DocumentType,
                SubCategory = item.SubCategory,
                UpdateEvery = item.UpdateEvery,
                UpdatePeriodicity = item.UpdatePeriodicity,
                WarningEvery = item.WarningEvery,
                WarningPeriodicity = item.WarningPeriodicity,
                IsRequired = item.IsRequired,
                Order = item.Order,
                Status = item.Status,
                DocumentsCount = item.Documents != null 
                    ? item.Documents.Count 
                    : 0
            };
        } // CatAuditorDocumentToItemListDto

        public static CatAuditorDocumentItemDetailDto CatAuditorDocumentToItemDetailDto(CatAuditorDocument item)
        {
            return new CatAuditorDocumentItemDetailDto
            {
                ID = item.ID,
                Name = item.Name,
                Description = item.Description,
                DocumentType = item.DocumentType,
                SubCategory = item.SubCategory,
                UpdateEvery = item.UpdateEvery,
                UpdatePeriodicity = item.UpdatePeriodicity,
                WarningEvery = item.WarningEvery,
                WarningPeriodicity = item.WarningPeriodicity,
                IsRequired = item.IsRequired,
                Order = item.Order,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Documents = item.Documents != null
                    ? AuditorDocumentMapping.AuditorDocumentToListDto(item.Documents)
                    : null,
            };
        } // CatAuditorDocumentToItemDetailDto

        public static CatAuditorDocument ItemAddDtoToCatAuditorDocument(CatAuditorDocumentPostDto itemDto)
        {
            return new CatAuditorDocument
            {
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToCatAuditorDocument

        public static CatAuditorDocument ItemEditDtoToCatAuditorDocument(CatAuditorDocumentPutDto itemDto)
        {
            return new CatAuditorDocument
            {
                ID = itemDto.ID,
                Name = itemDto.Name,
                Description = itemDto.Description,
                DocumentType = itemDto.DocumentType,
                SubCategory = itemDto.SubCategory,
                UpdateEvery = itemDto.UpdateEvery,
                UpdatePeriodicity = itemDto.UpdatePeriodicity,
                WarningEvery = itemDto.WarningEvery,
                WarningPeriodicity = itemDto.WarningPeriodicity,
                IsRequired = itemDto.IsRequired,
                Order = itemDto.Order,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToCatAuditorDocument

        public static CatAuditorDocument ItemDeleteDtoToCatAuditorDocument(CatAuditorDocumentDeleteDto itemDto)
        {
            return new CatAuditorDocument
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToCatAuditorDocument
    }
}