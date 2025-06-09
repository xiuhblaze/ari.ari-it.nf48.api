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
    public class ADCConceptsController : ApiController
    {
        private readonly ADCConceptService _service;

        // CONSTRUCTOR

        public ADCConceptsController()
        {
            _service = new ADCConceptService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<ADCConceptItemListDto>>))]
        public IHttpActionResult GetADCConcepts([FromUri] ADCConceptQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = ADCConceptMapping.ADCConceptToListDto(items);
            var response = new ApiResponse<IEnumerable<ADCConceptItemListDto>>(itemsDto)
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
        } // GetADCConcepts

        [HttpGet]
        [ResponseType(typeof(ApiResponse<ADCConceptItemDetailDto>))]
        public async Task<IHttpActionResult> GetADCConcept(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = ADCConceptMapping
                .ADCConceptToItemDetailDto(item);
            var response = new ApiResponse<ADCConceptItemDetailDto>(itemDto);

            return Ok(response);
        } // GetADCConcept

        [HttpPost]
        [ResponseType(typeof(ApiResponse<ADCConceptItemDetailDto>))]
        public async Task<IHttpActionResult> PostADCConcept([FromBody] ADCConceptItemCreateDto itemCreateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = ADCConceptMapping
                .ItemCreateDtoToADCConcept(itemCreateDto);
            var itemDto = ADCConceptMapping
                .ADCConceptToItemDetailDto(await _service.AddAsync(item));
            var response = new ApiResponse<ADCConceptItemDetailDto>(itemDto);

            return Ok(response);
        } // PostADCConcept

        [HttpPut]
        [ResponseType(typeof(ApiResponse<ADCConceptItemDetailDto>))]
        public async Task<IHttpActionResult> PutADCConcept(Guid id, [FromBody] ADCConceptItemUpdateDto itemUpdateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));
            
            if (id != itemUpdateDto.ID)
                throw new BusinessException("ID mismatch");
            
            var item = ADCConceptMapping
                .ItemUpdateDtoToADCConcept(itemUpdateDto);
            var itemDto = ADCConceptMapping
                .ADCConceptToItemDetailDto(await _service.UpdateAsync(item));
            var response = new ApiResponse<ADCConceptItemDetailDto>(itemDto);

            return Ok(response);
        } // PutADCConcept

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteADCConcept(Guid id, [FromBody] ADCConceptItemDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = ADCConceptMapping.ItemDeleteDtoToADCConcept(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteADCConcept
    }
}
