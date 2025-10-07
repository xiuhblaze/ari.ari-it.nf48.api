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
    public class UserSettingsController : ApiController
    {
        private readonly UserSettingService _service;

        // CONSTRUCTOR

        public UserSettingsController()
        {
            _service = new UserSettingService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<UserSettingItemDto>>))]
        public IHttpActionResult GetUserSettings([FromUri] UserSettingQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = UserSettingMapping.UserSettingToListDto(items);
            var response = new ApiResponse<IEnumerable<UserSettingItemDto>>(itemsDto)
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
        } // GetUserSettings

        [HttpGet]
        [ResponseType(typeof(ApiResponse<UserSettingItemDto>))]
        public async Task<IHttpActionResult> GetUserSetting(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = UserSettingMapping.UserSettingToItemDto(item);
            var response = new ApiResponse<UserSettingItemDto>(itemDto);

            return Ok(response);
        } // GetUserSetting

        [HttpPost]
        [ResponseType(typeof(ApiResponse<UserSettingItemDto>))]
        public async Task<IHttpActionResult> PostUserSetting([FromBody] UserSettingCreateDto itemCreateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = UserSettingMapping
                .ItemCreateDtoToUserSetting(itemCreateDto);
            var itemDto = UserSettingMapping
                .UserSettingToItemDto(await _service.CreateAsync(item));
            var response = new ApiResponse<UserSettingItemDto>(itemDto);

            return Ok(response);
        } // PostUserSetting

        [HttpPut]
        [ResponseType(typeof(ApiResponse<UserSettingItemDto>))]
        public async Task<IHttpActionResult> PutUserSetting(Guid id, [FromBody] UserSettingUpdateDto itemUpdateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemUpdateDto.ID)
                throw new BusinessException("ID mismatch");

            var item = UserSettingMapping
                .ItemUpdateDtoToUserSetting(itemUpdateDto);
            var itemDto = UserSettingMapping
                .UserSettingToItemDto(await _service.UpdateAsync(item));
            var response = new ApiResponse<UserSettingItemDto>(itemDto);

            return Ok(response);
        } // PutUserSetting

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteUserSetting(Guid id, [FromBody] UserSettingDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = UserSettingMapping
                .ItemDeleteDtoToUserSetting(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteUserSetting
    }
}
