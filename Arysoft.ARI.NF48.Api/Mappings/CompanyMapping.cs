using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class CompanyMapping
    {
        public static IEnumerable<CompanyListDto> CompanyToListDto(IEnumerable<Company> items)
        {
            var itemsDto = new List<CompanyListDto>();
            foreach (var item in items)
            {
                itemsDto.Add(CompanyToItemListDto(item));
            }
            return itemsDto;
        } // CompanyToListDto

        public static CompanyListDto CompanyToItemListDto(Company item)
        {
            return new CompanyListDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                Name = item.Name,
                LegalEntity = item.LegalEntity,
                Status = item.Status,
                UpdatedUser = item.UpdatedUser,
                OrganizationName = item.Organization != null
                    ? item.Organization.Name
                    : string.Empty
            };
        } // CompanyToItemListDto

        public static CompanyDetailDto CompanyToItemDetailDto(Company item)
        {
            return new CompanyDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                Name = item.Name,
                LegalEntity = item.LegalEntity,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Organization = item.Organization != null
                    ? OrganizationMapping.OrganizationToSummaryDto(item.Organization)
                    : null
            };
        } // CompanyToItemDetailDto

        public static Company ItemCreateDtoToCompany(CompanyPostDto itemDto)
        {
            return new Company
            {
                OrganizationID = itemDto.OrganizationID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemCreateDtoToCompany

        public static Company ItemEditDtoToCompany(CompanyPutDto itemDto)
        {
            return new Company
            {
                ID = itemDto.ID,
                Name = itemDto.Name,
                LegalEntity = itemDto.LegalEntity,
                //COID = itemDto.COID,
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