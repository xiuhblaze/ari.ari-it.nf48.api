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
    public class NSSCCategoriesController : ApiController
    {
        private readonly NSSCCategoryService _service;

        // CONSTRUCTOR

        public NSSCCategoriesController()
        { 
            _service = new NSSCCategoryService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<NSSCCategoryItemListDto>>))]
        public IHttpActionResult GetNSSCCategories([FromUri] NSSCCategoryQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = NSSCCategoryMapping.NSSCCategoryToListDto(items);
            var response = new ApiResponse<IEnumerable<NSSCCategoryItemListDto>>(itemsDto)
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
        } // GetNSSCCategories

        // GET: api/nssccategories/5
        [ResponseType(typeof(ApiResponse<NSSCCategoryItemDetailDto>))]
        public async Task<IHttpActionResult> GetNSSCCategory(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = NSSCCategoryMapping.NSSCCategoryToItemDetailDto(item);
            var response = new ApiResponse<NSSCCategoryItemDetailDto>(itemDto);

            return Ok(response);
        } // GetNSSCCategory

        // POST: api/nssccategories
        [HttpPost]
        [ResponseType(typeof(ApiResponse<NSSCCategoryItemDetailDto>))]
        public async Task<IHttpActionResult> PostNSSCCategory([FromBody] NSSCCategoryPostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = NSSCCategoryMapping.ItemAddDtoToNSSCCategory(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = NSSCCategoryMapping.NSSCCategoryToItemDetailDto(item);
            var response = new ApiResponse<NSSCCategoryItemDetailDto>(itemDto);

            return Ok(response);
        } // PostNSSCCategory

        // PUT: api/nssccategories/5
        [HttpPut]
        [ResponseType(typeof(ApiResponse<NSSCCategoryItemDetailDto>))]
        public async Task<IHttpActionResult> PutNSSCCategory(Guid id, [FromBody] NSSCCategoryPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NSSCCategoryMapping.ItemEditDtoToNSSCCategory(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = NSSCCategoryMapping.NSSCCategoryToItemDetailDto(item);
            var response = new ApiResponse<NSSCCategoryItemDetailDto>(itemDto);

            return Ok(response);
        } // PutNSSCCategory

        // DELETE: api/NSSCCategories/5
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteNSSCCategory(Guid id, [FromBody] NSSCCategoryDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NSSCCategoryMapping.ItemDeleteDtoToNSSCCategory(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteNSSCCategory
    }
}
