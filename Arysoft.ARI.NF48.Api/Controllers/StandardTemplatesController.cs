using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.IO;
using Arysoft.ARI.NF48.Api.Mappings;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;
using Arysoft.ARI.NF48.Api.Services;
using Arysoft.ARI.NF48.Api.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class StandardTemplatesController : ApiController
    {
        private readonly StandardTemplateService _service;

        // CONSTRUCTOR

        public StandardTemplatesController()
        {
            _service = new StandardTemplateService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<StandardTemplateItemListDto>>))]
        public IHttpActionResult GetStandarTemplates([FromUri] StandardTemplateQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = StandardTemplateMapping.StandardTemplatesToListDto(items);
            var response = new ApiResponse<IEnumerable<StandardTemplateItemListDto>>(itemsDto)
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
        } //GetStandarTemplates

        [HttpGet]
        [ResponseType(typeof(ApiResponse<StandardTemplateItemDetailDto>))]
        public async Task<IHttpActionResult> GetStandardTemplate(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new Exception("Item not found");
            var itemDto = StandardTemplateMapping.StandardTemplateToItemDetailDto(item);
            var response = new ApiResponse<StandardTemplateItemDetailDto>(itemDto);

            return Ok(response);
        } // GetStandardTemplate

        [HttpPost]
        [ResponseType(typeof(ApiResponse<StandardTemplateItemDetailDto>))]
        public async Task<IHttpActionResult> PostStandardTemplate([FromBody] StandardTemplateCreateDto itemCreateDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = StandardTemplateMapping.ItemCreateDtoToStandardTemplate(itemCreateDto);
            item = await _service.CreateAsync(item);
            var itemDto = StandardTemplateMapping.StandardTemplateToItemDetailDto(item);
            var response = new ApiResponse<StandardTemplateItemDetailDto>(itemDto);

            return Ok(response);
        } // PostStandardTemplate

        //[HttpPut]
        //[ResponseType(typeof(ApiResponse<StandardTemplateItemDetailDto>))]
        //public async Task<IHttpActionResult> PutStandardTemplate(Guid id, [FromBody] StandardTemplateUpdateDto itemUpdateDto)
        //{

        //    if (!ModelState.IsValid)
        //        throw new BusinessException(Strings.GetModelStateErrors(ModelState));

        //    if (id != itemUpdateDto.ID)
        //        throw new BusinessException("ID mismatch");

        //    var item = StandardTemplateMapping.ItemUpdateDtoToStandardTemplate(itemUpdateDto);
        //    item = await _service.UpdateAsync(item);
        //    var itemDto = StandardTemplateMapping.StandardTemplateToItemDetailDto(item);
        //    var response = new ApiResponse<StandardTemplateItemDetailDto>(itemDto);

        //    return Ok(response);
        //} // PutStandardTemplate

        [HttpPut]
        [ResponseType(typeof(ApiResponse<StandardTemplateItemDetailDto>))]
        public async Task<IHttpActionResult> PutStandardTemplate()
        { 
            var data = HttpContext.Current.Request.Form["data"];
            var file = HttpContext.Current.Request.Files.Count > 0 
                ? HttpContext.Current.Request.Files[0] 
                : null;
            string filename = null;

            if (string.IsNullOrEmpty(data))
                throw new BusinessException("No data");

            StandardTemplateUpdateDto itemUpdateDto = JsonConvert
                .DeserializeObject<StandardTemplateUpdateDto>(data)
                ?? throw new BusinessException("Can't read data object");

            var item = await _service.GetAsync(itemUpdateDto.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (file != null)
            {
                filename = FileRepository.UploadFile(
                    file,
                    $"~/files/standards/{item.StandardID}",
                    item.ID.ToString(),
                    new string[] { ".docx", "xlsx", ".pdf" }
                );
            }

            var itemToUpdate = StandardTemplateMapping.ItemUpdateDtoToStandardTemplate(itemUpdateDto);
            itemToUpdate.Filename = filename ?? item.Filename;
            item = await _service.UpdateAsync(itemToUpdate);
            var itemDto = StandardTemplateMapping.StandardTemplateToItemDetailDto(item);
            var response = new ApiResponse<StandardTemplateItemDetailDto>(itemDto);

            return Ok(response);
        } // PutStandardTemplate

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteStandardTemplate(Guid id, [FromBody] StandardTemplateDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID) 
                throw new BusinessException("ID mismatch");

            var item = StandardTemplateMapping.ItemDeleteDtoToStandardTemplate(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteStandardTemplate
    }
}
