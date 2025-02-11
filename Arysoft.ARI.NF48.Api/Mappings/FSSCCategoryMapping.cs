using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class FSSCCategoryMapping
    {
        public static IEnumerable<FSSCCategoryItemListDto> FSSCCategoryToListDto(IEnumerable<FSSCCategory> items) 
        {
            var itemsDto = new List<FSSCCategoryItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(FSSCCategoryToItemListDto(item));
            }

            return itemsDto;
        } // FSSCCategoryToListDto

        public static FSSCCategoryItemListDto FSSCCategoryToItemListDto(FSSCCategory item)
        {
            return new FSSCCategoryItemListDto
            { 
                ID = item.ID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                SubCategoriesCount = item.FSSCSubCategories != null 
                    ? item.FSSCSubCategories.Count() 
                    : 0
            };
        } // FSSCCategoryToItemListDto

        public static FSSCCategoryItemDetailDto FSSCCategoryToItemDetailDto(FSSCCategory item)
        {
            return new FSSCCategoryItemDetailDto
            {
                ID = item.ID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                FSSCSubCategories = item.FSSCSubCategories != null
                    ? FSSCSubCategoryMapping.FSSCSubCategoryToListDto(item.FSSCSubCategories)
                        .Where(s => s.Status != StatusType.Nothing)
                    : null
            };
        } // FSSCCategoryToItemDetailDto

        public static FSSCCategory ItemAddDtoToFSSCCategory(FSSCCategoryPostDto itemDto)
        {
            return new FSSCCategory
            {
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToFSSCCategory

        public static FSSCCategory ItemEditDtoToFSSCCategory(FSSCCategoryPutDto itemDto)
        {
            return new FSSCCategory
            {
                ID = itemDto.ID,
                Name = itemDto.Name,
                Description = itemDto.Description,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToFSSCCategory

        public static FSSCCategory ItemDeleteDtoToFSSCCategory(FSSCCategoryDeleteDto itemDto)
        {
            return new FSSCCategory
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToFSSCCategory
    }
}