using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class NSSCCategoryMapping
    {
        public static IEnumerable<NSSCCategoryItemListDto> NSSCCategoryToListDto(IEnumerable<NSSCCategory> items) 
        {
            var itemsDto = new List<NSSCCategoryItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(NSSCCategoryToItemListDto(item));
            }

            return itemsDto;
        } // NSSCCategoryToListDto

        public static NSSCCategoryItemListDto NSSCCategoryToItemListDto(NSSCCategory item)
        {
            return new NSSCCategoryItemListDto
            { 
                ID = item.ID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                SubCategoriesCount = item.NSSCSubCategories != null 
                    ? item.NSSCSubCategories.Count() 
                    : 0
            };
        } // NSSCCategoryToItemListDto

        public static NSSCCategoryItemDetailDto NSSCCategoryToItemDetailDto(NSSCCategory item)
        {
            return new NSSCCategoryItemDetailDto
            {
                ID = item.ID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                NSSCSubCategories = item.NSSCSubCategories != null
                    ? NSSCSubCategoryMapping.NSSCSubCategoryToListDto(item.NSSCSubCategories)
                        .Where(s => s.Status != StatusType.Nothing)
                    : null
            };
        } // NSSCCategoryToItemDetailDto

        public static NSSCCategory ItemAddDtoToNSSCCategory(NSSCCategoryPostDto itemDto)
        {
            return new NSSCCategory
            {
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToNSSCCategory

        public static NSSCCategory ItemEditDtoToNSSCCategory(NSSCCategoryPutDto itemDto)
        {
            return new NSSCCategory
            {
                ID = itemDto.ID,
                Name = itemDto.Name,
                Description = itemDto.Description,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToNSSCCategory

        public static NSSCCategory ItemDeleteDtoToNSSCCategory(NSSCCategoryDeleteDto itemDto)
        {
            return new NSSCCategory
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToNSSCCategory
    }
}