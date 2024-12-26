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
    public class NSSCAuditorActivitiesController : ApiController
    {
        private readonly NSSCAuditorActivityService _service;

        // CONSTRUCTOR

        public NSSCAuditorActivitiesController()
        {
            _service = new NSSCAuditorActivityService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<NSSCAuditorActivityItemListDto>>))]
        public IHttpActionResult GetNSSCAuditorActivities([FromUri] NSSCAuditorActivityQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = NSSCAuditorActivityMapping.NSSCAuditorActivityToListDto(items);
            var response = new ApiResponse<IEnumerable<NSSCAuditorActivityItemListDto>>(itemsDto)
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
        } // GetNSSCAuditorActivities

        [ResponseType(typeof(ApiResponse<NSSCAuditorActivityItemDetailDto>))]
        public async Task<IHttpActionResult> GetNSSCAuditorActivity(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = NSSCAuditorActivityMapping.NSSCAuditorActivityToItemDetailDto(item);
            var response = new ApiResponse<NSSCAuditorActivityItemDetailDto>(itemDto);

            return Ok(response);
        } // GetNSSCAuditorActivity

        [ResponseType(typeof(ApiResponse<NSSCAuditorActivityItemDetailDto>))]
        public async Task<IHttpActionResult> PostNSSCAuditorActivity([FromBody] NSSCAuditorActivityPostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = NSSCAuditorActivityMapping.ItemAddDtoToNSSCAuditorActivity(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = NSSCAuditorActivityMapping.NSSCAuditorActivityToItemDetailDto(item);
            var response = new ApiResponse<NSSCAuditorActivityItemDetailDto>(itemDto);

            return Ok(response);
        } // PostNSSCAuditorActivity

        [ResponseType(typeof(ApiResponse<NSSCAuditorActivityItemDetailDto>))]
        public async Task<IHttpActionResult> PutNSSCAuditorActivity(Guid id, [FromBody] NSSCAuditorActivityPutDto itemPutDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemPutDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NSSCAuditorActivityMapping.ItemEditDtoToNSSCAuditorActivity(itemPutDto);
            item = await _service.UpdateAsync(item);
            var itemDto = NSSCAuditorActivityMapping.NSSCAuditorActivityToItemDetailDto(item);
            var response = new ApiResponse<NSSCAuditorActivityItemDetailDto>(itemDto);

            return Ok(response);
        } // PutNSSCAuditorActivity

        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteNSSCAuditorActivity(Guid id, [FromBody] NSSCAuditorActivityDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NSSCAuditorActivityMapping.ItemDeleteDtoToNSSCAuditorActivity(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteNSSCAuditorActivity

    }
}
