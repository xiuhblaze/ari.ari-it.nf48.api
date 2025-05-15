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
    public class UsersController : ApiController
    {
        private UserService _userService;

        public UsersController()
        {
            _userService = new UserService();
        }

        // ENDPOINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<UserListItemDto>>))]
        public async Task<IHttpActionResult> GetUsers([FromUri] UserQueryFilters filters)
        {
            var items = _userService.Gets(filters);
            var itemsDto = await UserMapping.UsersToListDto(items);

            var response = new ApiResponse<IEnumerable<UserListItemDto>>(itemsDto);
            var metadata = new Metadata
            {
                TotalCount = items.TotalCount,
                PageSize = items.PageSize,
                CurrentPage = items.CurrentPage,
                TotalPages = items.TotalPages,
                HasPreviousPage = items.HasPreviousPage,
                HasNextPage = items.HasNextPage
            };
            response.Meta = metadata;

            return Ok(response);
        } // GetUsers

        [ResponseType(typeof(ApiResponse<UserDetailDto>))]
        public async Task<IHttpActionResult> GetUser(Guid id)
        {
            try
            {
                var item = await _userService.GetAsync(id);
                var itemDto = await UserMapping.UserToDetailDto(item);
                var response = new ApiResponse<UserDetailDto>(itemDto);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetUser

        [ResponseType(typeof(ApiResponse<UserDetailDto>))]
        public async Task<IHttpActionResult> PostUser([FromBody] UserPostDto itemAddDto)
        {
            if (!ModelState.IsValid) throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = await _userService
                .AddAsync(new User { UpdatedUser = itemAddDto.UpdatedUser });
            var itemDto = await UserMapping.UserToDetailDto(item);
            var response = new ApiResponse<UserDetailDto>(itemDto);

            return Ok(response);

        } // PostUser

        [ResponseType(typeof(ApiResponse<UserDetailDto>))]
        public async Task<IHttpActionResult> PutUser(Guid id, [FromBody] UserPutDto itemEditDto)
        {
            if (!ModelState.IsValid) throw new BusinessException(Strings.GetModelStateErrors(ModelState));
            if (id != itemEditDto.ID) throw new BusinessException("ID mismatch");

            var itemToEdit = UserMapping.ItemEditDtoToUser(itemEditDto);
            var item = await _userService.UpdateAsync(itemToEdit);
            var itemDto = await UserMapping.UserToDetailDto(item);
            var response = new ApiResponse<UserDetailDto>(itemDto);

            return Ok(response);
        } // PutUser

        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteUser(Guid id, [FromBody] UserDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid) throw new BusinessException(Strings.GetModelStateErrors(ModelState));
            if (id != itemDeleteDto.ID) throw new BusinessException("ID mismatch");

            var item = UserMapping.ItemDeleteDtoToUser(itemDeleteDto);
            await _userService.DeleteAsync(item);

            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteUser

        // ROLES

        [HttpPost]
        [Route("api/Users/{id}/role")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> AddRole(Guid id, [FromBody] UserEditRoleDto itemAddRole)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemAddRole.ID) 
                throw new BusinessException("ID mismatch");

            await _userService.AddRoleAsync(id, itemAddRole.RoleID);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // AddRole

        [HttpDelete]
        [Route("api/Users/{id}/role")]
        public async Task<IHttpActionResult> DelRole(Guid id, [FromBody] UserEditRoleDto itemDelRole)
        {
            if (!ModelState.IsValid) 
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelRole.ID) 
                throw new BusinessException("ID mismatch");

            await _userService.DelRoleAsync(id, itemDelRole.RoleID);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DelRole
    }
}
