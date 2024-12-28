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
    public class FSSCActivitiesController : ApiController
    {
        private readonly FSSCActivityService _service;

        // CONSTRUCTOR

        public FSSCActivitiesController()
        { 
            _service = new FSSCActivityService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<FSSCActivityItemListDto>>))]
        public IHttpActionResult GetFSSCActivities([FromUri] FSSCActivityQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = FSSCActivityMapping.FSSCActivityToListDto(items);
            var response = new ApiResponse<IEnumerable<FSSCActivityItemListDto>>(itemsDto)
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
        } // GetFSSCActivities

        // GET: api/FSSCActivity/5
        [ResponseType(typeof(ApiResponse<FSSCActivityItemDetailDto>))]
        public async Task<IHttpActionResult> GetFSSCActivity(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = FSSCActivityMapping.FSSCActivityToItemDetailDto(item);
            var response = new ApiResponse<FSSCActivityItemDetailDto>(itemDto);

            return Ok(response);
        } // GetFSSCActivity

        // POST: api/FSSCActivity
        [HttpPost]
        [ResponseType(typeof(ApiResponse<FSSCActivityItemDetailDto>))]
        public async Task<IHttpActionResult> PostFSSCActivity([FromBody] FSSCActivityPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = FSSCActivityMapping.ItemAddDtoToFSSCActivity(itemAddDto);
            item = await _service.AddAsync(item);
            var itemDto = FSSCActivityMapping.FSSCActivityToItemDetailDto(item);
            var response = new ApiResponse<FSSCActivityItemDetailDto>(itemDto);

            return Ok(response);
        } // PostFSSCActivity

        // PUT: api/FSSCActivity/5
        [HttpPut]
        [ResponseType(typeof(ApiResponse<FSSCActivityItemDetailDto>))]
        public async Task<IHttpActionResult> PutFSSCActivity(Guid id, [FromBody] FSSCActivityPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = FSSCActivityMapping.ItemEditDtoToFSSCActivity(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = FSSCActivityMapping.FSSCActivityToItemDetailDto(item);
            var response = new ApiResponse<FSSCActivityItemDetailDto>(itemDto);

            return Ok(response);
        } // PutFSSCActivity

        // DELETE: api/FSSCActivity/5
        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteFSSCActivity(Guid id, [FromBody] FSSCActivityDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = FSSCActivityMapping.ItemDeleteDtoToFSSCActivity(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteFSSCActivity
    }
}
