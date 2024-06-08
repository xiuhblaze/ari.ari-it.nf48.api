using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class NaceCodeMapping
    {
        public static IEnumerable<NaceCodeItemListDto> NaceCodeToListDto(IEnumerable<NaceCode> items)
        {
            var itemsDto = new List<NaceCodeItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(NaceCodeToItemListDto(item));
            }

            return itemsDto;
        } // NaceCodeToListDto

        public static NaceCodeItemListDto NaceCodeToItemListDto(NaceCode item)
        {
            return new NaceCodeItemListDto
            {
                ID = item.ID,
                Sector = item.Sector,
                Division = item.Division,
                Group = item.Group,
                Class = item.Class,
                Description = item.Description,
                Status = item.Status
            };
        } // NaceCodeToItemListDto

        public static NaceCodeItemDetailDto NaceCodeToItemDetailDto(NaceCode item)
        {
            return new NaceCodeItemDetailDto
            {
                ID = item.ID,
                Sector = item.Sector,
                Division = item.Division,
                Group = item.Group,
                Class = item.Class,
                Description = item.Description,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser
            };
        } // NaceCodeToItemDetailDto

        public static NaceCode ItemAddDtoToNaceCode(NaceCodePostDto itemDto)
        {
            return new NaceCode
            {
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToNaceCode

        public static NaceCode ItemEditDtoToNaceCode(NaceCodePutDto itemDto)
        {
            return new NaceCode
            {
                ID = itemDto.ID,
                Sector = itemDto.Sector,
                Division = itemDto.Division,
                Group = itemDto.Group,
                Class = itemDto.Class,
                Description = itemDto.Description,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToNaceCode

        public static NaceCode ItemDeleteDtoToNaceCode(NaceCodeDeleteDto itemDto)
        {
            return new NaceCode
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToNaceCode
    }
}