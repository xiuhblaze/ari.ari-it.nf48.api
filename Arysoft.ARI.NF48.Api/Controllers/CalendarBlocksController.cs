using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Mappings;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;
using Arysoft.ARI.NF48.Api.Services;
using Arysoft.ARI.NF48.Api.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class CalendarBlocksController : ApiController
    {
        private readonly CalendarBlockService _calendarBlockService;

        // CONSTRUCTOR

        public CalendarBlocksController()
        {
            _calendarBlockService = new CalendarBlockService();
        }

        // ENDPOINTS

        // GET: api/CalendarBlocks?params=values...
        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<CalendarBlock>>))]
        public async Task<IHttpActionResult> GetCalendarBlocks([FromUri] CalendarBlockQueryFilters filters)
        {
            var items = _calendarBlockService.Gets(filters);
            var ownerNames = await _calendarBlockService.GetOwnerNamesAsync(items);
            var itemsDto = items
                .Select(i =>
                {
                    ownerNames.TryGetValue(i.ID, out var ownerName);
                    return i.ToListDto(ownerName ?? string.Empty);
                })
                .ToList();
            var response = new ApiResponse<IEnumerable<CalendarBlockListDto>>(itemsDto)
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
        } // GetCalendarBlocks

        // GET: api/CalendarBlocks/{id}
        [HttpGet]
        [ResponseType(typeof(ApiResponse<CalendarBlockDetailDto>))]
        public async Task<IHttpActionResult> GetCalendarBlock(Guid id)
        {
            var item = await _calendarBlockService.GetAsync(id)
                ?? throw new BusinessException("Item not found", System.Net.HttpStatusCode.NotFound);
            var itemDto = item.ToDetailDto();
            var response = new ApiResponse<CalendarBlockDetailDto>(itemDto);

            return Ok(response);
        } // GetCalendarBlock

        // POST: api/CalendarBlock
        [HttpPost]
        [ResponseType(typeof(ApiResponse<CalendarBlockDetailDto>))]
        public async Task<IHttpActionResult> PostCalendarBlock([FromBody] CalendarBlockCreateDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = itemDto.ToCalendarBlock();
            item = await _calendarBlockService.CreateAsync(item);
            var createdItemDto = item.ToDetailDto();
            var response = new ApiResponse<CalendarBlockDetailDto>(createdItemDto);

            return Ok(response);
        } // PostCalendarBlock

        [HttpPut]
        [ResponseType(typeof(ApiResponse<CalendarBlockDetailDto>))]
        public async Task<IHttpActionResult> PutCalendarBlock(Guid id, [FromBody] CalendarBlockUpdateDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.ID)
                throw new BusinessException("ID mismatch");

            var item = itemDto.ToCalendarBlock();
            item = await _calendarBlockService.UpdateAsync(item);
            var updatedItemDto = item.ToDetailDto();
            var response = new ApiResponse<CalendarBlockDetailDto>(updatedItemDto);

            return Ok(response);
        } // PutCalendarBlock

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteCalendarBlock(Guid id, [FromBody] CalendarBlockDeleteDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.ID)
                throw new BusinessException("ID mismatch");

            var item = itemDto.ToCalendarBlock();
            await _calendarBlockService.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteCalendarBlock

    } // CalendarBlockController
}