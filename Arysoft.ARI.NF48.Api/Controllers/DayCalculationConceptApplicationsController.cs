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
    public class DayCalculationConceptApplicationsController : ApiController
    {
        private DayCalculationConceptApplicationService _service;

        // CONSTRUCTOR

        public DayCalculationConceptApplicationsController()
        {
            _service = new DayCalculationConceptApplicationService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<DayCalculationConceptApplication>>))]
        public IHttpActionResult GetDayCalculationConceptApplications([FromUri] DayCalculationConceptApplicationQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = DayCalculationConceptApplicationMapping.DayCalculationConceptApplicationToListDto(items);
            var response = new ApiResponse<IEnumerable<DayCalculationConceptApplicationItemListDto>>(itemsDto)
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
        } // GetDayCalculationConceptApplications
                
        [HttpGet]
        [ResponseType(typeof(ApiResponse<DayCalculationConceptApplicationItemDetailDto>))]
        public async Task<IHttpActionResult> GetDayCalculationConceptApplication(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = DayCalculationConceptApplicationMapping.DayCalculationConceptApplicationToItemDetailDto(item);
            var response = new ApiResponse<DayCalculationConceptApplicationItemDetailDto>(itemDto);

            return Ok(response);
        } // GetDayCalculationConceptApplication

        // POST: api/DayCalculationConceptApplication
        [HttpPost]
        [ResponseType(typeof(ApiResponse<DayCalculationConceptApplicationItemDetailDto>))]
        public async Task<IHttpActionResult> PostDayCalculationConceptApplication([FromBody] DayCalculationConceptApplicationPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = DayCalculationConceptApplicationMapping.ItemAddDtoToDayCalculationConceptApplication(itemAddDto);
            item = await _service.AddAsync(item);
            var itemDto = DayCalculationConceptApplicationMapping.DayCalculationConceptApplicationToItemDetailDto(item);
            var response = new ApiResponse<DayCalculationConceptApplicationItemDetailDto>(itemDto);

            return Ok(response);
        } // PostDayCalculationConceptApplication

        [HttpPut]
        [ResponseType(typeof(ApiResponse<DayCalculationConceptApplicationItemDetailDto>))]
        public async Task<IHttpActionResult> PutDayCalculationConceptApplication(Guid id, [FromBody] DayCalculationConceptApplicationPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = DayCalculationConceptApplicationMapping.ItemEditDtoToDayCalculationConceptApplication(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = DayCalculationConceptApplicationMapping.DayCalculationConceptApplicationToItemDetailDto(item);
            var response = new ApiResponse<DayCalculationConceptApplicationItemDetailDto>(itemDto);

            return Ok(response);
        } // PutDayCalculationConceptApplication

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteDayCalculationConceptApplication(Guid id, [FromBody] DayCalculationConceptApplicationDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = DayCalculationConceptApplicationMapping.ItemDeleteToDayCalculationConceptApplication(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteDayCalculationConceptApplication
    }
}