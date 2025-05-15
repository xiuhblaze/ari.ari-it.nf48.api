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
    public class RolesController : ApiController
    {
        private readonly RoleService roleService;

        // CONSTRUCTOR

        public RolesController()
        {
            roleService = new RoleService();
        }

        // ENDPOINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<RoleItemListDto>>))]
        public IHttpActionResult GetRoles([FromUri] RoleQueryFilters filters)
        { 
            var items = roleService.Gets(filters);
            var itemsDto = RoleMapping.RolesToListDto(items);
            var response = new ApiResponse<IEnumerable<RoleItemListDto>>(itemsDto);
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
        } // GetRoles

        [ResponseType(typeof(ApiResponse<UserDetailDto>))]
        public async Task<IHttpActionResult> GetRole(Guid id)
        { 
            var item = await roleService.GetAsync(id) 
                ?? throw new BusinessException("Item not found");
            var itemDto = await RoleMapping.RoleToItemDetailDto(item);    
            var response = new ApiResponse<RoleItemDetailDto>(itemDto);

            return Ok(response);
        } // GetRole

        [ResponseType(typeof(ApiResponse<RoleItemDetailDto>))]
        public async Task<IHttpActionResult> PostRole([FromBody] RolePostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = await roleService
                .AddAsync(new Models.Role { UpdatedUser = itemAddDto.UpdatedUser });
            var itemDto = await RoleMapping.RoleToItemDetailDto(item);
            var response = new ApiResponse<RoleItemDetailDto>(itemDto);

            return Ok(response);
        } // PostRole

        [ResponseType(typeof(ApiResponse<RoleItemDetailDto>))]
        public async Task<IHttpActionResult> PutRole(Guid id, [FromBody] RolePutDto itemEditDto)
        {
            if (!ModelState.IsValid) throw new BusinessException(Strings.GetModelStateErrors(ModelState));
            if (id != itemEditDto.ID) throw new BusinessException("ID mismatch");

            var itemToEdit = RoleMapping.ItemEditDtoToRole(itemEditDto);
            var item = await roleService.UpdateAsync(itemToEdit);
            var itemDto = await RoleMapping.RoleToItemDetailDto(item);
            var response = new ApiResponse<RoleItemDetailDto>(itemDto);

            return Ok(response);
        } // PutUser

        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteRole(Guid id, [FromBody] RoleDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid) throw new BusinessException(Strings.GetModelStateErrors(ModelState));
            if (id != itemDeleteDto.ID) throw new BusinessException("ID mismatch");

            var item = RoleMapping.ItemDeleteDtoToRole(itemDeleteDto);
            await roleService.DeleteAsync(item);

            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteRole
    }
}
