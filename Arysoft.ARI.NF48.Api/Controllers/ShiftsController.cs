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
    public class ShiftsController : ApiController
    {
        private ShiftService _shiftService;

        // CONSTRUCTOR

        public ShiftsController()
        { 
            _shiftService = new ShiftService();
        }

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<ShiftItemListDto>>))]
        public IHttpActionResult GetShifts([FromUri] ShiftQueryFilters filters)
        {

            var items = _shiftService.Gets(filters);
            var itemsDto = ShiftMapping.ShiftsToListDto(items);
            var response = new ApiResponse<IEnumerable<ShiftItemListDto>>(itemsDto)
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
        } // GetShifts

        [ResponseType(typeof(ApiResponse<ShiftItemDetailDto>))]
        public async Task<IHttpActionResult> GetShift(Guid id)
        {
            var item = await _shiftService.GetAsync(id)
                ?? throw new BusinessException("Item not found");

            var itemDto = ShiftMapping.ShiftToItemDetailDto(item);
            var response = new ApiResponse<ShiftItemDetailDto>(itemDto);

            return Ok(response);
        } // GetShift

        // POST: api/Shift
        [ResponseType(typeof(ApiResponse<ShiftItemDetailDto>))]
        public async Task<IHttpActionResult> PostShift([FromBody] ShiftPostDto shiftDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var itemToAdd = ShiftMapping.ItemAddDtoToShift(shiftDto);
            var item = await _shiftService.AddAsync(itemToAdd);
            var itemDto = ShiftMapping.ShiftToItemDetailDto(item);
            var response = new ApiResponse<ShiftItemDetailDto>(itemDto);

            return Ok(response);
        } // PostShift

        // PUT: api/Shift/5
        [ResponseType(typeof(ApiResponse<ShiftItemDetailDto>))]
        public async Task<IHttpActionResult> PutShift(Guid id, [FromBody] ShiftPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = ShiftMapping.ItemEditDtoToShift(itemEditDto);
            item = await _shiftService.UpdateAsync(item);
            var itemDto = ShiftMapping.ShiftToItemDetailDto(item);
            var response = new ApiResponse<ShiftItemDetailDto>(itemDto);

            return Ok(response);
        } // PutShift

        // DELETE: api/Shift/5
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteShift(Guid id, [FromBody] ShiftDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid) 
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID) 
                throw new BusinessException("ID mismatch");

            var item = ShiftMapping.ItemDeleteDtoToShift(itemDeleteDto);
            await _shiftService.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteShift

    }
}
