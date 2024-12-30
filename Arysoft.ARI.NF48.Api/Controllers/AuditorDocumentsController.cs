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
    public class AuditorDocumentsController : ApiController
    {
        private readonly AuditorDocumentService _service;

        // CONSTRUCTOR

        public AuditorDocumentsController()
        {
            _service = new AuditorDocumentService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<AuditorDocumentItemListDto>>))]
        public IHttpActionResult GetAuditorDocuments([FromUri] AuditorDocumentQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = AuditorDocumentMapping.AuditorDocumentToListDto(items);
            var response = new ApiResponse<IEnumerable<AuditorDocumentItemListDto>>(itemsDto)
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
        } // GetAuditorDocuments

        [ResponseType(typeof(ApiResponse<AuditorDocumentItemDetailDto>))]
        public async Task<IHttpActionResult> GetAuditorDocument(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = AuditorDocumentMapping.AuditorDocumentToItemDetailDto(item);
            var response = new ApiResponse<AuditorDocumentItemDetailDto>(itemDto);

            return Ok(response);
        } // GetAuditorDocument 

        [ResponseType(typeof(ApiResponse<AuditorDocumentItemDetailDto>))]
        public async Task<IHttpActionResult> PostAuditorDocument([FromBody] AuditorDocumentPostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = AuditorDocumentMapping.ItemAddDtoToAuditorDocument(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = AuditorDocumentMapping.AuditorDocumentToItemDetailDto(item);
            var response = new ApiResponse<AuditorDocumentItemDetailDto>(itemDto);

            return Ok(response);
        } // PostAuditorDocument

        [ResponseType(typeof(ApiResponse<AuditorDocumentItemDetailDto>))]
        public async Task<IHttpActionResult> PutAuditorDocument()
        {
            var data = HttpContext.Current.Request.Params["data"];
            var file = HttpContext.Current.Request.Files.Count > 0
                ? HttpContext.Current.Request.Files[0] 
                : null;
            string filename = null;

            AuditorDocumentPutDto itemEditDto = JsonConvert.DeserializeObject<AuditorDocumentPutDto>(data) 
                ?? throw new BusinessException("Can't read data object");

            var item = await _service.GetAsync(itemEditDto.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (file != null)
            {
                filename = FileRepository.UploadFile(
                    file,
                    $"~/files/auditors/{item.AuditorID}",
                    itemEditDto.ID.ToString()
                );
            }

            var itemToEdit = AuditorDocumentMapping.ItemEditDtoToAuditorDocument(itemEditDto);
            itemToEdit.Filename = filename ?? item.Filename;
            item = await _service.UpdateAsync(itemToEdit);
            var itemDto = AuditorDocumentMapping.AuditorDocumentToItemDetailDto(item);
            var response = new ApiResponse<AuditorDocumentItemDetailDto>(itemDto);

            return Ok(response);
        } // PutAuditorDocument

        [HttpDelete]
        [Route("api/AuditorDocuments/{id}/DocumentFile")]
        public async Task<IHttpActionResult> DeletePhotofile(Guid id, [FromBody] AuditorDeleteDto itemDelDto)
        {
            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Record to delete file not found");

            if (FileRepository.DeleteFile($"~/files/auditors/{item.ID}", item.Filename))
            {
                item.Filename = null;
                item.UpdatedUser = itemDelDto.UpdatedUser;
                await _service.UpdateAsync(item);
            }

            return Ok(new ApiResponse<bool>(true));
        } // DeletePhotofile

        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteAuditorDocument(Guid id, [FromBody] AuditorDocumentDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            //TODO: Si el estatus es Eliminado, borrar el archivo físico

            var item = AuditorDocumentMapping.ItemDeleteDtoToAuditorDocument(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteAuditorDocument
    }
}
