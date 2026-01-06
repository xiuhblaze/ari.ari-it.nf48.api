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
    public class AppFormsController : ApiController
    {
        private readonly AppFormService _service;

        // CONSTRUCTOR

        public AppFormsController()
        {
            _service = new AppFormService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<AppFormItemListDto>>))]
        public IHttpActionResult GetAppForms([FromUri] AppFormQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = AppFormMapping.AppFormToListDto(items); // items.Select(AppFormMapping.AppFormToItemListDto);
            var response = new ApiResponse<IEnumerable<AppFormItemListDto>>(itemsDto)
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
        } // GetAppForms

        [HttpGet]
        [ResponseType(typeof(ApiResponse<AppFormItemDetailDto>))]
        public async Task<IHttpActionResult> GetAppForm(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = await AppFormMapping.AppFormToItemDetailDto(item);
            var response = new ApiResponse<AppFormItemDetailDto>(itemDto);

            return Ok(response);
        } // GetAppForm

        [HttpPost]
        [ResponseType(typeof(ApiResponse<AppFormItemDetailDto>))]
        public async Task<IHttpActionResult> PostAppForm([FromBody] AppFormCreateDto itemAddDto)
        {
            if (!ModelState.IsValid) 
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = AppFormMapping.ItemCreateDtoToAppForm(itemAddDto);
            var itemDto = await AppFormMapping.AppFormToItemDetailDto(await _service.AddAsync(item));
            var response = new ApiResponse<AppFormItemDetailDto>(itemDto);

            return Ok(response);
        } // PostAppForm

        [HttpPost]
        [Route("api/AppForms/Duplicate")]
        [ResponseType(typeof(ApiResponse<AppFormItemDetailDto>))]
        public async Task<IHttpActionResult> DuplicateAppForm([FromBody] AppFormDuplicateDto itemDuplicateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var newItem = await _service.DuplicateAsync(itemDuplicateDto.ID, itemDuplicateDto.UpdatedUser);
            var itemDto = await AppFormMapping.AppFormToItemDetailDto(newItem);
            var response = new ApiResponse<AppFormItemDetailDto>(itemDto);

            return Ok(response);
        } // DuplicateAppForm

        [HttpPut]
        [ResponseType(typeof(ApiResponse<AppFormItemDetailDto>))]
        public async Task<IHttpActionResult> PutAppForm(Guid id, [FromBody] AppFormUpdateDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("The ID of the item does not match the ID of the request");

            var item = AppFormMapping.ItemUpdateDtoToAppForm(itemEditDto);            
            var itemDto = await AppFormMapping.AppFormToItemDetailDto(await _service.UpdateAsync(item));
            var response = new ApiResponse<AppFormItemDetailDto>(itemDto);

            return Ok(response);
        } // PutAppForm

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteAppForm(Guid id, [FromBody] AppFormDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("The ID of the item does not match the ID of the request");

            var item = AppFormMapping.ItemDeleteDtoToAppForm(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteAppForm

        // NACE CODES

        [HttpPost]
        [Route("api/AppForms/{id}/nace-code")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> AddNaceCode(Guid id, [FromBody] AppFormNaceCodeDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.AppFormID)
                throw new BusinessException("The ID of the item does not match the ID of the request");
                        
            await _service.AddNaceCodeAsync(itemDto.AppFormID, itemDto.NaceCodeID);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // AddNaceCode

        [HttpDelete]
        [Route("api/AppForms/{id}/nace-code")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DelNaceCode(Guid id, [FromBody] AppFormNaceCodeDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.AppFormID)
                throw new BusinessException("The ID of the item does not match the ID of the request");

            await _service.DelNaceCodeAsync(itemDto.AppFormID, itemDto.NaceCodeID);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DelNaceCode

        // CONTACTS

        [HttpPost]
        [Route("api/AppForms/{id}/contact")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> AddContact(Guid id, [FromBody] AppFormContactDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.AppFormID)
                throw new BusinessException("The ID of the item does not match the ID of the request");

            await _service.AddContactAsync(itemDto.AppFormID, itemDto.ContactID);
            var response = new ApiResponse<bool>(true);
            
            return Ok(response);
        } // AddContact

        [HttpDelete]
        [Route("api/AppForms/{id}/contact")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DelContact(Guid id, [FromBody] AppFormContactDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.AppFormID)
                throw new BusinessException("The ID of the item does not match the ID of the request");

            await _service.DelContactAsync(itemDto.AppFormID, itemDto.ContactID);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DelContact

        // SITES

        [HttpPost]
        [Route("api/AppForms/{id}/site")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> AddSite(Guid id, [FromBody] AppFormSiteDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.AppFormID)
                throw new BusinessException("The ID of the item does not match the ID of the request");

            await _service.AddSiteAsync(itemDto.AppFormID, itemDto.SiteID);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // AddSite

        [HttpDelete]
        [Route("api/AppForms/{id}/site")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DelSite(Guid id, [FromBody] AppFormSiteDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.AppFormID)
                throw new BusinessException("The ID of the item does not match the ID of the request");

            await _service.DelSiteAsync(itemDto.AppFormID, itemDto.SiteID);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DelSite
    }
}
