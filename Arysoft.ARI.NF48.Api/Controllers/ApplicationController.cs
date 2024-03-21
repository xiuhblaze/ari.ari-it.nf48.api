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
    public class ApplicationController : ApiController
    {
        private ApplicationService _applicationService;

        // CONSTRUCTOR 

        public ApplicationController()
        {
            _applicationService = new ApplicationService();
        }

        // ENDPOINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<ApplicationItemListDto>>))]
        public IHttpActionResult GetApplications([FromUri] ApplicationQueryFilters filters)
        {
            var items = _applicationService.Gets(filters);
            var itemsDto = ApplicationMapping.ApplicationsToListDto(items);

            var response = new ApiResponse<IEnumerable<ApplicationItemListDto>>(itemsDto);
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
        } // GetApplications

        [HttpGet]
        [ResponseType(typeof(ApiResponse<ApplicationItemDetailDto>))]
        public async Task<IHttpActionResult> GetApplication(Guid id)
        {
            var item = await _applicationService.GetAsync(id);
            var itemDto = ApplicationMapping.ApplicationToItemDetailDto(item);
            var response = new ApiResponse<ApplicationItemDetailDto>(itemDto);

            return Ok(response);
        } // GetApplication

        [HttpPost]
        [ResponseType(typeof(ApiResponse<ApplicationItemDetailDto>))]
        public async Task<IHttpActionResult> PostApplication([FromBody] ApplicationPostDto itemAddDto)
        {
            if (!ModelState.IsValid) throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = await _applicationService
                .AddAsync(new Application { UpdatedUser = itemAddDto.UpdatedUser });
            var itemDto = ApplicationMapping.ApplicationToItemDetailDto(item);
            var response = new ApiResponse<ApplicationItemDetailDto>(itemDto);

            return Ok(response);
        } // PostApplication

        [HttpPut]
        [ResponseType(typeof(ApiResponse<ApplicationItemDetailDto>))]
        public async Task<IHttpActionResult> PutApplication(Guid id, [FromBody] ApplicationPutDto itemEditDto)
        {
            if (!ModelState.IsValid) throw new BusinessException(Strings.GetModelStateErrors(ModelState));
            if (id != itemEditDto.ID) throw new BusinessException("ID mismatch");

            var itemToEdit = ApplicationMapping.ItemEditDtoToApplication(itemEditDto);
            var item = await _applicationService.UpdateAsync(itemToEdit);
            var itemDto = ApplicationMapping.ApplicationToItemDetailDto(item);
            var response = new ApiResponse<ApplicationItemDetailDto>(itemDto);

            return Ok(response);
        } // PutApplication

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteApplication(Guid id, [FromBody] ApplicationDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid) throw new BusinessException(Strings.GetModelStateErrors(ModelState));
            if (id != itemDeleteDto.ID) throw new BusinessException("ID mismatch");

            var item = new Application { 
                ID = itemDeleteDto.ID,
                UpdatedUser = itemDeleteDto.UpdatedUser
            };
            await _applicationService.DeleteAsync(item);
            var response = new ApiResponse<bool>(true); 
            
            return Ok(response);
        } // DeleteApplication
    }
}
