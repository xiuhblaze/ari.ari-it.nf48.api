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
    public class FSSCAuditExperiencesController : ApiController
    {
        private readonly FSSCAuditExperienceService _service;

        // CONSTRUCTOR

        public FSSCAuditExperiencesController()
        {
            _service = new FSSCAuditExperienceService();
        } // FSSCAuditExperiencesController

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<FSSCAuditExperienceItemListDto>>))]
        public IHttpActionResult GetFSSCAuditExperiences([FromUri] FSSCAuditExperienceQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemDto = FSSCAuditExperienceMapping.FSSCAuditExperienceToListDto(items);
            var response = new ApiResponse<IEnumerable<FSSCAuditExperienceItemListDto>>(itemDto)
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
        } // GetFSSCAuditExperiences

        [ResponseType(typeof(ApiResponse<FSSCAuditExperienceItemDetailDto>))]
        public async Task<IHttpActionResult> GetFSSCActionExperience(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = FSSCAuditExperienceMapping.FSSCAuditExperienceToItemDetailDto(item);
            var response = new ApiResponse<FSSCAuditExperienceItemDetailDto>(itemDto);

            return Ok(response);
        } // GetFSSCActionExperience

        [HttpPost]
        [ResponseType(typeof(ApiResponse<FSSCAuditExperienceItemDetailDto>))]
        public async Task<IHttpActionResult> PostFSSCAuditExperience([FromBody] FSSCAuditExperiencePostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = FSSCAuditExperienceMapping.ItemAddDtoToFSSCAuditExperience(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = FSSCAuditExperienceMapping.FSSCAuditExperienceToItemDetailDto(item);
            var response = new ApiResponse<FSSCAuditExperienceItemDetailDto>(itemDto);

            return Ok(response);
        } // PostFSSCAuditExperience

        [HttpPut]
        [ResponseType(typeof(ApiResponse<FSSCAuditExperienceItemDetailDto>))]
        public async Task<IHttpActionResult> PutFSSCAuditExperience(Guid id, [FromBody] FSSCAuditExperiencePutDto itemPutDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemPutDto.ID)
                throw new BusinessException("ID mismatch");

            var item = FSSCAuditExperienceMapping.ItemEditDtoToNASSCAuditExperience(itemPutDto);
            item = await _service.UpdateAsync(item);
            var itemDto = FSSCAuditExperienceMapping.FSSCAuditExperienceToItemDetailDto(item);
            var response = new ApiResponse<FSSCAuditExperienceItemDetailDto>(itemDto);

            return Ok(response);
        } // PutFSSCAuditExperience

        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteFSSCAuditExperience(Guid id, [FromBody] FSSCAuditExperienceDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = FSSCAuditExperienceMapping.ItemDeleteDtoToFSSCAuditExperience(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteFSSCJobExperience
    }
}
