using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.IO;
using Arysoft.ARI.NF48.Api.Mappings;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;
using Arysoft.ARI.NF48.Api.Services;
using Arysoft.ARI.NF48.Api.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var itemUpdated = await _service.UpdateAsync(itemToUpdate);
            var itemDto = ADCSiteMapping
                .ADCSiteToItemDetailDto(itemUpdated);
            var response = new ApiResponse<ADCSiteItemDetailDto>(itemDto);

            return Ok(response);
        } // PutADCSite - with file

        [HttpPut]
        [Route("api/ADCSites/list")]
        [ResponseType(typeof(ApiResponse<ADCSiteItemListDto>))]
        public async Task<IHttpActionResult> PutADCSiteList()
        {
            var data = HttpContext.Current.Request.Params["data"];
            var files = HttpContext.Current.Request.Files.Count > 0
                ? HttpContext.Current.Request.Files
                : null;

            ADCSiteListUpdateDto itemsUpdateDto = JsonConvert
                .DeserializeObject<ADCSiteListUpdateDto>(data)
                ?? throw new BusinessException("Can't read data object");

            var itemsToUpdate = ADCSiteMapping.UpdateListDtoToADCSite(itemsUpdateDto);

            if (ADCSiteService.IsMultiStandard(itemsUpdateDto.Items.First().ID))
            {   
                if (files != null && files.Count > 0)
                {
                    itemsToUpdate = await SaveAllFilesAsync(files, itemsToUpdate.ToList());
                }
            }

            var resultItems = await _service.UpdateListAsync(itemsToUpdate.ToList());
            var itemsDto = ADCSiteMapping.ADCSiteToListDto(resultItems);
            var response = new ApiResponse<IEnumerable<ADCSiteItemListDto>>(itemsDto);
            
            return Ok(response);
        } // PutADCSiteList

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
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

        // PRIVATE METHODS

        private async Task<List<ADCSite>> SaveAllFilesAsync(HttpFileCollection files, List<ADCSite> items)
        {
            List<ADCSite> itemsToUpdate = new List<ADCSite>();

            try
            {

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile file = files[i];

                    if (file.ContentLength > 0)
                    {
                        var filename = Path.GetFileNameWithoutExtension(file.FileName);
                        Guid id = new Guid(filename);
                        var foundItem = await _service.GetAsync(id)
                            ?? throw new BusinessException("Item not found");
                        var itemToUpdate = items.Find(x => x.ID == id)
                            ?? throw new BusinessException($"Item to update not found: {id}");

                        var organizationID = foundItem.ADC.AppForm.AuditCycle.OrganizationID.ToString();
                        var auditCycleID = foundItem.ADC.AppForm.AuditCycle.ID.ToString();

                        var newFilename = FileRepository.UploadFile(
                            file,
                            $"~/files/organizations/{organizationID}/Cycles/{auditCycleID}/ADC",
                            foundItem.ID.ToString()
                        );

                        itemToUpdate.MD11Filename = newFilename;
                        itemToUpdate.MD11UploadedBy = itemToUpdate.UpdatedUser;

                        itemsToUpdate.Add(itemToUpdate);
                    }
                }

                // Add items that are not in the itemsToUpdate list
                foreach (var item in items)
                {
                    if (!itemsToUpdate.Exists(x => x.ID == item.ID))
                    {
                        itemsToUpdate.Add(item);
                    }
                }

            } catch (Exception ex)
            {
                throw new BusinessException($"ADCSitesController.SaveAllFilesAsync: {ex.Message}");
            }

            return itemsToUpdate;
        } // SaveAllFiles
    }
}
