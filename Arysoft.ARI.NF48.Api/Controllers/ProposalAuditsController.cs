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
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class ProposalAuditsController : ApiController
    {
        private readonly ProposalAuditService _service;

        // CONSTRUCTOR

        public ProposalAuditsController()
        {
            _service = new ProposalAuditService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<ProposalAuditItemDto>>))]
        public IHttpActionResult GetProposalAudits([FromUri] ProposalAuditQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = ProposalAuditMapping.ProposalAuditToListDto(items);
            var response = new ApiResponse<IEnumerable<ProposalAuditItemDto>>(itemsDto)
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
        } // GetProposalAudits

        [HttpGet]
        [ResponseType(typeof(ApiResponse<ProposalAuditItemDto>))]
        public async Task<IHttpActionResult> GetProposalAudit(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new Exceptions.BusinessException("Item not found");
            var itemDto = ProposalAuditMapping.ProposalAuditToItemDto(item);
            var response = new ApiResponse<ProposalAuditItemDto>(itemDto);

            return Ok(response);
        } // GetProposalAudit

        [HttpPost]
        [ResponseType(typeof(ApiResponse<ProposalAuditItemDto>))]
        public async Task<IHttpActionResult> PostProposalAudit([FromBody] ProposalAuditCreateDto itemCreateDto)
        {
            if (!ModelState.IsValid)
                throw new Exceptions.BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = ProposalAuditMapping.ItemCreateDtoToProposalAudit(itemCreateDto);
            var createdItem = await _service.CreateAsync(item);
            var createdItemDto = ProposalAuditMapping.ProposalAuditToItemDto(createdItem);
            var response = new ApiResponse<ProposalAuditItemDto>(createdItemDto);

            return Ok(response);
        } // PostProposalAudit

        [HttpPut]
        [ResponseType(typeof(ApiResponse<ProposalAuditItemDto>))]
        public async Task<IHttpActionResult> PutProposalAudit([FromBody] ProposalAuditUpdateDto itemUpdateDto)
        {
            if (!ModelState.IsValid)
                throw new Exceptions.BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = ProposalAuditMapping.ItemUpdateDtoToProposalAudit(itemUpdateDto);
            var updatedItem = await _service.UpdateAsync(item);
            var updatedItemDto = ProposalAuditMapping.ProposalAuditToItemDto(updatedItem);
            var response = new ApiResponse<ProposalAuditItemDto>(updatedItemDto);
            
            return Ok(response);
        } // PutProposalAudit

        [HttpPut]
        [Route("api/proposalaudits/list")]
        [ResponseType(typeof(ApiResponse<IEnumerable<ProposalAuditItemDto>>))]
        public async Task<IHttpActionResult> PutProposalAuditList([FromBody] IEnumerable<ProposalAuditUpdateDto> itemsUpdateDto)
        {
            if (!ModelState.IsValid)
                throw new Exceptions.BusinessException(Strings.GetModelStateErrors(ModelState));

            var items = ProposalAuditMapping.ItemsUpdateDtoToProposalAuditList(itemsUpdateDto);
            var updatedItems = await _service.UpdatedListAsync(items.ToList());
            var updatedItemsDto = ProposalAuditMapping.ProposalAuditToListDto(updatedItems);
            var response = new ApiResponse<IEnumerable<ProposalAuditItemDto>>(updatedItemsDto);

            return Ok(response);
        } // PutProposalAuditList

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteProposalAudit(Guid id, [FromBody] ProposalAuditDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = ProposalAuditMapping.ItemDeleteDtoToProposalAudit(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteProposalAudit
    }
}
