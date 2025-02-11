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
    public class FSSCAuditorActivitiesController : ApiController
    {
        private readonly FSSCAuditorActivityService _service;

        // CONSTRUCTOR

        public FSSCAuditorActivitiesController()
        {
            _service = new FSSCAuditorActivityService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<FSSCAuditorActivityItemListDto>>))]
        public IHttpActionResult GetFSSCAuditorActivities([FromUri] FSSCAuditorActivityQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = FSSCAuditorActivityMapping.FSSCAuditorActivityToListDto(items);
            var response = new ApiResponse<IEnumerable<FSSCAuditorActivityItemListDto>>(itemsDto)
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
        } // GetFSSCAuditorActivities

        [ResponseType(typeof(ApiResponse<FSSCAuditorActivityItemDetailDto>))]
        public async Task<IHttpActionResult> GetFSSCAuditorActivity(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = FSSCAuditorActivityMapping.FSSCAuditorActivityToItemDetailDto(item);
            var response = new ApiResponse<FSSCAuditorActivityItemDetailDto>(itemDto);

            return Ok(response);
        } // GetFSSCAuditorActivity

        [ResponseType(typeof(ApiResponse<FSSCAuditorActivityItemDetailDto>))]
        public async Task<IHttpActionResult> PostFSSCAuditorActivity([FromBody] FSSCAuditorActivityPostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = FSSCAuditorActivityMapping.ItemAddDtoToFSSCAuditorActivity(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = FSSCAuditorActivityMapping.FSSCAuditorActivityToItemDetailDto(item);
            var response = new ApiResponse<FSSCAuditorActivityItemDetailDto>(itemDto);

            return Ok(response);
        } // PostFSSCAuditorActivity

        [ResponseType(typeof(ApiResponse<FSSCAuditorActivityItemDetailDto>))]
        public async Task<IHttpActionResult> PutFSSCAuditorActivity(Guid id, [FromBody] FSSCAuditorActivityPutDto itemPutDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemPutDto.ID)
                throw new BusinessException("ID mismatch");

            var item = FSSCAuditorActivityMapping.ItemEditDtoToFSSCAuditorActivity(itemPutDto);
            item = await _service.UpdateAsync(item);
            var itemDto = FSSCAuditorActivityMapping.FSSCAuditorActivityToItemDetailDto(item);
            var response = new ApiResponse<FSSCAuditorActivityItemDetailDto>(itemDto);

            return Ok(response);
        } // PutFSSCAuditorActivity

        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteFSSCAuditorActivity(Guid id, [FromBody] FSSCAuditorActivityDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = FSSCAuditorActivityMapping.ItemDeleteDtoToFSSCAuditorActivity(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteFSSCAuditorActivity

    }
}
