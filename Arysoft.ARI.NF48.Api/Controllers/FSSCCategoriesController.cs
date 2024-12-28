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
    public class FSSCCategoriesController : ApiController
    {
        private readonly FSSCCategoryService _service;

        // CONSTRUCTOR

        public FSSCCategoriesController()
        { 
            _service = new FSSCCategoryService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<FSSCCategoryItemListDto>>))]
        public IHttpActionResult GetFSSCCategories([FromUri] FSSCCategoryQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = FSSCCategoryMapping.FSSCCategoryToListDto(items);
            var response = new ApiResponse<IEnumerable<FSSCCategoryItemListDto>>(itemsDto)
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
        } // GetFSSCCategories

        // GET: api/nssccategories/5
        [ResponseType(typeof(ApiResponse<FSSCCategoryItemDetailDto>))]
        public async Task<IHttpActionResult> GetFSSCCategory(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = FSSCCategoryMapping.FSSCCategoryToItemDetailDto(item);
            var response = new ApiResponse<FSSCCategoryItemDetailDto>(itemDto);

            return Ok(response);
        } // GetFSSCCategory

        // POST: api/nssccategories
        [HttpPost]
        [ResponseType(typeof(ApiResponse<FSSCCategoryItemDetailDto>))]
        public async Task<IHttpActionResult> PostFSSCCategory([FromBody] FSSCCategoryPostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = FSSCCategoryMapping.ItemAddDtoToFSSCCategory(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = FSSCCategoryMapping.FSSCCategoryToItemDetailDto(item);
            var response = new ApiResponse<FSSCCategoryItemDetailDto>(itemDto);

            return Ok(response);
        } // PostFSSCCategory

        // PUT: api/nssccategories/5
        [HttpPut]
        [ResponseType(typeof(ApiResponse<FSSCCategoryItemDetailDto>))]
        public async Task<IHttpActionResult> PutFSSCCategory(Guid id, [FromBody] FSSCCategoryPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = FSSCCategoryMapping.ItemEditDtoToFSSCCategory(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = FSSCCategoryMapping.FSSCCategoryToItemDetailDto(item);
            var response = new ApiResponse<FSSCCategoryItemDetailDto>(itemDto);

            return Ok(response);
        } // PutFSSCCategory

        // DELETE: api/FSSCCategories/5
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteFSSCCategory(Guid id, [FromBody] FSSCCategoryDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = FSSCCategoryMapping.ItemDeleteDtoToFSSCCategory(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteFSSCCategory
    }
}
