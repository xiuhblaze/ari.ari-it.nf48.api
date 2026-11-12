using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Mappings;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;
using Arysoft.ARI.NF48.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Management;

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
            var itemsDto = await CalendarBlockMapping.CalendarBlockToListAsync(items);
            var response = new ApiResponse<IEnumerable<CalendarBlockItemListDto>>(itemsDto)
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
        [ResponseType(typeof(ApiResponse<CalendarBlockItemDetailDto>))]
        public async Task<IHttpActionResult> GetCalendarBlock(Guid id)
        {
            var item = await _calendarBlockService.GetAsync(id)
                ?? throw new BusinessException("Calendar block item not found", System.Net.HttpStatusCode.NotFound);

        } // GetCalendarBlock
    }
}