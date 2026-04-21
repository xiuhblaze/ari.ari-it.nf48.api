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
    public class RiskLevelsController : ApiController
    {
        private readonly RiskLevelService _riskLevelService;

        // CONSTRUCTOR

        public RiskLevelsController()
        {
            _riskLevelService = new RiskLevelService();
        }

        // ENDPOINT

        // GET: api/RiskLevels
        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<RiskLevel>>))]
        public IHttpActionResult GetRiskLevels([FromUri]RiskLevelQueryFilters filters)
        {
            var items = _riskLevelService.Gets(filters);
            var itemsDto = RiskLevelMapping.RiskLevelToListDto(items);
            var response = new ApiResponse<IEnumerable<RiskLevelItemListDto>>(itemsDto)
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
        } // GetRiskLevels

        [HttpGet]
        [ResponseType(typeof(ApiResponse<RiskLevel>))]
        public async Task<IHttpActionResult> GetRiskLevel(Guid id)
        {
            var item = await _riskLevelService.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = RiskLevelMapping.RiskLevelToItemDetailDto(item);
            var response = new ApiResponse<RiskLevelItemDetailDto>(itemDto);

            return Ok(response);
        } // GetRiskLevel

        [HttpPost]
        [ResponseType(typeof(ApiResponse<RiskLevelItemDetailDto>))]
        public async Task<IHttpActionResult> PostRiskLevel([FromBody] RiskLevelCreateDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = RiskLevelMapping
                .ItemCreateDtoToRiskLevel(itemDto);
            item = await _riskLevelService.CreateAsync(item);
            var createdItemDto = RiskLevelMapping.RiskLevelToItemDetailDto(item);
            var response = new ApiResponse<RiskLevelItemDetailDto>(createdItemDto);

            return Ok(response);
        } // PostRiskLevel

        [HttpPut]
        [ResponseType(typeof(ApiResponse<RiskLevelItemDetailDto>))]
        public async Task<IHttpActionResult> PutRiskLevel(Guid id, [FromBody] RiskLevelUpdateDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.ID)
                throw new BusinessException("ID mismatch");

            var item = RiskLevelMapping
                .ItemUpdateDtoToRiskLevel(itemDto);
            item = await _riskLevelService.UpdateAsync(item);
            var updatedItemDto = RiskLevelMapping
                .RiskLevelToItemDetailDto(item);
            var response = new ApiResponse<RiskLevelItemDetailDto>(updatedItemDto);

            return Ok(response);
        } // PutRiskLevel

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteRiskLevel(Guid id, [FromBody] RiskLevelDeleteDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.ID)
                throw new BusinessException("ID mismatch");

            var item = RiskLevelMapping
                .ItemDeleteDtoToRiskLevel(itemDto);
            await _riskLevelService.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteRiskLevel

    } // RiskLevelsController
}