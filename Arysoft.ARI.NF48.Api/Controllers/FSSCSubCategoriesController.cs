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
    public class FSSCSubCategoriesController : ApiController
    {
        private readonly FSSCSubCategoryService _service;

        // CONSTRUCTOR

        public FSSCSubCategoriesController()
        {
            _service = new FSSCSubCategoryService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<FSSCSubCategoryItemListDto>>))]
        public IHttpActionResult GetFSSCSubCategories([FromUri] FSSCSubCategoryQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = FSSCSubCategoryMapping.FSSCSubCategoryToListDto(items);
            var response = new ApiResponse<IEnumerable<FSSCSubCategoryItemListDto>>(itemsDto)
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
        } // GetFSSCSubCategories

        // GET: api/nssccategories/5
        [ResponseType(typeof(ApiResponse<FSSCSubCategoryItemDetailDto>))]
        public async Task<IHttpActionResult> GetFSSCSubCategory(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = FSSCSubCategoryMapping.FSSCSubCategoryToItemDetailDto(item);
            var response = new ApiResponse<FSSCSubCategoryItemDetailDto>(itemDto);

            return Ok(response);
        } // GetFSSCSubCategory

        // POST: api/nssccategories
        [HttpPost]
        [ResponseType(typeof(ApiResponse<FSSCSubCategoryItemDetailDto>))]
        public async Task<IHttpActionResult> PostFSSCSubCategory([FromBody] FSSCSubCategoryPostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = FSSCSubCategoryMapping.ItemAddDtoToFSSCSubCategory(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = FSSCSubCategoryMapping.FSSCSubCategoryToItemDetailDto(item);
            var response = new ApiResponse<FSSCSubCategoryItemDetailDto>(itemDto);

            return Ok(response);
        } // PostFSSCSubCategory

        // PUT: api/nssccategories/5
        [HttpPut]
        [ResponseType(typeof(ApiResponse<FSSCSubCategoryItemDetailDto>))]
        public async Task<IHttpActionResult> PutFSSCSubCategory(Guid id, [FromBody] FSSCSubCategoryPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = FSSCSubCategoryMapping.ItemEditDtoToFSSCSubCategory(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = FSSCSubCategoryMapping.FSSCSubCategoryToItemDetailDto(item);
            var response = new ApiResponse<FSSCSubCategoryItemDetailDto>(itemDto);

            return Ok(response);
        } // PutFSSCSubCategory

        // DELETE: api/FSSCSubCategories/5
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteFSSCSubCategory(Guid id, [FromBody] FSSCSubCategoryDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = FSSCSubCategoryMapping.ItemDeleteDtoToFSSCSubCategory(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteFSSCSubCategory
    }
}
