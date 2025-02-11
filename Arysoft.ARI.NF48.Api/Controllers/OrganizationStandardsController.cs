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
    public class OrganizationStandardsController : ApiController
    {
        private readonly OrganizationStandardService _service;

        // CONSTRUCTOR

        public OrganizationStandardsController()
        {
            _service = new OrganizationStandardService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<OrganizationStandardItemListDto>>))]
        public IHttpActionResult GetOrganizationStandards([FromUri] OrganizationStandardQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = OrganizationStandardMapping.OrganizationStandardToListDto(items);
            var response = new ApiResponse<IEnumerable<OrganizationStandardItemListDto>>(itemsDto)
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
        } // GetOrganizationStandards

        [ResponseType(typeof(ApiResponse<OrganizationStandardItemDetailDto>))]
        public async Task<IHttpActionResult> GetOrganizationStandard(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = OrganizationStandardMapping.OrganizationStandardToItemDetailDto(item);
            var response = new ApiResponse<OrganizationStandardItemDetailDto>(itemDto);

            return Ok(response);
        } // GetOrganizationStandard

        public async Task<IHttpActionResult> PostOrganizationStandard([FromBody] OrganizationStandardPostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = OrganizationStandardMapping.ItemAddDtoToOrganizationStandard(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = OrganizationStandardMapping.OrganizationStandardToItemDetailDto(item);
            var response = new ApiResponse<OrganizationStandardItemDetailDto>(itemDto);

            return Ok(response);
        } // PostOrganizationStandard

        public async Task<IHttpActionResult> PutOrganizationStandard(Guid id, [FromBody] OrganizationStandardPutDto itemPutDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemPutDto.ID)
                throw new BusinessException("ID mismatch");

            var item = OrganizationStandardMapping.ItemEditDtoToOrganizationStandard(itemPutDto);
            item = await _service.UpdateAsync(item);
            var itemDto = OrganizationStandardMapping.OrganizationStandardToItemDetailDto(item);
            var response = new ApiResponse<OrganizationStandardItemDetailDto>(itemDto);

            return Ok(response);
        } // PutOrganizationStandard

        public async Task<IHttpActionResult> DeleteOrganizationStandard(Guid id, [FromBody] OrganizationStandardDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = OrganizationStandardMapping.ItemDeleteDtoToOrganizationStandard(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteOrganizationStandard
    }
}
