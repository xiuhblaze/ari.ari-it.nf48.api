using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;
using Arysoft.ARI.NF48.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        // GET: api/RiskLevels
        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<CalendarBlock>>))]
        public IHttpActionResult GetCalendarBlocks([FromUri] CalendarBlockQueryFilters filters)
        {
            var items = _calendarBlockService.Gets(filters);
            var itemsDto = ...
        } // GetCalendarBlocks
    }
}