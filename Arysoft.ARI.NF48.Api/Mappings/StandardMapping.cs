using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class StandardMapping
    {
        public static IEnumerable<StandardItemListDto> StandardsToListDto(IEnumerable<Standard> items)
        {
            var itemsDto = new List<StandardItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(StandardToItemListDto(item));
            }

            return itemsDto;
        } // StandardsToListDto

        public static StandardItemListDto StandardToItemListDto(Standard item)
        {
            return new StandardItemListDto
            { 
                ID = item.ID,
                Name = item.Name,
                Description = item.Description,
                MaxReductionDays = item.MaxReductionDays,
                SalesMaxReductionDays = item.SalesMaxReductionDays,
                Status = item.Status,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                ApplicationsCount = item.Applications != null 
                    ? item.Applications.Count 
                    : 0,
                AuditorsCount = item.AuditorStandards != null
                    ? item.AuditorStandards.Count
                    : 0,
                CatAuditorDocumentsCount = item.CatAuditorDocuments != null
                    ? item.CatAuditorDocuments.Count 
                    : 0
            };
        } // StandardToItemListDto

        public static StandardItemDetailDto StandardToItemDetailDto(Standard item)
        {
            return new StandardItemDetailDto
            {
                ID = item.ID,
                Name = item.Name,
                Description = item.Description,
                MaxReductionDays = item.MaxReductionDays,
                SalesMaxReductionDays = item.SalesMaxReductionDays,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Applications = item.Applications != null
                    ? ApplicationMapping.ApplicationsToListDto(item.Applications)
                    : null,
                Auditors = item.AuditorStandards != null
                    ? AuditorStandardMapping.AuditorStandardToListDto(item.AuditorStandards)
                    : null,
                CatAuditorDocuments = item.CatAuditorDocuments != null
                    ? CatAuditorDocumentMapping.CatAuditorDocumentToListDto(item.CatAuditorDocuments)
                    : null
            };
        } // StandardToItemDetailDto

        public static Standard ItemAddDtoToStandard(StandardPostDto itemDto)
        {
            return new Standard
            {
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToStandard

        public static Standard ItemEditDtoToStandard(StandardPutDto itemDto) 
        {
            var item = new Standard();

            item.ID = itemDto.ID;
            item.Name = itemDto.Name;
            item.Description = itemDto.Description;
            item.MaxReductionDays = itemDto.MaxReductionDays;
            item.SalesMaxReductionDays = itemDto.SalesMaxReductionDays;
            item.Status = itemDto.Status;
            item.UpdatedUser = itemDto.UpdatedUser;

            return item;
        } // ItemEditDtoToStandard

        public static Standard ItemDeleteDtoToStandard(StandardDeleteDto itemDto)
        {
            return new Standard
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToStandard
    }
}