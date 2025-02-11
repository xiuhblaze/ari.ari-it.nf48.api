using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class FSSCActivityMapping
    {
        public static IEnumerable<FSSCActivityItemListDto> FSSCActivityToListDto(IEnumerable<FSSCActivity> items)
        {
            var itemsDto = new List<FSSCActivityItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(FSSCActivityToItemListDto(item));
            }

            return itemsDto;
        } // FSSCActivityToListDto

        public static FSSCActivityItemListDto FSSCActivityToItemListDto(FSSCActivity item)
        {
            return new FSSCActivityItemListDto
            {
                ID = item.ID,
                FSSCSubCategoryID = item.FSSCSubCategoryID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                FSSCSubCategoryName = item.FSSCSubCategory != null
                    ? item.FSSCSubCategory.Name
                    : string.Empty
            };
        } // FSSCActivityToItemListDto

        public static FSSCActivityItemDetailDto FSSCActivityToItemDetailDto(FSSCActivity item)
        {
            return new FSSCActivityItemDetailDto
            {
                ID = item.ID,
                FSSCSubCategoryID = item.FSSCSubCategoryID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                FSSCSubCategory = item.FSSCSubCategory != null
                    ? FSSCSubCategoryMapping.FSSCSubCategoryToItemListDto(item.FSSCSubCategory)
                    : null
            };
        } // FSSCActivityToItemDetailDto

        public static FSSCActivity ItemAddDtoToFSSCActivity(FSSCActivityPostDto itemDto)
        {
            return new FSSCActivity
            {
                FSSCSubCategoryID = itemDto.FSSCSubCategoryID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToFSSCActivity

        public static FSSCActivity ItemEditDtoToFSSCActivity(FSSCActivityPutDto itemDto)
        {
            return new FSSCActivity
            {
                ID = itemDto.ID,
                Name = itemDto.Name,
                Description = itemDto.Description,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToFSSCActivity

        public static FSSCActivity ItemDeleteDtoToFSSCActivity(FSSCActivityDeleteDto itemDto)
        {
            return new FSSCActivity
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToFSSCActivity
    }
}