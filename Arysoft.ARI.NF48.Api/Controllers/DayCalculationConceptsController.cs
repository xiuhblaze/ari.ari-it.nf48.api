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
    public class DayCalculationConceptsController : ApiController
    {
        private DayCalculationConceptService _service;

        // CONSTRUCTOR

        public DayCalculationConceptsController()
        {
            _service = new DayCalculationConceptService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<DayCalculationConcept>>))]
        public IHttpActionResult GetDayCalculationConcepts([FromUri] DayCalculationConceptQueryFilters filters)
        {
            var items = _service.Gets(filters);
            // var itemsDto = DayCa...
        } // GetDayCalculationConcepts
    }
}