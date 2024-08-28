using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class Category22KMapping
    {
        public static IEnumerable<Category22KItemListDto> Category22KToListDto(IEnumerable<Category22K> items)
        {
            var itemsDto = new List<Category22KItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(Category22KToItemListDto(item));
            }

            return itemsDto;
        } // Category22KToListDto

        public static Category22KItemListDto Category22KToItemListDto(Category22K item)
        {
            return new Category22KItemListDto
            {
                ID = item.ID,
                Cluster = item.Cluster,
                Category = item.Category,
                CategoryDescription = item.CategoryDescription,
                SubCategory = item.SubCategory,
                SubCategoryDescription = item.SubCategoryDescription,
                Examples = item.Examples,
                IsAccredited = item.IsAccredited,
                Status = item.Status,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser
            };
        } // Category22KToItemListDto

        public static Category22KItemDetailDto Category22KToItemDetailDto(Category22K item)
        {
            return new Category22KItemDetailDto
            {
                ID = item.ID,
                Cluster = item.Cluster,
                Category = item.Category,
                CategoryDescription = item.CategoryDescription,
                SubCategory = item.SubCategory,
                SubCategoryDescription = item.SubCategoryDescription,
                Examples = item.Examples,
                IsAccredited = item.IsAccredited,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser
            };
        } // Category22KToItemDetailDto

        public static Category22K ItemAddDtoToCategory22K(Category22KPostDto itemDto)
        {
            return new Category22K
            {
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToCategory22K

        public static Category22K ItemEditDtoToCategory22K(Category22KPutDto itemDto)
        {
            return new Category22K
            {
                ID = itemDto.ID,
                Cluster = itemDto.Cluster,
                Category = itemDto.Category,
                CategoryDescription = itemDto.CategoryDescription,
                SubCategory = itemDto.SubCategory,
                SubCategoryDescription = itemDto.SubCategoryDescription,
                Examples = itemDto.Examples,
                IsAccredited = itemDto.IsAccredited,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToCategory22K

        public static Category22K ItemDeleteDtoToCategory22K(Category22KDeleteDto itemDto)
        {
            return new Category22K
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToCategory22K

        public static string Category22KToSummary(Category22K item)
        {
            var summary = item.Category;

            summary += string.IsNullOrEmpty(item.CategoryDescription)
                ? string.Empty 
                : " " + item.CategoryDescription;

            return summary;
        } // Category22KToSummary
    }
}