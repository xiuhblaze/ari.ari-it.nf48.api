using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class OrganizationStandardMapping
    {
        public static IEnumerable<OrganizationStandardItemListDto> OrganizationStandardToListDto(IEnumerable<OrganizationStandard> items)
        { 
            var itemsDto = new List<OrganizationStandardItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(OrganizationStandardToItemListDto(item));
            }

            return itemsDto;
        } // OrganizationStandardToListDto

        public static OrganizationStandardItemListDto OrganizationStandardToItemListDto(OrganizationStandard item)
        {
            return new OrganizationStandardItemListDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                StandardID = item.StandardID ?? Guid.Empty,
                ExtraInfo = item.ExtraInfo,
                OrganizationName = item.Organization != null
                    ? item.Organization.Name 
                    : string.Empty,
                StandardName = item.Standard != null
                    ? item.Standard.Name
                    : string.Empty,
                StandardBase = item.Standard != null
                    ? item.Standard.StandardBase ?? StandardBaseType.Nothing
                    : StandardBaseType.Nothing,
                Status = item.Status
            };
        } // OrganizationStandardToItemListDto

        public static OrganizationStandardItemDetailDto OrganizationStandardToItemDetailDto(OrganizationStandard item)
        {
            return new OrganizationStandardItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                StandardID = item.StandardID ?? Guid.Empty,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Organization = item.Organization != null
                    ? OrganizationMapping.OrganizationToItemListDto(item.Organization) 
                    : null,
                Standard = item.Standard != null
                    ? StandardMapping.StandardToItemListDto(item.Standard)
                    : null
            };
        } // OrganizationStandardToItemDetailDto

        public static OrganizationStandard ItemAddDtoToOrganizationStandard(OrganizationStandardPostDto itemDto)
        {
            return new OrganizationStandard
            { 
                OrganizationID = itemDto.OrganizationID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToOrganizationStandard

        public static OrganizationStandard ItemEditDtoToOrganizationStandard(OrganizationStandardPutDto itemDto)
        {
            return new OrganizationStandard
            {
                ID = itemDto.ID,
                StandardID = itemDto.StandardID,
                ExtraInfo = itemDto.ExtraInfo,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToOrganizationStandard

        public static OrganizationStandard ItemDeleteDtoToOrganizationStandard(OrganizationStandardDeleteDto itemDto)
        {
            return new OrganizationStandard
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToOrganizationStandard
    }
}