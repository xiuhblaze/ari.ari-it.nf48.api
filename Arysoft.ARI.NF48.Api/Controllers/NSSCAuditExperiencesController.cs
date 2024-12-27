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
    public class NSSCAuditExperiencesController : ApiController
    {
        private readonly NSSCAuditExperienceService _service;

        // CONSTRUCTOR

        public NSSCAuditExperiencesController()
        {
            _service = new NSSCAuditExperienceService();
        } // NSSCAuditExperiencesController

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<NSSCAuditExperienceItemListDto>>))]
        public IHttpActionResult GetNSSCAuditExperiences([FromUri] NSSCAuditExperienceQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemDto = NSSCAuditExperienceMapping.NSSCAuditExperienceToListDto(items);
            var response = new ApiResponse<IEnumerable<NSSCAuditExperienceItemListDto>>(itemDto)
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
        } // GetNSSCAuditExperiences

        [ResponseType(typeof(ApiResponse<NSSCAuditExperienceItemDetailDto>))]
        public async Task<IHttpActionResult> GetNSSCActionExperience(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = NSSCAuditExperienceMapping.NSSCAuditExperienceToItemDetailDto(item);
            var response = new ApiResponse<NSSCAuditExperienceItemDetailDto>(itemDto);

            return Ok(response);
        } // GetNSSCActionExperience

        [HttpPost]
        [ResponseType(typeof(ApiResponse<NSSCAuditExperienceItemDetailDto>))]
        public async Task<IHttpActionResult> PostNSSCAuditExperience([FromBody] NSSCAuditExperiencePostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = NSSCAuditExperienceMapping.ItemAddDtoToNSSCAuditExperience(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = NSSCAuditExperienceMapping.NSSCAuditExperienceToItemDetailDto(item);
            var response = new ApiResponse<NSSCAuditExperienceItemDetailDto>(itemDto);

            return Ok(response);
        } // PostNSSCAuditExperience

        [HttpPut]
        [ResponseType(typeof(ApiResponse<NSSCAuditExperienceItemDetailDto>))]
        public async Task<IHttpActionResult> PutNSSCAuditExperience(Guid id, [FromBody] NSSCAuditExperiencePutDto itemPutDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemPutDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NSSCAuditExperienceMapping.ItemEditDtoToNASSCAuditExperience(itemPutDto);
            item = await _service.UpdateAsync(item);
            var itemDto = NSSCAuditExperienceMapping.NSSCAuditExperienceToItemDetailDto(item);
            var response = new ApiResponse<NSSCAuditExperienceItemDetailDto>(itemDto);

            return Ok(response);
        } // PutNSSCAuditExperience

        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteNSSCAuditExperience(Guid id, [FromBody] NSSCAuditExperienceDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NSSCAuditExperienceMapping.ItemDeleteDtoToNSSCAuditExperience(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteNSSCJobExperience
    }
}
