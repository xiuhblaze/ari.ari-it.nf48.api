using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Mappings;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;
using Arysoft.ARI.NF48.Api.Services;
using Arysoft.ARI.NF48.Api.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class OrganizationsController : ApiController
    {
        private readonly OrganizationService _organizationService;

        // CONSTRUCTOR 

        public OrganizationsController()
        {
            _organizationService = new OrganizationService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<OrganizationItemListDto>>))]
        public IHttpActionResult GetOrganizations([FromUri] OrganizationQueryFilters filters)
        {
            var items = _organizationService.Gets(filters);
            var itemsDto = OrganizationMapping.OrganizationToListDto(items);
            var response = new ApiResponse<IEnumerable<OrganizationItemListDto>>(itemsDto)
            {
                Meta = new Metadata
                {
                    TotalCount = items.TotalCount,
                    PageSize = items.PageSize,
                    CurrentPage = items.CurrentPage,
                    TotalPages = items.TotalPages,
                    HasPreviousPage = items.HasPreviousPage,
                    HasNextPage = items.HasNextPage
                }
            };

            return Ok(response);
        } // GetOrganizations

        [ResponseType(typeof(ApiResponse<OrganizationItemDetailDto>))]
        public async Task<IHttpActionResult> GetOrganization(Guid id)
        {
            var item = await _organizationService.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = OrganizationMapping.OrganizationToItemDetailDto(item);
            var response = new ApiResponse<OrganizationItemDetailDto>(itemDto);

            return Ok(response);
        } // GetOrganization

        // POST: api/Organization
        [ResponseType(typeof(ApiResponse<OrganizationItemDetailDto>))]
        public async Task<IHttpActionResult> PostOrganization([FromBody] OrganizationPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var itemToAdd = OrganizationMapping.ItemAddDtoToOrganization(itemAddDto);
            var item = await _organizationService.AddAsync(itemToAdd);
            var itemDto = OrganizationMapping.OrganizationToItemDetailDto(item);
            var response = new ApiResponse<OrganizationItemDetailDto>(itemDto);

            return Ok(response);
        } // PostOrganization

        // PUT: api/Organization/5
        [ResponseType(typeof(ApiResponse<OrganizationItemDetailDto>))]
        public async Task<IHttpActionResult> PutOrganization(Guid id, [FromBody] OrganizationPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var itemToEdit = OrganizationMapping.ItemEditDtoToOrganization(itemEditDto);
            var item = await _organizationService.UpdateAsync(itemToEdit);
            var itemDto = OrganizationMapping.OrganizationToItemDetailDto(item);
            var response = new ApiResponse<OrganizationItemDetailDto>(itemDto);

            return Ok(response);
        } // PutOrganization

        // DELETE: api/Organization/5
        public async Task<IHttpActionResult> DeleteOrganization(Guid id, [FromBody] OrganizationDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = OrganizationMapping.ItemDeleteDtoToOrganization(itemDelDto);
            await _organizationService.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteOrganization

    }
}
