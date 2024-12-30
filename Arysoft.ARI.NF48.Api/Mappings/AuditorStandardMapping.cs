using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Tools;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class AuditorStandardMapping
    {
        public static IEnumerable<AuditorStandardItemListDto> AuditorStandardToListDto(IEnumerable<AuditorStandard> items)
        {
            var itemsDto = new List<AuditorStandardItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(AuditorStandardToItemListDto(item));
            }

            return itemsDto;
        } // AuditorStandardToListDto

        public static AuditorStandardItemListDto AuditorStandardToItemListDto(AuditorStandard item)
        {
            return new AuditorStandardItemListDto
            {
                ID = item.ID,
                AuditorID = item.AuditorID,
                StandardID = item.StandardID ?? Guid.Empty,
                Comments = item.Comments,
                AuditorFullName = item.Auditor != null
                    ? Strings.FullName(item.Auditor.FirstName, item.Auditor.MiddleName, item.Auditor.LastName)
                    : string.Empty,
                StandardName = item.Standard != null
                    ? item.Standard.Name 
                    : string.Empty,
                StandardDescription = item.Standard != null
                    ? item.Standard.Description
                    : string.Empty,
                Status = item.Status
            };
        } // AuditorStandardToItemListDto

        public static AuditorStandardItemDetailDto AuditorStandardToItemDetailDto(AuditorStandard item)
        {
            return new AuditorStandardItemDetailDto
            { 
                ID = item.ID,
                AuditorID = item.AuditorID,
                StandardID = item.StandardID ?? Guid.Empty,
                Comments = item.Comments,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Auditor = item.Auditor != null
                    ? AuditorMapping.AuditorToItemListDto(item.Auditor)
                    : null,
                Standard = item.Standard != null
                    ? StandardMapping.StandardToItemListDto(item.Standard)
                    : null
            };
        } // AuditorStandardToItemDetailDto

        public static AuditorStandard ItemAddDtoToAuditorStandard(AuditorStandardPostDto itemDto)
        {
            return new AuditorStandard
            {
                AuditorID = itemDto.AuditorID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToAuditorStandard

        public static AuditorStandard ItemEditDtoToAuditorStandard(AuditorStandardPutDto itemDto)
        {
            return new AuditorStandard
            {
                ID = itemDto.ID,
                StandardID = itemDto.StandardID,
                Comments = itemDto.Comments,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToAuditorStandard

        public static AuditorStandard ItemDeleteDtoToAuditorStandard(AuditorStandardDeleteDto itemDto)
        {
            return new AuditorStandard
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToAuditorStandard
    }
}