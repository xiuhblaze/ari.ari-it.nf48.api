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
    public class ADCSiteAuditsController : ApiController
    {
        private readonly ADCSiteAuditService _service;

        // CONSTRUCTOR

        public ADCSiteAuditsController()
        {
            _service = new ADCSiteAuditService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<ADCSiteAuditItemDto>>))]
        public IHttpActionResult GetADCSiteAudits([FromUri] ADCSiteAuditQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = ADCSiteAuditMapping.ADCSiteAuditToListDto(items);
            var response = new ApiResponse<IEnumerable<ADCSiteAuditItemDto>>(itemsDto)
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
        } // GetADCSiteAudits

        public async Task<IHttpActionResult> GetADCSiteAudit(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new Exceptions.BusinessException("Item not found");
            var itemDto = ADCSiteAuditMapping
                .ADCSiteAuditToItemDto(item);
            var response = new ApiResponse<ADCSiteAuditItemDto>(itemDto);

            return Ok(response);
        } // GetADCSiteAudit

        [HttpPost]
        [ResponseType(typeof(ApiResponse<ADCSiteAuditItemDto>))]
        public async Task<IHttpActionResult> PostADCSiteAudit([FromBody] ADCSiteAuditCreateDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new Exceptions.BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = ADCSiteAuditMapping
                .ItemCreateDtoToADCSiteAudit(itemDto);
            item = await _service.CreateAsync(item);
            var createdItemDto = ADCSiteAuditMapping
                .ADCSiteAuditToItemDto(item);
            var response = new ApiResponse<ADCSiteAuditItemDto>(createdItemDto);

            return Ok(response);
        } // PostADCSiteAudit

        [HttpPut]
        [ResponseType(typeof(ApiResponse<ADCSiteAuditItemDto>))]
        public async Task<IHttpActionResult> PutADCSiteAudit(Guid id, [FromBody] ADCSiteAuditUpdateDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new Exceptions.BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.ID)
                throw new BusinessException("ID mismatch");

            var item = ADCSiteAuditMapping
                .ItemUpdateDtoToADCSiteAudit(itemDto);
            item = await _service.UpdateAsync(item);
            var updatedItemDto = ADCSiteAuditMapping
                .ADCSiteAuditToItemDto(item);
            var response = new ApiResponse<ADCSiteAuditItemDto>(updatedItemDto);

            return Ok(response);
        } // PutADCSiteAudit

        [HttpPut]
        [Route("api/ADCSiteAudits/list")]
        [ResponseType(typeof(ApiResponse<IEnumerable<ADCSiteAuditItemDto>>))]
        public async Task<IHttpActionResult> PutADCSiteAudits([FromBody] ADCSiteAuditListUpdateDto itemsDto)
        {
            if (!ModelState.IsValid)
                throw new Exceptions.BusinessException(Strings.GetModelStateErrors(ModelState));

            var items = ADCSiteAuditMapping
                .UpdateListDtoToADCSiteAudit(itemsDto);
            var updatedItems = await _service.UpdateListAsync(items.ToList());
            var updatedItemsDto = ADCSiteAuditMapping
                .ADCSiteAuditToListDto(updatedItems);

            var response = new ApiResponse<IEnumerable<ADCSiteAuditItemDto>>(updatedItemsDto);
            
            return Ok(response);
        } // PutADCSiteAudits

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteADCSiteAudit(Guid id, [FromUri] ADCSiteAuditDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new Exceptions.BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = ADCSiteAuditMapping.ItemDeleteDtoToADCSiteAudit(itemDeleteDto);            
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteADCSiteAudit
    }
}
