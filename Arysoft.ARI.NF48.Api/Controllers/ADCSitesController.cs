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
    public class ADCSitesController : ApiController
    {
        private readonly ADCSiteService _service;

        // CONSTRUCTOR
        
        public ADCSitesController()
        {
            _service = new ADCSiteService();
        }

        // END POINTS
        
        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<ADCSiteItemListDto>>))]
        public IHttpActionResult GetADCSites([FromUri] ADCSiteQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = ADCSiteMapping.ADCSiteToListDto(items);
            var response = new ApiResponse<IEnumerable<ADCSiteItemListDto>>(itemsDto)
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
        } // GetADCSites

        [HttpGet]
        [ResponseType(typeof(ApiResponse<ADCSiteItemDetailDto>))]
        public async Task<IHttpActionResult> GetADCSite(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = ADCSiteMapping
                .ADCSiteToItemDetailDto(item);
            var response = new ApiResponse<ADCSiteItemDetailDto>(itemDto);

            return Ok(response);
        } // GetADCSite

        [HttpPost]
        [ResponseType(typeof(ApiResponse<ADCSiteItemDetailDto>))]
        public async Task<IHttpActionResult> PostADCSite([FromBody] ADCSiteItemCreateDto itemCreateDto)
        { 
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = ADCSiteMapping
                .ItemCreateDtoToADCSite(itemCreateDto);
            var itemDto = ADCSiteMapping
                .ADCSiteToItemDetailDto(await _service.AddAsync(item));
            var response = new ApiResponse<ADCSiteItemDetailDto>(itemDto);

            return Ok(response);
        } // PostADCSite

        [HttpPut]
        [ResponseType(typeof(ApiResponse<ADCSiteItemDetailDto>))]
        public async Task<IHttpActionResult> PutADCSite()
        {
            var data = HttpContext.Current.Request.Params["data"];
            var file = HttpContext.Current.Request.Files.Count > 0
                ? HttpContext.Current.Request.Files[0]
                : null;
            string filename = null;

            ADCSiteItemUpdateDto itemUpdateDto = JsonConvert
                    .DeserializeObject<ADCSiteItemUpdateDto>(data)
                ?? throw new BusinessException("Can't read data object");

            var item = await _service.GetAsync(itemUpdateDto.ID)
                ?? throw new BusinessException("Item not found");

            if (file != null && file.ContentLength > 0)
            {
                var organizationID = item.ADC.AppForm.AuditCycle.OrganizationID.ToString();
                var auditCycleID = item.ADC.AppForm.AuditCycle.ID.ToString();

                filename = FileRepository.UploadFile(
                    file,
                    $"~/files/organizations/{organizationID}/Cycles/{auditCycleID}/ADC",
                    item.ID.ToString()
                );
            }

            var itemToUpdate = ADCSiteMapping.ItemUpdateDtoToADCSite(itemUpdateDto);

            itemToUpdate.MD11Filename = filename ?? item.MD11Filename;
            itemToUpdate.MD11UploadedBy = string.IsNullOrEmpty(filename)
                ? item.MD11UploadedBy
                : itemToUpdate.UpdatedUser;

            var itemDto = ADCSiteMapping
                .ADCSiteToItemDetailDto(await _service.UpdateAsync(itemToUpdate));
            var response = new ApiResponse<ADCSiteItemDetailDto>(itemDto);

            return Ok(response);
        } // PutADCSite - with file

        //[HttpPut] // OLD CODE - Antes de Fileupload
        //[ResponseType(typeof(ApiResponse<ADCSiteItemDetailDto>))]
        //public async Task<IHttpActionResult> PutADCSite(Guid id, [FromBody] ADCSiteItemUpdateDto itemUpdateDto)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BusinessException(Strings.GetModelStateErrors(ModelState));

        //    if (id != itemUpdateDto.ID)
        //        throw new BusinessException("ID mismatch");

        //    var item = ADCSiteMapping
        //        .ItemUpdateDtoToADCSite(itemUpdateDto);
        //    var itemDto = ADCSiteMapping
        //        .ADCSiteToItemDetailDto(await _service.UpdateAsync(item));
        //    var response = new ApiResponse<ADCSiteItemDetailDto>(itemDto);

        //    return Ok(response);
        //} // PutADCSite

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<ADCSiteItemDetailDto>))]
        public async Task<IHttpActionResult> DeleteADCSite(Guid id, [FromBody] ADCSiteItemDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            var item = ADCSiteMapping.ItemDeleteDtoToADCSite(itemDeleteDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteADCSite
    }
}
