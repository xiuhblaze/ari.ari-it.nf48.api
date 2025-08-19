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
    public class ADCConceptValuesController : ApiController
    {
        private readonly ADCConceptValueService _service;

        // CONSTRUCTOR

        public ADCConceptValuesController()
        {
            _service = new ADCConceptValueService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<ADCConceptValueItemListDto>>))]
        public IHttpActionResult GetADCConceptValues([FromUri] ADCConceptValueQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = ADCConceptValueMapping.ADCConceptValueToListDto(items);
            var response = new ApiResponse<IEnumerable<ADCConceptValueItemListDto>>(itemsDto)
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
        } // GetADCConceptValues

        [HttpGet]
        [ResponseType(typeof(ApiResponse<ADCConceptValueItemDetailDto>))]
        public async Task<IHttpActionResult> GetADCConceptValue(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = ADCConceptValueMapping
                .ADCConceptValueToItemDetailDto(item);
            var response = new ApiResponse<ADCConceptValueItemDetailDto>(itemDto);

            return Ok(response);
        } // GetADCConceptValue

        [HttpPost]
        [ResponseType(typeof(ApiResponse<ADCConceptValueItemDetailDto>))]
        public async Task<IHttpActionResult> PostADCConceptValue(ADCConceptValueItemCreateDto itemCreateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = ADCConceptValueMapping
                .ItemCreateDtoToADCConceptValue(itemCreateDto);            
            var itemDto = ADCConceptValueMapping
                .ADCConceptValueToItemDetailDto(await _service.CreateAsync(item));
            var response = new ApiResponse<ADCConceptValueItemDetailDto>(itemDto);

            return Ok(response);
        } // PostADCConceptValue

        [HttpPut]
        [ResponseType(typeof(ApiResponse<ADCConceptValueItemDetailDto>))]
        public async Task<IHttpActionResult> PutADCConceptValue(Guid id, [FromBody] ADCConceptValueItemUpdateDto itemUpdateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemUpdateDto.ID)
                throw new BusinessException("ID mismatch");

            var item = ADCConceptValueMapping
                .ItemUpdateDtoToADCConceptValue(itemUpdateDto);
            var itemDto = ADCConceptValueMapping
                .ADCConceptValueToItemDetailDto(await _service.UpdateAsync(item));
            var response = new ApiResponse<ADCConceptValueItemDetailDto>(itemDto);

            return Ok(response);
        } // PutADCConceptValue

        [HttpPut]
        [Route("api/ADCConceptValues/list")]
        [ResponseType(typeof(ApiResponse<ADCConceptValueItemListDto>))]
        public async Task<IHttpActionResult> PutADCConceptValueList([FromBody] ADCConceptValueListUpdateDto itemsUpdateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var items = ADCConceptValueMapping
                .UpdateListDtoToADCConceptValues(itemsUpdateDto);
            var resultItems = await _service.UpdateListAsync(items.ToList());
            var itemsDto = ADCConceptValueMapping
                .ADCConceptValueToListDto(resultItems);

            var response = new ApiResponse<IEnumerable<ADCConceptValueItemListDto>>(itemsDto);

            return Ok(response);
        } // PutADCConceptValueList

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteADCConceptValue(Guid id, [FromBody] ADCConceptValueItemDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = ADCConceptValueMapping.ItemDeleteDtoToADCConceptValue(itemDeleteDto);            
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteADCConceptValue
    }
}
