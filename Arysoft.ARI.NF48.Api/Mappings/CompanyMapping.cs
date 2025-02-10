using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class CompanyMapping
    {
        public static IEnumerable<CompanyItemListDto> CompanyToListDto(IEnumerable<Company> items)
        {
            var itemsDto = new List<CompanyItemListDto>();
            foreach (var item in items)
            {
                itemsDto.Add(CompanyToItemListDto(item));
            }
            return itemsDto;
        } // CompanyToListDto

        public static CompanyItemListDto CompanyToItemListDto(Company item)
        {
            return new CompanyItemListDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                Name = item.Name,
                LegalEntity = item.LegalEntity,
                COID = item.COID,
                Status = item.Status,
                UpdatedUser = item.UpdatedUser,
                OrganizationName = item.Organization != null
                    ? item.Organization.Name
                    : string.Empty
            };
        } // CompanyToItemListDto

        public static CompanyItemDetailDto CompanyToItemDetailDto(Company item)
        {
            return new CompanyItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                Name = item.Name,
                LegalEntity = item.LegalEntity,
                COID = item.COID,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Organization = item.Organization != null
                    ? OrganizationMapping.OrganizationToItemListDto(item.Organization)
                    : null
            };
        } // CompanyToItemDetailDto

        public static Company ItemAddDtoToCompany(CompanyPostDto itemDto)
        {
            return new Company
            {
                OrganizationID = itemDto.OrganizationID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToCompany

        public static Company ItemEditDtoToCompany(CompanyPutDto itemDto)
        {
            return new Company
            {
                ID = itemDto.ID,
                Name = itemDto.Name,
                LegalEntity = itemDto.LegalEntity,
                COID = itemDto.COID,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToCompany

        public static Company ItemDeleteDtoToCompany(CompanyDeleteDto itemDto)
        {
            return new Company
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToCompany
    }
}