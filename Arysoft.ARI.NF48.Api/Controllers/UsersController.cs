using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Mappings;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;
using Arysoft.ARI.NF48.Api.Services;
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
        public IHttpActionResult GetUsers([FromUri] UserQueryFilters filters)
        { 
            var items = _userService.Gets(filters);
            var itemsDto = UserMapping.UsersToListDto(items);

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
            var item = await _userService.GetAsync(id);
            var itemDto = UserMapping.UserToDetailDto(item);

            var response = new ApiResponse<UserDetailDto>(itemDto);
            return Ok(response);
        } // GetUser

        // public async Task<IHttpActionResult> PostUser([FromBody] )
    }
}
