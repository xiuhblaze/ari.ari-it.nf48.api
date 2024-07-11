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
    public class Categories22KController : ApiController
    {
        private Category22KService _category22KService;

        // CONSTRUCTOR

        public Categories22KController()
        { 
            _category22KService = new Category22KService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<Category22KItemListDto>>))]
        public IHttpActionResult GetCategories22K([FromUri] Category22KQueryFilters filters)
        { 
            var items = _category22KService.Gets(filters);
            var itemsDto = Category22KMapping.Category22KToListDto(items);
            var response = new ApiResponse<IEnumerable<Category22KItemListDto>>(itemsDto)
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
        } // GetCategories22K

        [ResponseType(typeof(ApiResponse<Category22KItemDetailDto>))]
        public async Task<IHttpActionResult> GetCategory22K(Guid id)
        {
            var item = await _category22KService.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = Category22KMapping.Category22KToItemDetailDto(item);
            var response = new ApiResponse<Category22KItemDetailDto>(itemDto);

            return Ok(response);
        } // GetCategory22K

        // POST: api/Category22K
        [ResponseType(typeof(ApiResponse<Category22KItemDetailDto>))]
        public async Task<IHttpActionResult> PostCategory22K([FromBody] Category22KPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var itemToAdd = Category22KMapping.ItemAddDtoToCategory22K(itemAddDto);
            var item = await _category22KService.AddAsync(itemToAdd);
            var itemDto = Category22KMapping.Category22KToItemDetailDto(item);
            var response = new ApiResponse<Category22KItemDetailDto>(itemDto);

            return Ok(response);
        } // PostCategory22K

        // PUT: api/Category22K/5
        [ResponseType(typeof(ApiResponse<Category22KItemDetailDto>))]
        public async Task<IHttpActionResult> PutCategory22K(Guid id, [FromBody] Category22KPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var itemToEdit = Category22KMapping.ItemEditDtoToCategory22K(itemEditDto);
            var item = await _category22KService.UpdateAsync(itemToEdit);
            var itemDto = Category22KMapping.Category22KToItemDetailDto(item);
            var response = new ApiResponse<Category22KItemDetailDto>(itemDto);

            return Ok(response);
        } // PutCategory22K

        // DELETE: api/Category22K/5
        public async Task<IHttpActionResult> DeleteCategory22K(Guid id, [FromBody] Category22KDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = Category22KMapping.ItemDeleteDtoToCategory22K(itemDelDto);
            await _category22KService.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteCategory22K
    }
}
