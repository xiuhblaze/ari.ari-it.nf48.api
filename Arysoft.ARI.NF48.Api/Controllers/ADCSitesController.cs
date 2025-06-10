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
    public class ADCSitesController : ApiController
    {
        private readonly ADCSiteService _service;

        // CONSTRUCTOR
        
        public ADCSitesController()
        {
            _service = new ADCSiteService();
        }

        // END POINTS
        
        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<ADCSiteItemListDto>>))]
        public IHttpActionResult GetADCSites([FromUri] ADCSiteQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = ADCSiteMapping.ADCSiteToListDto(items);
            var response = new ApiResponse<IEnumerable<ADCSiteItemListDto>>(itemsDto)
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
        } // GetADCSites

        [HttpGet]
        [ResponseType(typeof(ApiResponse<ADCSiteItemDetailDto>))]
        public async Task<IHttpActionResult> GetADCSite(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = ADCSiteMapping
                .ADCSiteToItemDetailDto(item);
            var response = new ApiResponse<ADCSiteItemDetailDto>(itemDto);

            return Ok(response);
        } // GetADCSite

        [HttpPost]
        [ResponseType(typeof(ApiResponse<ADCSiteItemDetailDto>))]
        public async Task<IHttpActionResult> PostADCSite([FromBody] ADCSiteItemCreateDto itemCreateDto)
        { 
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = ADCSiteMapping
                .ItemCreateDtoToADCSite(itemCreateDto);
            var itemDto = ADCSiteMapping
                .ADCSiteToItemDetailDto(await _service.AddAsync(item));
            var response = new ApiResponse<ADCSiteItemDetailDto>(itemDto);

            return Ok(response);
        } // PostADCSite

        [HttpPut]
        [ResponseType(typeof(ApiResponse<ADCSiteItemDetailDto>))]
        public async Task<IHttpActionResult> PutADCSite(Guid id, [FromBody] ADCSiteItemUpdateDto itemUpdateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemUpdateDto.ID)
                throw new BusinessException("ID mismatch");

            var item = ADCSiteMapping
                .ItemUpdateDtoToADCSite(itemUpdateDto);
            var itemDto = ADCSiteMapping
                .ADCSiteToItemDetailDto(await _service.UpdateAsync(item));
            var response = new ApiResponse<ADCSiteItemDetailDto>(itemDto);

            return Ok(response);
        } // PutADCSite

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<ADCSiteItemDetailDto>))]
        public async Task<IHttpActionResult> DeleteADCSite(Guid id, [FromBody] ADCSiteItemDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = ADCSiteMapping.ItemDeleteDtoToADCSite(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteADCSite
    }
}
