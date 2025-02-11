using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class FSSCSubCategoryMapping
    {
        public static IEnumerable<FSSCSubCategoryItemListDto> FSSCSubCategoryToListDto(IEnumerable<FSSCSubCategory> items)
        {
            var itemsDto = new List<FSSCSubCategoryItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(FSSCSubCategoryToItemListDto(item));
            }

            return itemsDto;
        } // FSSCSubCategoryToListDto

        public static FSSCSubCategoryItemListDto FSSCSubCategoryToItemListDto(FSSCSubCategory item)
        {
            return new FSSCSubCategoryItemListDto
            {
                ID = item.ID,
                FSSCCategoryID = item.FSSCCategoryID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                FSSCCategoryName = item.FSSCCategory != null
                    ? item.FSSCCategory.Name 
                    : string.Empty,
                ActivitiesCount = item.FSSCActivities != null
                    ? item.FSSCActivities.Count()
                    : 0
            };
        } // FSSCSubCategoryToItemListDto

        public static FSSCSubCategoryItemDetailDto FSSCSubCategoryToItemDetailDto(FSSCSubCategory item)
        {
            return new FSSCSubCategoryItemDetailDto
            {
                ID = item.ID,
                FSSCCategoryID = item.FSSCCategoryID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                FSSCCategory = item.FSSCCategory != null
                    ? FSSCCategoryMapping.FSSCCategoryToItemListDto(item.FSSCCategory) 
                    : null,
                FSSCActivities = item.FSSCActivities != null
                    ? FSSCActivityMapping.FSSCActivityToListDto(item.FSSCActivities)
                        .Where(s => s.Status != StatusType.Nothing)
                    : null
            };
        } // FSSCSubCategoryToItemDetailDto

        public static FSSCSubCategory ItemAddDtoToFSSCSubCategory(FSSCSubCategoryPostDto itemDto)
        {
            return new FSSCSubCategory
            {
                FSSCCategoryID = itemDto.FSSCCategoryID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToFSSCSubCategory

        public static FSSCSubCategory ItemEditDtoToFSSCSubCategory(FSSCSubCategoryPutDto itemDto)
        {
            return new FSSCSubCategory
            {
                ID = itemDto.ID,
                Name = itemDto.Name,
                Description = itemDto.Description,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToFSSCSubCategory

        public static FSSCSubCategory ItemDeleteDtoToFSSCSubCategory(FSSCSubCategoryDeleteDto itemDto)
        {
            return new FSSCSubCategory
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToFSSCSubCategory
    }
}