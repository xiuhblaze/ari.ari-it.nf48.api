using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.IO;
using Arysoft.ARI.NF48.Api.Mappings;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;
using Arysoft.ARI.NF48.Api.Services;
using Arysoft.ARI.NF48.Api.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class ProposalsController : ApiController
    {
        private readonly ProposalService _service;

        // CONSTRUCTOR

        public ProposalsController()
        {
            _service = new ProposalService();
        } // ProposalController

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<ProposalItemListDto>>))]
        public IHttpActionResult GetProposals([FromUri] ProposalQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = Mappings.ProposalMapping.ProposalToListDto(items);
            var  response = new ApiResponse<IEnumerable<ProposalItemListDto>>(itemsDto)
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
        } // GetProposals

        [HttpGet]
        [ResponseType(typeof(ApiResponse<ProposalItemDetailDto>))]
        public async Task<IHttpActionResult> GetProposal(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new Exceptions.BusinessException("Item not found");
            var itemDto = Mappings.ProposalMapping.ProposalToItemDetailDto(item);
            var response = new ApiResponse<ProposalItemDetailDto>(itemDto);

            return Ok(response);
        } // GetProposal

        [HttpPost]
        [ResponseType(typeof(ApiResponse<ProposalItemDetailDto>))]
        public async Task<IHttpActionResult> PostProposal(ProposalCreateDto itemCreateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = ProposalMapping.ItemCreateDtoToProposal(itemCreateDto);
            item = await _service.CreateAsync(item);
            var itemDto = ProposalMapping.ProposalToItemDetailDto(item);
            var response = new ApiResponse<ProposalItemDetailDto>(itemDto);

            return Ok(response);
        } // PostProposal

        [HttpPut]
        [ResponseType(typeof(ApiResponse<ProposalItemDetailDto>))]
        public async Task<IHttpActionResult> PutProposal()
        {
            var data = HttpContext.Current.Request.Form["data"];
            var file = HttpContext.Current.Request.Files.Count > 0 
                ? HttpContext.Current.Request.Files[0] 
                : null;
            string filename = null;

            if (string.IsNullOrEmpty(data))
                throw new BusinessException("No data");

            ProposalUpdateDto itemUpdateDto = JsonConvert
                .DeserializeObject<ProposalUpdateDto>(data)
                ?? throw new BusinessException("Can't read data object");

            var item = await _service.GetAsync(itemUpdateDto.ID ?? Guid.Empty)
                ?? throw new BusinessException("Item not found");

            if (file != null)
            {
                filename = FileRepository.UploadFile(
                    file,
                    $"~/files/{item.AuditCycle.OrganizationID}/Cycles/{item.AuditCycle.ID}/Proposals",
                    item.ID.ToString(),
                    new string[] { ".docx", "xlsx", ".pdf", ".jpg", ".png" }
                );
            }

            var itemToEdit = ProposalMapping.ItemUpdateDtoToProposal(itemUpdateDto);

            itemToEdit.SignedFilename = filename ?? item.SignedFilename;

            item = await _service.UpdateAsync(itemToEdit);
            var itemDto = ProposalMapping.ProposalToItemDetailDto(item);
            var response = new ApiResponse<ProposalItemDetailDto>(itemDto);

            return Ok(response);
        } // PutProposal

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteProposal(Guid id, [FromBody] ProposalDeleteDto itemDeleteDto )
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = ProposalMapping.ItemDeleteDtoToProposal(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteProposal

        // ADCs

        [HttpPost]
        [Route("api/Proposals/{id}/adc")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> AddADC(Guid id, [FromBody] ProposalADCDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.ProposalID)
                throw new BusinessException("ID mismatch");

            var item = new Proposal
            {
                ID = itemDto.ProposalID ?? Guid.Empty,
                UpdatedUser = itemDto.UpdatedUser
            };
            await _service.AddADCAsync(item, itemDto.ADCID ?? Guid.Empty);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // PostProposalAddADC

        [HttpDelete]
        [Route("api/Proposals/{id}/adc")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> RemoveADC(Guid id, [FromBody] ProposalADCDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.ProposalID)
                throw new BusinessException("ID mismatch");

            var item = new Proposal
            {
                ID = itemDto.ProposalID ?? Guid.Empty,
                UpdatedUser = itemDto.UpdatedUser
            };
            await _service.RemoveADCAsync(item, itemDto.ADCID ?? Guid.Empty);
            var response = new ApiResponse<bool>(true);
            
            return Ok(response);
        } // PostProposalRemoveADC
    }
}
