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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class OrganizationsFamiliesController : ApiController
    {
        private readonly OrganizationFamilyService _service;

        // CONSTRUCTOR

        public OrganizationsFamiliesController()
        {
            _service = new OrganizationFamilyService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<OrganizationFamilyItemListDto>>))]
        public IHttpActionResult GetOrganizationsFamilies([FromUri] OrganizationFamilyQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = OrganizationFamilyMapping.OrganizationFamilyToListDto(items);
            var response = new ApiResponse<IEnumerable<OrganizationFamilyItemListDto>>(itemsDto)
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
        } // GetOrganizationsFamilies

        [HttpGet]
        [ResponseType(typeof(ApiResponse<OrganizationFamilyItemDetailDto>))]
        public async Task<IHttpActionResult> GetOrganizationFamily(Guid id)
        { 
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = OrganizationFamilyMapping.OrganizationFamilyToItemDetailDto(item);
            var response = new ApiResponse<OrganizationFamilyItemDetailDto>(itemDto);

            return Ok(response);
        } // GetOrganizationFamily

        [HttpPost]
        [ResponseType(typeof(ApiResponse<OrganizationFamilyItemDetailDto>))]
        public async Task<IHttpActionResult> PostOrganizationFamily([FromBody] OrganizationFamilyPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = OrganizationFamilyMapping.ItemAddDtoToOrganizationFamily(itemAddDto);
            item = await _service.AddAsync(item);
            var itemDto = OrganizationFamilyMapping.OrganizationFamilyToItemDetailDto(item);
            var response = new ApiResponse<OrganizationFamilyItemDetailDto>(itemDto);

            return Ok(response);
        } // PostOrganizationFamily

        [HttpPut]
        [ResponseType(typeof(ApiResponse<OrganizationFamilyItemDetailDto>))]
        public async Task<IHttpActionResult> PutOrganizationFamily(Guid id, [FromBody] OrganizationFamilyPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = OrganizationFamilyMapping.ItemEditDtoToOrganizationFamily(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = OrganizationFamilyMapping.OrganizationFamilyToItemDetailDto(item);
            var response = new ApiResponse<OrganizationFamilyItemDetailDto>(itemDto);

            return Ok(response);
        } // PutOrganizationFamily

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteOrganizationFamily(Guid id, [FromBody] OrganizationFamilyDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = OrganizationFamilyMapping.ItemDeleteDtoToOrganizationFamily(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        }
    }
}
