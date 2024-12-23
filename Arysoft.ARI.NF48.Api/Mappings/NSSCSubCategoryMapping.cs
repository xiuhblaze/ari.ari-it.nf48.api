using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class NSSCSubCategoryMapping
    {
        public static IEnumerable<NSSCSubCategoryItemListDto> NSSCSubCategoryToListDto(IEnumerable<NSSCSubCategory> items)
        {
            var itemsDto = new List<NSSCSubCategoryItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(NSSCSubCategoryToItemListDto(item));
            }

            return itemsDto;
        } // NSSCSubCategoryToListDto

        public static NSSCSubCategoryItemListDto NSSCSubCategoryToItemListDto(NSSCSubCategory item)
        {
            return new NSSCSubCategoryItemListDto
            {
                ID = item.ID,
                NSSCCategoryID = item.NSSCCategoryID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                NSSCCategoryName = item.NSSCCategory != null
                    ? item.NSSCCategory.Name 
                    : string.Empty,
                ActivitiesCount = item.NSSCActivities != null
                    ? item.NSSCActivities.Count()
                    : 0
            };
        } // NSSCSubCategoryToItemListDto

        public static NSSCSubCategoryItemDetailDto NSSCSubCategoryToItemDetailDto(NSSCSubCategory item)
        {
            return new NSSCSubCategoryItemDetailDto
            {
                ID = item.ID,
                NSSCCategoryID = item.NSSCCategoryID,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                NSSCCategory = item.NSSCCategory != null
                    ? NSSCCategoryMapping.NSSCCategoryToItemListDto(item.NSSCCategory) 
                    : null,
                //NSSCActivities = item.NSSCActivities != null
                //    ? NSSCActivityMapping.NSSCActivityToListDto(item.NSSCActivities)
                //        .Where(s => s.Status != StatusType.Nothing)
                //    : null
            };
        } // NSSCSubCategoryToItemDetailDto

        public static NSSCSubCategory ItemAddDtoToNSSCSubCategory(NSSCSubCategoryPostDto itemDto)
        {
            return new NSSCSubCategory
            {
                NSSCCategoryID = itemDto.NSSCCategoryID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToNSSCSubCategory

        public static NSSCSubCategory ItemEditDtoToNSSCSubCategory(NSSCSubCategoryPutDto itemDto)
        {
            return new NSSCSubCategory
            {
                ID = itemDto.ID,
                Name = itemDto.Name,
                Description = itemDto.Description,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToNSSCSubCategory

        public static NSSCSubCategory ItemDeleteDtoToNSSCSubCategory(NSSCSubCategoryDeleteDto itemDto)
        {
            return new NSSCSubCategory
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToNSSCSubCategory
    }
}