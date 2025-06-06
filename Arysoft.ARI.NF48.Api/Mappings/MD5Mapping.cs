using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class MD5Mapping
    {
        public static IEnumerable<MD5ItemListDto> MD5ToListDto(IEnumerable<MD5> items)
        {
            var itemsDto = new List<MD5ItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(MD5ToItemListDto(item));
            }

            return itemsDto;
        } // MD5ToListDto

        public static MD5ItemListDto MD5ToItemListDto(MD5 item)
        {
            return new MD5ItemListDto
            {
                ID = item.ID,
                StartValue = item.StartValue,
                EndValue = item.EndValue,
                Days = item.Days,
                Status = item.Status
            };
        } // MD5ToItemListDto
        
        public static MD5ItemDetailDto MD5ToItemDetailDto(MD5 item)
        {
            return new MD5ItemDetailDto
            {
                ID = item.ID,
                StartValue = item.StartValue,
                EndValue = item.EndValue,
                Days = item.Days,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser
            };
        } // MD5ToItemDetailDto

        public static MD5 ItemCreateDtoToMD5(MD5ItemCreateDto item)
        {
            return new MD5
            {
                StartValue = item.StartValue,
                EndValue = item.EndValue,
                Days = item.Days,
                Status = item.Status ?? StatusType.Nothing,
                UpdatedUser = item.UpdatedUser
            };
        } // ItemAddDtoToMD5

        public static MD5 ItemUpdateDtoToMD5(MD5ItemUpdateDto item)
        {
            return new MD5
            {
                ID = item.ID,
                StartValue = item.StartValue,
                EndValue = item.EndValue,
                Days = item.Days,
                UpdatedUser = item.UpdatedUser
            };
        } // ItemUpdateDtoToMD5

        public static MD5 ItemDeleteDtoToMD5(MD5ItemDeleteDto item)
        {
            return new MD5
            {
                ID = item.ID,
                UpdatedUser = item.UpdatedUser
            };
        } // ItemDeleteDtoToMD5
    }
}