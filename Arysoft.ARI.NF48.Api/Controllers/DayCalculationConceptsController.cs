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
using System.Threading.Tasks;
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
            var itemsDto = DayCalculationConceptMapping.DayCalculationConceptToListDto(items);
            var response = new ApiResponse<IEnumerable<DayCalculationConceptItemListDto>>(itemsDto)
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
        } // GetDayCalculationConcepts

        [ResponseType(typeof(ApiResponse<DayCalculationConceptItemDetailDto>))]
        public async Task<IHttpActionResult> GetDayCalculationConcept(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = DayCalculationConceptMapping.DayCalculationConceptToItemDetailDto(item);
            var response = new ApiResponse<DayCalculationConceptItemDetailDto>(itemDto);

            return Ok(response);
        } // GetDayCalculationConcept

        // POST: api/DayCalculationConcept
        [ResponseType(typeof(ApiResponse<DayCalculationConceptItemDetailDto>))]
        public async Task<IHttpActionResult> PostDayCalculationConcept([FromBody] DayCalculationConceptPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = DayCalculationConceptMapping.ItemAddDtoToDayCalculationConcept(itemAddDto);
            item = await _service.AddAsync(item);
            var itemDto = DayCalculationConceptMapping.DayCalculationConceptToItemDetailDto(item);
            var response = new ApiResponse<DayCalculationConceptItemDetailDto>(itemDto);

            return Ok(response);
        } // PostDayCalculationConcept

        // PUT: api/DayCalculationConcept/5
        [ResponseType(typeof(ApiResponse<DayCalculationConceptItemDetailDto>))]
        public async Task<IHttpActionResult> PutDayCalculationConcept(Guid id, [FromBody] DayCalculationConceptPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = DayCalculationConceptMapping.ItemEditDtoToDayCalculationConcept(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = DayCalculationConceptMapping.DayCalculationConceptToItemDetailDto(item);
            var response = new ApiResponse<DayCalculationConceptItemDetailDto>(itemDto);

            return Ok(response);
        } // PutDayCalculationConcept

        public async Task<IHttpActionResult> DeleteDayCalculationConcept(Guid id, [FromBody] DayCalculationConceptDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = DayCalculationConceptMapping.ItemDeleteToDayCalculationconcept(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteDayCalculationConcept
    } 
}