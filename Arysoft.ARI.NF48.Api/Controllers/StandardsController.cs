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
    public class StandardsController : ApiController
    {
        private readonly StandardService _standardService;

        // CONSTRUCTOR 

        public StandardsController()
        {
            _standardService = new StandardService();
        } // StandardsController

        // ENDPOINT

        // GET: api/Standards
        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<Standard>>))]
        public IHttpActionResult GetStandards([FromUri]StandardQueryFilters filters)
        {
            var items = _standardService.Gets(filters);
            var itemsDto = StandardMapping.StandardsToListDto(items);
            var response = new ApiResponse<IEnumerable<StandardItemListDto>>(itemsDto)
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
        } // GetStandards

        // GET: api/Standards/5
        [HttpGet]
        [ResponseType(typeof(ApiResponse<Standard>))]
        public async Task<IHttpActionResult> GetStandard(Guid id)
        {
            var item = await _standardService.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = StandardMapping.StandardToItemDetailDto(item);
            var response = new ApiResponse<StandardItemDetailDto>(itemDto);

            return Ok(response);
        } // GetStandard

        // POST: api/Standards
        [HttpPost]
        [ResponseType(typeof(ApiResponse<Standard>))]
        public async Task<IHttpActionResult> PostStandard([FromBody] StandardPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var itemToAdd = StandardMapping.ItemAddDtoToStandard(itemAddDto);
            var item = await _standardService.AddAsync(itemToAdd);
            var itemDto = StandardMapping.StandardToItemDetailDto(item);
            var response = new ApiResponse<StandardItemDetailDto>(itemDto);

            return Ok(response);
        } // PostStandard

        // PUT: api/Standards/5
        [HttpPut]
        [ResponseType(typeof(ApiResponse<Standard>))]
        public async Task<IHttpActionResult> PutStandard(Guid id, [FromBody] StandardPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var itemToEdit = StandardMapping.ItemEditDtoToStandard(itemEditDto);
            var item = await _standardService.UpdateAsync(itemToEdit);
            var itemDto = StandardMapping.StandardToItemDetailDto(item);
            var response = new ApiResponse<StandardItemDetailDto>(itemDto);

            return Ok(response);
        } // PutStandard

        // DELETE: api/Standards/5
        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteAsync(Guid id, [FromBody] StandardDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = StandardMapping.ItemDeleteDtoToStandard(itemDelDto);
            await _standardService.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteAsync         
    }
}