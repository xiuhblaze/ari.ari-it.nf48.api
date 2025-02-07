using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class OrganizationFamilyMapping
    {
        public static IEnumerable<OrganizationFamilyItemListDto> OrganizationFamilyToListDto(IEnumerable<OrganizationFamily> items)
        { 
            var itemsDto = new List<OrganizationFamilyItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(OrganizationFamilyToItemListDto(item));
            }

            return itemsDto;
        } // OrganizationFamilyToListDto

        public static OrganizationFamilyItemListDto OrganizationFamilyToItemListDto(OrganizationFamily item)
        {
            return new OrganizationFamilyItemListDto
            {
                ID = item.ID,
                Description = item.Description,
                Status = item.Status,
                Organizations = item.Organizations != null
                    ? OrganizationMapping.OrganizationToSimpleListDto(item.Organizations
                        .Where(o => o.Status != OrganizationStatusType.Nothing))
                    : null
            };
        } // OrganizationFamilyToItemListDto

        public static OrganizationFamilyItemDetailDto OrganizationFamilyToItemDetailDto(OrganizationFamily item)
        {
            return new OrganizationFamilyItemDetailDto
            {
                ID = item.ID,
                Description = item.Description,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Organizations = item.Organizations != null
                    ? OrganizationMapping.OrganizationToListDto(item.Organizations
                        .Where(o => o.Status != OrganizationStatusType.Nothing))
                    : null
            };
        } // OrganizationFamilyToItemDetailDto

        public static OrganizationFamily ItemAddDtoToOrganizationFamily(OrganizationFamilyPostDto itemDto)
        {
            return new OrganizationFamily
            {
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToOrganizationFamily

        public static OrganizationFamily ItemEditDtoToOrganizationFamily(OrganizationFamilyPutDto itemDto)
        {
            return new OrganizationFamily
            { 
                ID = itemDto.ID,
                Description = itemDto.Description,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToOrganizationFamily

        public static OrganizationFamily ItemDeleteDtoToOrganizationFamily(OrganizationFamilyDeleteDto itemDto)
        {
            return new OrganizationFamily
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToOrganizationFamily
    }
}