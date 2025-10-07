using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class StandardTemplateMapping
    {
        public static IEnumerable<StandardTemplateItemListDto> StandardTemplatesToListDto(IEnumerable<StandardTemplate> items)
        {
            var itemsDto = new List<StandardTemplateItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(StandardTemplateToItemListDto(item));
            }

            return itemsDto;
        } // StandardTemplatesToListDto

        public static StandardTemplateItemListDto StandardTemplateToItemListDto(StandardTemplate item)
        {
            return new StandardTemplateItemListDto
            {
                ID = item.ID,
                StandardID = item.StandardID,
                Description = item.Description,
                Filename = item.Filename,
                Version = item.Version,
                Status = item.Status,
                StandardName = item.Standard != null ? item.Standard.Name : string.Empty
            };
        } // StandardTemplateToItemListDto

        public static StandardTemplateItemDetailDto StandardTemplateToItemDetailDto(StandardTemplate item)
        {
            return new StandardTemplateItemDetailDto
            {
                ID = item.ID,
                StandardID = item.StandardID,
                Description = item.Description,
                Filename = item.Filename,
                Version = item.Version,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Standard = item.Standard != null 
                    ? StandardMapping.StandardToItemListDto(item.Standard) 
                    : null
            };
        } // StandardTemplateToItemDetailDto

        public static StandardTemplate ItemCreateDtoToStandardTemplate(StandardTemplateCreateDto itemCreateDto)
        {
            var item = new StandardTemplate
            {
                StandardID = itemCreateDto.StandardID ?? Guid.Empty,
                UpdatedUser = itemCreateDto.UpdatedUser
            };

            return item;
        } // ItemCreateDtoToStandardTemplate

        public static StandardTemplate ItemUpdateDtoToStandardTemplate(StandardTemplateUpdateDto itemUpdateDto)
        {
            var item = new StandardTemplate
            {
                ID = itemUpdateDto.ID,
                Description = itemUpdateDto.Description,
                //Filename = itemUpdateDto.Filename,
                Version = itemUpdateDto.Version,
                Status = itemUpdateDto.Status.Value,
                UpdatedUser = itemUpdateDto.UpdatedUser
            };

            return item;
        } // ItemUpdateDtoToStandardTemplate

        public static StandardTemplate ItemDeleteDtoToStandardTemplate(StandardTemplateDeleteDto itemDeleteDto)
        {
            var item = new StandardTemplate
            {
                ID = itemDeleteDto.ID,
                UpdatedUser = itemDeleteDto.UpdatedUser
            };

            return item;
        } // ItemDeleteDtoToStandardTemplate
    }
}