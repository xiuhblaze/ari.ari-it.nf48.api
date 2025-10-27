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
    public class ADCsController : ApiController
    {
        private readonly ADCService _service;

        // CONSTRUCTOR

        public ADCsController()
        {
            _service = new ADCService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<ADCItemListDto>>))]
        public IHttpActionResult GetADCs([FromUri] ADCQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = ADCMapping.ADCToListDto(items);
            var response = new ApiResponse<IEnumerable<ADCItemListDto>>(itemsDto)
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
        } // GetADCs

        [HttpGet]
        [ResponseType(typeof(ApiResponse<ADCItemDetailDto>))]
        public async Task<IHttpActionResult> GetADC(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = ADCMapping.ADCToItemDetailDto(item);
            var response = new ApiResponse<ADCItemDetailDto>(itemDto);
            return Ok(response);
        } // GetADC

        [HttpPost]
        [ResponseType(typeof(ApiResponse<ADCItemDetailDto>))]
        public async Task<IHttpActionResult> PostADC([FromBody] ADCCreateDto itemCreateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = ADCMapping.ItemCreateDtoToADC(itemCreateDto);
            var itemDto = ADCMapping.ADCToItemDetailDto(await _service.AddAsync(item));
            var response = new ApiResponse<ADCItemDetailDto>(itemDto);

            return Ok(response);
        } // PostADC

        [HttpPut]
        [ResponseType(typeof(ApiResponse<ADCItemDetailDto>))]
        public async Task<IHttpActionResult> PutUpdateADC(Guid id, [FromBody] ADCUpdateDto itemUpdateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemUpdateDto.ID)
                throw new BusinessException("ID Mismatch");

            var item = ADCMapping
                .ItemUpdateDtoToADC(itemUpdateDto);
            var itemDto = ADCMapping.ADCToItemDetailDto(await _service.UpdateAsync(item));
            var response = new ApiResponse<ADCItemDetailDto>(itemDto);

            return Ok(response);
        } // PutUpdateADC

        [HttpPut()]
        [Route("api/ADCs/complete")]
        [ResponseType(typeof(ApiResponse<ADCItemDetailDto>))]
        public async Task<IHttpActionResult> PutUpdateCompleteADC([FromBody] ADCWithSiteListUpdateDto itemUpdateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = ADCMapping.ItemUpdateWithListDtoToADC(itemUpdateDto);
            item = await _service.UpdateCompleteADCAsync(item);
            var itemDto = ADCMapping.ADCToItemDetailDto(item);
            var response = new ApiResponse<ADCItemDetailDto>(itemDto);

            return Ok(response);
        } // PutUpdateCompleteADC

        //[HttpPut()]
        //[Route("api/ADCs/{id}/proposal/{proposalID}")]
        //[ResponseType(typeof(ApiResponse<ADCItemDetailDto>))]
        //public async Task<IHttpActionResult> PutUpdateProposalToADC(Guid id, Guid proposalID, ADCUpdateProposalIDDto itemUpdateDto)
        //{   
        //    if (!ModelState.IsValid)
        //        throw new BusinessException(Strings.GetModelStateErrors(ModelState));

        //    if (id != itemUpdateDto.ID)
        //        throw new BusinessException("ID Mismatch");

        //    if (proposalID != itemUpdateDto.ProposalID)
        //        throw new BusinessException("Proposal ID Mismatch");

        //    var item = await _service.UpdateProposalIDAsync(
        //        itemUpdateDto.ID ?? Guid.Empty,
        //        itemUpdateDto.ProposalID ?? Guid.Empty,
        //        itemUpdateDto.UpdatedUser);
        //    var itemDto = ADCMapping.ADCToItemDetailDto(item);
        //    var response = new ApiResponse<ADCItemDetailDto>(itemDto);

        //    return Ok(response);
        //} // PutUpdateProposalToADC

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteADC(Guid id, ADCDeleteDto itemDeleteDto)
        {

            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID Mismatch");

            var item = ADCMapping
                .ItemDeleteDtoToADC(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteADC
    }
}
