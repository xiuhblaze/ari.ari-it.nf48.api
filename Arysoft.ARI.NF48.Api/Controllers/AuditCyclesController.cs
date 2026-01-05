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
    public class AuditCyclesController : ApiController
    {
        private readonly AuditCycleService _service;

        // CONSTRUCTOR

        public AuditCyclesController()
        {
            _service = new AuditCycleService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<AuditCycleItemListDto>>))]
        public IHttpActionResult GetAuditCycles([FromUri] AuditCycleQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = AuditCycleMapping.AuditCyclesToListDto(items);
            var response = new ApiResponse<IEnumerable<AuditCycleItemListDto>>(itemsDto)
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
        } // GetAuditCycles 

        [ResponseType(typeof(ApiResponse<AuditCycleItemDetailDto>))]
        public async Task<IHttpActionResult> GetAuditCycle(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = await AuditCycleMapping.AuditCycleToItemDetailDto(item);
            var response = new ApiResponse<AuditCycleItemDetailDto>(itemDto);

            return Ok(response);
        } // GetAuditCycle

        [HttpPost]
        [ResponseType(typeof(ApiResponse<AuditCycleItemDetailDto>))]
        public async Task<IHttpActionResult> PostAuditCycle([FromBody] AuditCyclePostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = AuditCycleMapping.ItemAddDtoToAuditCycle(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = await AuditCycleMapping.AuditCycleToItemDetailDto(item);
            var response = new ApiResponse<AuditCycleItemDetailDto>(itemDto);

            return Ok(response);
        } // PostAuditCycle

        [HttpPut]
        [ResponseType(typeof(ApiResponse<AuditCycleItemDetailDto>))]
        public async Task<IHttpActionResult> PutAuditCycle(Guid id, [FromBody] AuditCyclePutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = AuditCycleMapping.ItemEditDtoToAuditCycle(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = await AuditCycleMapping.AuditCycleToItemDetailDto(item);
            var response = new ApiResponse<AuditCycleItemDetailDto>(itemDto);

            return Ok(response);
        } // PutAuditCycle

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteAuditCycle(Guid id, [FromBody] AuditCycleDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = AuditCycleMapping.ItemDeleteDtoToAuditCycle(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteAuditCycle

        [HttpPost]
        [ResponseType(typeof(ApiResponse<bool>))]
        [Route("api/auditcycles/createmissingauditcycles")]
        public async Task<IHttpActionResult> CreateMissingAuditCycles()
        { 
            await _service.CreateMissingAuditCyclesAsync();
            var response = new ApiResponse<bool>(true);
            return Ok(response);
        } // CreateMissingAuditCycles
    }
}
