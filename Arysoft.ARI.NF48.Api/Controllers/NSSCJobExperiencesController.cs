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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class NSSCJobExperiencesController : ApiController
    {
        private readonly NSSCJobExperienceService _service;

        // CONSTRUCTOR

        public NSSCJobExperiencesController()
        {
            _service = new NSSCJobExperienceService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<NSSCJobExperienceItemListDto>>))]
        public IHttpActionResult GetNSSCJobExperiences([FromUri] NSSCJobExperienceQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = NSSCJobExperienceMapping.NSSCJobExperienceToListDto(items);
            var response = new ApiResponse<IEnumerable<NSSCJobExperienceItemListDto>>(itemsDto)
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
        } // GetNSSCJobExperiences

        [ResponseType(typeof(ApiResponse<NSSCJobExperienceItemDetailDto>))]
        public async Task<IHttpActionResult> GetNSSCJobExperience(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = NSSCJobExperienceMapping.NSSCJobExperienceToItemDetailDto(item);
            var response = new ApiResponse<NSSCJobExperienceItemDetailDto>(itemDto);

            return Ok(response);
        } // GetNSSCJobExperience

        [ResponseType(typeof(ApiResponse<NSSCJobExperienceItemDetailDto>))]
        public async Task<IHttpActionResult> PostNSSCJobExperience([FromBody] NSSCJobExperiencePostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = NSSCJobExperienceMapping.ItemAddDtoToNSSCJobExperience(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = NSSCJobExperienceMapping.NSSCJobExperienceToItemDetailDto(item);
            var response = new ApiResponse<NSSCJobExperienceItemDetailDto>(itemDto);

            return Ok(response);
        } // PostNSSCJobExperience

        [HttpPut]
        [ResponseType(typeof(ApiResponse<NSSCJobExperienceItemDetailDto>))]
        public async Task<IHttpActionResult> PutNSSCJobExperience(Guid id, [FromBody] NSSCJobExperiencePutDto itemPutDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemPutDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NSSCJobExperienceMapping.ItemEditDtoToNSSCJobExperience(itemPutDto);
            item = await _service.UpdateAsync(item);
            var itemDto = NSSCJobExperienceMapping.NSSCJobExperienceToItemDetailDto(item);
            var response = new ApiResponse<NSSCJobExperienceItemDetailDto>(itemDto);

            return Ok(response);
        } // PostNSSCJobExperience

        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteNSSCJobExperience(Guid id, [FromBody] NSSCJobExperienceDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NSSCJobExperienceMapping.ItemDeleteDtoToNSSCJobExperience(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteNSSCJobExperience
    }
}
