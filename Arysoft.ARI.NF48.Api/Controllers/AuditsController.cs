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
    public class AuditsController : ApiController
    {
        private readonly AuditService _service;

        // CONSTRUCTOR

        public AuditsController()
        {
            _service = new AuditService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<AuditItemListDto>>))]
        public IHttpActionResult GetAudits([FromUri] AuditQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemDto = AuditMapping.AuditToListDto(items);
            var response = new ApiResponse<IEnumerable<AuditItemListDto>>(itemDto)
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
        } // GetAudits

        [ResponseType(typeof(ApiResponse<AuditItemDetailDto>))]
        public async Task<IHttpActionResult> GetAudit(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = AuditMapping.AuditToItemDetailDto(item);
            var response = new ApiResponse<AuditItemDetailDto>(itemDto);

            return Ok(response);
        } // GetAudit

        [HttpGet]
        [Route("api/Audits/has-auditor-an-audit")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> HasAuditorAnAudit(AuditorInAuditDto auditorInAuditDto)
        {
            var hasAudit = await _service.HasAuditorAnAudit(
                auditorInAuditDto.AuditorID,
                auditorInAuditDto.StartDate,
                auditorInAuditDto.EndDate,
                auditorInAuditDto.AuditExceptionID);
            var response = new ApiResponse<bool>(hasAudit);
            return Ok(response);
        } // GetHasAuditorAnAudit

        [HttpGet]
        [Route("api/Audits/has-standard-step-an-audit")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> HasStandardStepAnAudit(StandardStepInAuditCycleDto valuesDto)
        {
            var hasStandardStep = await _service.IsAnyStandardStepAuditInAuditCycle(
                valuesDto.AuditCycleID,
                valuesDto.StandardID,
                valuesDto.Step,
                valuesDto.AuditExceptionID
                );
            var response = new ApiResponse<bool>(hasStandardStep);
            return Ok(response);
        } // HasStandardStepAnAudit

        [HttpPost]
        [ResponseType(typeof(ApiResponse<AuditItemDetailDto>))]
        public async Task<IHttpActionResult> PostAudit([FromBody] AuditPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = AuditMapping.ItemAddDtoToAudit(itemAddDto);
            item = await _service.AddAsync(item);
            var itemDto = AuditMapping.AuditToItemDetailDto(item);
            var response = new ApiResponse<AuditItemDetailDto>(itemDto);

            return Ok(response);
        } // PostAudit

        public async Task<IHttpActionResult> PutAudit(Guid id, [FromBody] AuditPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = AuditMapping.ItemEditDtoToAudit(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = AuditMapping.AuditToItemDetailDto(item);
            var response = new ApiResponse<AuditItemDetailDto>(itemDto);

            return Ok(response);
        } // PutAudit

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteAudit(Guid id, [FromBody] AuditDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = AuditMapping.ItemDeleteDtoToAudit(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteAudit
    }
}
