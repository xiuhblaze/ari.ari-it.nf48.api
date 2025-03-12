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
    public class AuditAuditorsController : ApiController
    {
        private readonly AuditAuditorService _service;

        // CONSTRUCTOR

        public AuditAuditorsController()
        {
            _service = new AuditAuditorService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<AuditAuditorItemListDto>>))]
        public IHttpActionResult GetAuditAuditors([FromUri] AuditAuditorQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemDto = AuditAuditorMapping.AuditAuditorToListDto(items);
            var response = new ApiResponse<IEnumerable<AuditAuditorItemListDto>>(itemDto)
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
        } // GetAuditAuditors

        [HttpGet]
        [ResponseType(typeof(ApiResponse<AuditAuditorItemDetailDto>))]
        public async Task<IHttpActionResult> GetAuditAuditor(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = AuditAuditorMapping.AuditAuditorToItemDetailDto(item);
            var response = new ApiResponse<AuditAuditorItemDetailDto>(itemDto);

            return Ok(response);
        } // GetAuditAuditor

        [HttpPost]
        [ResponseType(typeof(ApiResponse<AuditAuditorItemDetailDto>))]
        public async Task<IHttpActionResult> PostAuditAuditor(AuditAuditorPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = AuditAuditorMapping.ItemAddDtoToAuditAuditor(itemAddDto);
            item = await _service.AddAsync(item);
            var itemDto = AuditAuditorMapping.AuditAuditorToItemDetailDto(item);
            var response = new ApiResponse<AuditAuditorItemDetailDto>(itemDto);

            return Ok(response);
        } // PostAuditAuditor

        [HttpPut]
        [ResponseType(typeof(ApiResponse<AuditAuditorItemDetailDto>))]
        public async Task<IHttpActionResult> PutAuditAuditor(Guid id, [FromBody] AuditAuditorPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = AuditAuditorMapping.ItemEditDtoToAuditAuditor(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = AuditAuditorMapping.AuditAuditorToItemDetailDto(item);
            var response = new ApiResponse<AuditAuditorItemDetailDto>(itemDto);

            return Ok(response);
        } // PutAuditAuditor

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteAuditAuditor(Guid id, [FromBody] AuditAuditorDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = AuditAuditorMapping.ItemDeleteDtoToAuditAuditor(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteAuditAuditor

        // AUDIT STANDARDS

        [HttpPost]
        [Route("api/AuditAuditors/{id}/audit-standard")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> AddAuditStandard(Guid id, [FromBody] AuditAuditorEditAuditStandardDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.AuditAuditorID)
                throw new BusinessException("ID mismatch");

            await _service.AddAuditStandardAsync(itemDto.AuditAuditorID, itemDto.AuditStandardID);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // AddAuditStandard

        [HttpDelete]
        [Route("api/AuditAuditors/{id}/audit-standard")]
        public async Task<IHttpActionResult> DelAuditStandard(Guid id, [FromBody] AuditAuditorEditAuditStandardDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.AuditAuditorID)
                throw new BusinessException("ID mismatch");

            await _service.DelAuditStandardAsync(itemDto.AuditAuditorID, itemDto.AuditStandardID);
            var response = new ApiResponse<bool>(true);
            
            return Ok(response);
        } // DelAuditStandard
    }
}
