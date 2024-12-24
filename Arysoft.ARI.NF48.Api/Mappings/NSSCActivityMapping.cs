using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class NSSCActivityMapping
    {
        public static IEnumerable<NSSCActivityItemListDto> NSSCActivityToListDto(IEnumerable<NSSCActivity> items)
        {
            var itemsDto = new List<NSSCActivityItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(NSSCActivityToItemListDto(item));
            }

            return itemsDto;
        } // NSSCActivityToListDto

        public static NSSCActivityItemListDto NSSCActivityToItemListDto(NSSCActivity item)
        {
            return new NSSCActivityItemListDto
            {
                ID = item.ID,
                NSSCSubCategoryID = item.NSSCSubCategoryID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                NSSCSubCategoryName = item.NSSCSubCategory != null
                    ? item.NSSCSubCategory.Name
                    : string.Empty
            };
        } // NSSCActivityToItemListDto

        public static NSSCActivityItemDetailDto NSSCActivityToItemDetailDto(NSSCActivity item)
        {
            return new NSSCActivityItemDetailDto
            {
                ID = item.ID,
                NSSCSubCategoryID = item.NSSCSubCategoryID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                NSSCSubCategory = item.NSSCSubCategory != null
                    ? NSSCSubCategoryMapping.NSSCSubCategoryToItemListDto(item.NSSCSubCategory)
                    : null
            };
        } // NSSCActivityToItemDetailDto

        public static NSSCActivity ItemAddDtoToNSSCActivity(NSSCActivityPostDto itemDto)
        {
            return new NSSCActivity
            {
                NSSCSubCategoryID = itemDto.NSSCSubCategoryID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToNSSCActivity

        public static NSSCActivity ItemEditDtoToNSSCActivity(NSSCActivityPutDto itemDto)
        {
            return new NSSCActivity
            {
                ID = itemDto.ID,
                Name = itemDto.Name,
                Description = itemDto.Description,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToNSSCActivity

        public static NSSCActivity ItemDeleteDtoToNSSCActivity(NSSCActivityDeleteDto itemDto)
        {
            return new NSSCActivity
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToNSSCActivity
    }
}