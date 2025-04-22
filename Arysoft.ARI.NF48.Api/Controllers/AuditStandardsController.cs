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
    public class AuditStandardsController : ApiController
    {
        private readonly AuditStandardService _service;

        // CONSTRUCTOR

        public AuditStandardsController()
        {
            _service = new AuditStandardService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<AuditStandardItemListDto>>))]
        public IHttpActionResult GetAuditStandards([FromUri] AuditStandardQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemDto = AuditStandardMapping.AuditStandardToListDto(items);
            var response = new ApiResponse<IEnumerable<AuditStandardItemListDto>>(itemDto)
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
        } // GetAuditStandards

        [HttpGet]
        [ResponseType(typeof(ApiResponse<AuditStandardItemDetailDto>))]
        public async Task<IHttpActionResult> GetAuditStandard(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = AuditStandardMapping.AuditStandardToItemDetailDto(item);
            var response = new ApiResponse<AuditStandardItemDetailDto>(itemDto);

            return Ok(response);
        } // GetAuditStandard

        [HttpPost]
        [ResponseType(typeof(ApiResponse<AuditStandardItemDetailDto>))]
        public async Task<IHttpActionResult> PostAuditStandard(AuditStandardPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = AuditStandardMapping.ItemAddDtoToAuditStandard(itemAddDto);
            item = await _service.AddAsync(item);
            var itemDto = AuditStandardMapping.AuditStandardToItemDetailDto(item);
            var response = new ApiResponse<AuditStandardItemDetailDto>(itemDto);

            return Ok(response);
        } // PostAuditStandard

        [HttpPut]
        [ResponseType(typeof(ApiResponse<AuditStandardItemDetailDto>))]
        public async Task<IHttpActionResult> PutAuditStandard(Guid id, [FromBody] AuditStandardPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = AuditStandardMapping.ItemEditDtoToAuditStandard(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = AuditStandardMapping.AuditStandardToItemDetailDto(item);
            var response = new ApiResponse<AuditStandardItemDetailDto>(itemDto);

            return Ok(response);
        } // PutAuditStandard

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteAuditStandard(Guid id, [FromBody] AuditStandardDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = AuditStandardMapping.ItemDeleteDtoToAuditStandard(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteAuditStandard
    }
}
