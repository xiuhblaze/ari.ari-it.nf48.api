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
    public class NSSCSubCategoriesController : ApiController
    {
        private readonly NSSCSubCategoryService _service;

        // CONSTRUCTOR

        public NSSCSubCategoriesController()
        {
            _service = new NSSCSubCategoryService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<NSSCSubCategoryItemListDto>>))]
        public IHttpActionResult GetNSSCSubCategories([FromUri] NSSCSubCategoryQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = NSSCSubCategoryMapping.NSSCSubCategoryToListDto(items);
            var response = new ApiResponse<IEnumerable<NSSCSubCategoryItemListDto>>(itemsDto)
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
        } // GetNSSCSubCategories

        // GET: api/nssccategories/5
        [ResponseType(typeof(ApiResponse<NSSCSubCategoryItemDetailDto>))]
        public async Task<IHttpActionResult> GetNSSCSubCategory(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = NSSCSubCategoryMapping.NSSCSubCategoryToItemDetailDto(item);
            var response = new ApiResponse<NSSCSubCategoryItemDetailDto>(itemDto);

            return Ok(response);
        } // GetNSSCSubCategory

        // POST: api/nssccategories
        [HttpPost]
        [ResponseType(typeof(ApiResponse<NSSCSubCategoryItemDetailDto>))]
        public async Task<IHttpActionResult> PostNSSCSubCategory([FromBody] NSSCSubCategoryPostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = NSSCSubCategoryMapping.ItemAddDtoToNSSCSubCategory(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = NSSCSubCategoryMapping.NSSCSubCategoryToItemDetailDto(item);
            var response = new ApiResponse<NSSCSubCategoryItemDetailDto>(itemDto);

            return Ok(response);
        } // PostNSSCSubCategory

        // PUT: api/nssccategories/5
        [HttpPut]
        [ResponseType(typeof(ApiResponse<NSSCSubCategoryItemDetailDto>))]
        public async Task<IHttpActionResult> PutNSSCSubCategory(Guid id, [FromBody] NSSCSubCategoryPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NSSCSubCategoryMapping.ItemEditDtoToNSSCSubCategory(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = NSSCSubCategoryMapping.NSSCSubCategoryToItemDetailDto(item);
            var response = new ApiResponse<NSSCSubCategoryItemDetailDto>(itemDto);

            return Ok(response);
        } // PutNSSCSubCategory

        // DELETE: api/NSSCSubCategories/5
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteNSSCSubCategory(Guid id, [FromBody] NSSCSubCategoryDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NSSCSubCategoryMapping.ItemDeleteDtoToNSSCSubCategory(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteNSSCSubCategory
    }
}
