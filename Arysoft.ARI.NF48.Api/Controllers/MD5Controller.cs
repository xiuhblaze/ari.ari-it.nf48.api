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
    public class MD5Controller : ApiController
    {
        private readonly MD5Service _service;

        // CONSTRUCTOR

        public MD5Controller()
        {
            _service = new MD5Service();
        }

        // ENDPOINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<MD5ItemListDto>>))]
        public IHttpActionResult GetMD5s([FromUri] MD5QueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = MD5Mapping.MD5ToListDto(items);
            var response = new ApiResponse<IEnumerable<MD5ItemListDto>>(itemsDto)
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
        } // GetMD5s

        [HttpGet]
        [ResponseType(typeof(ApiResponse<MD5ItemDetailDto>))]
        public async Task<IHttpActionResult> GetMD5(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = MD5Mapping.MD5ToItemDetailDto(item);
            var response = new ApiResponse<MD5ItemDetailDto>(itemDto);

            return Ok(response);
        } // GetMD5

        [HttpPost]
        [ResponseType(typeof(ApiResponse<MD5ItemDetailDto>))]
        public async Task<IHttpActionResult> PostMD5([FromBody] MD5ItemCreateDto itemCreateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = MD5Mapping
                .ItemCreateDtoToMD5(itemCreateDto);
            var itemDto = MD5Mapping
                .MD5ToItemDetailDto(await _service.CreateAsync(item));            
            var response = new ApiResponse<MD5ItemDetailDto>(itemDto);

            return Ok(response);
        } // PostMD5

        [HttpPut]
        [ResponseType(typeof(ApiResponse<MD5ItemDetailDto>))]
        public async Task<IHttpActionResult> PutMD5(Guid id, [FromBody] MD5ItemUpdateDto itemUpdateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemUpdateDto.ID)
                throw new BusinessException("ID mismatch");

            var item = MD5Mapping
                .ItemUpdateDtoToMD5(itemUpdateDto);
            var itemDto = MD5Mapping
                .MD5ToItemDetailDto(await _service.UpdateAsync(item));
            var response = new ApiResponse<MD5ItemDetailDto>(itemDto);

            return Ok(response);
        } // PutMD5

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteMD5(Guid id, [FromBody] MD5ItemDeleteDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.ID)
                throw new BusinessException("ID mismatch");

            var item = MD5Mapping
                .ItemDeleteDtoToMD5(itemDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteMD5
    }
}
