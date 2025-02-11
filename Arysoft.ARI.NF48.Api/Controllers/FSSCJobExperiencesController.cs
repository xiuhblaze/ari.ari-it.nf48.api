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
    public class FSSCJobExperiencesController : ApiController
    {
        private readonly FSSCJobExperienceService _service;

        // CONSTRUCTOR

        public FSSCJobExperiencesController()
        {
            _service = new FSSCJobExperienceService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<FSSCJobExperienceItemListDto>>))]
        public IHttpActionResult GetFSSCJobExperiences([FromUri] FSSCJobExperienceQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = FSSCJobExperienceMapping.FSSCJobExperienceToListDto(items);
            var response = new ApiResponse<IEnumerable<FSSCJobExperienceItemListDto>>(itemsDto)
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
        } // GetFSSCJobExperiences

        [ResponseType(typeof(ApiResponse<FSSCJobExperienceItemDetailDto>))]
        public async Task<IHttpActionResult> GetFSSCJobExperience(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = FSSCJobExperienceMapping.FSSCJobExperienceToItemDetailDto(item);
            var response = new ApiResponse<FSSCJobExperienceItemDetailDto>(itemDto);

            return Ok(response);
        } // GetFSSCJobExperience

        [ResponseType(typeof(ApiResponse<FSSCJobExperienceItemDetailDto>))]
        public async Task<IHttpActionResult> PostFSSCJobExperience([FromBody] FSSCJobExperiencePostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = FSSCJobExperienceMapping.ItemAddDtoToFSSCJobExperience(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = FSSCJobExperienceMapping.FSSCJobExperienceToItemDetailDto(item);
            var response = new ApiResponse<FSSCJobExperienceItemDetailDto>(itemDto);

            return Ok(response);
        } // PostFSSCJobExperience

        [HttpPut]
        [ResponseType(typeof(ApiResponse<FSSCJobExperienceItemDetailDto>))]
        public async Task<IHttpActionResult> PutFSSCJobExperience(Guid id, [FromBody] FSSCJobExperiencePutDto itemPutDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemPutDto.ID)
                throw new BusinessException("ID mismatch");

            var item = FSSCJobExperienceMapping.ItemEditDtoToFSSCJobExperience(itemPutDto);
            item = await _service.UpdateAsync(item);
            var itemDto = FSSCJobExperienceMapping.FSSCJobExperienceToItemDetailDto(item);
            var response = new ApiResponse<FSSCJobExperienceItemDetailDto>(itemDto);

            return Ok(response);
        } // PostFSSCJobExperience

        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteFSSCJobExperience(Guid id, [FromBody] FSSCJobExperienceDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = FSSCJobExperienceMapping.ItemDeleteDtoToFSSCJobExperience(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteFSSCJobExperience
    }
}
