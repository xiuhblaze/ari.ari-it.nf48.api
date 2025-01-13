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
    public class AuditCycleStandardsController : ApiController
    {
        private readonly AuditCycleStandardService _service;

        // CONSTRUCTOR

        public AuditCycleStandardsController()
        {
            _service = new AuditCycleStandardService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<AuditCycleStandardItemListDto>>))]
        public IHttpActionResult GetAuditCycleStandards([FromUri] AuditCycleStandardQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = AuditCycleStandardMapping.AuditCycleStandardsToListDto(items);
            var response = new ApiResponse<IEnumerable<AuditCycleStandardItemListDto>>(itemsDto)
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
        } // GetAuditCycleStandards

        [ResponseType(typeof(ApiResponse<AuditCycleStandardItemDetailDto>))]
        public async Task<IHttpActionResult> GetAuditCycle(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = AuditCycleStandardMapping.AuditCycleStandardToItemDetailDto(item);
            var response = new ApiResponse<AuditCycleStandardItemDetailDto>(itemDto);

            return Ok(response);
        } // GetAuditCycle

        [HttpPost]
        [ResponseType(typeof(ApiResponse<AuditCycleStandardItemDetailDto>))]
        public async Task<IHttpActionResult> PostAuditCycleStandard(AuditCycleStandardPostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = AuditCycleStandardMapping.ItemAddDtoToAuditCycleStandard(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = AuditCycleStandardMapping.AuditCycleStandardToItemDetailDto(item);
            var response = new ApiResponse<AuditCycleStandardItemDetailDto>(itemDto);

            return Ok(response);
        } // PostAuditCycleStandard

        [HttpPut]
        [ResponseType(typeof(ApiResponse<AuditCycleStandardItemDetailDto>))]
        public async Task<IHttpActionResult> PutAuditCycleStandard(Guid id, [FromBody] AuditCycleStandardPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = AuditCycleStandardMapping.ItemEditDtoToAuditCycleStandard(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = AuditCycleStandardMapping.AuditCycleStandardToItemDetailDto(item);
            var response = new ApiResponse<AuditCycleStandardItemDetailDto>(itemDto);

            return Ok(response);
        } // PutAuditCycle

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteAuditCycleStandard(Guid id, [FromBody] AuditCycleStandardDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = AuditCycleStandardMapping.ItemDeleteDtoToAuditCycleStandard(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        }
    }
}
