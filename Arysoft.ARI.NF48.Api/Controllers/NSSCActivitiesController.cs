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
    public class NSSCActivitiesController : ApiController
    {
        private readonly NSSCActivityService _service;

        // CONSTRUCTOR

        public NSSCActivitiesController()
        { 
            _service = new NSSCActivityService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<NSSCActivityItemListDto>>))]
        public IHttpActionResult GetNSSCActivities([FromUri] NSSCActivityQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = NSSCActivityMapping.NSSCActivityToListDto(items);
            var response = new ApiResponse<IEnumerable<NSSCActivityItemListDto>>(itemsDto)
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
        } // GetNSSCActivities

        // GET: api/NSSCActivity/5
        [ResponseType(typeof(ApiResponse<NSSCActivityItemDetailDto>))]
        public async Task<IHttpActionResult> GetNSSCActivity(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = NSSCActivityMapping.NSSCActivityToItemDetailDto(item);
            var response = new ApiResponse<NSSCActivityItemDetailDto>(itemDto);

            return Ok(response);
        } // GetNSSCActivity

        // POST: api/NSSCActivity
        [HttpPost]
        [ResponseType(typeof(ApiResponse<NSSCActivityItemDetailDto>))]
        public async Task<IHttpActionResult> PostNSSCActivity([FromBody] NSSCActivityPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = NSSCActivityMapping.ItemAddDtoToNSSCActivity(itemAddDto);
            item = await _service.AddAsync(item);
            var itemDto = NSSCActivityMapping.NSSCActivityToItemDetailDto(item);
            var response = new ApiResponse<NSSCActivityItemDetailDto>(itemDto);

            return Ok(response);
        } // PostNSSCActivity

        // PUT: api/NSSCActivity/5
        [HttpPut]
        [ResponseType(typeof(ApiResponse<NSSCActivityItemDetailDto>))]
        public async Task<IHttpActionResult> PutNSSCActivity(Guid id, [FromBody] NSSCActivityPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NSSCActivityMapping.ItemEditDtoToNSSCActivity(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = NSSCActivityMapping.NSSCActivityToItemDetailDto(item);
            var response = new ApiResponse<NSSCActivityItemDetailDto>(itemDto);

            return Ok(response);
        } // PutNSSCActivity

        // DELETE: api/NSSCActivity/5
        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteNSSCActivity(Guid id, [FromBody] NSSCActivityDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NSSCActivityMapping.ItemDeleteDtoToNSSCActivity(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteNSSCActivity
    }
}
