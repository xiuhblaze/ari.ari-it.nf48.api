using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
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
    public class AuditCycleDocumentsController : ApiController
    {
        private readonly AuditCycleDocumentService _service;

        // CONSTRUCTOR

        public AuditCycleDocumentsController()
        {
            _service = new AuditCycleDocumentService();
        }

        // END POINTS

        public IHttpActionResult GetAuditCycleDocuments([FromUri] AuditCycleDocumentQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = AuditCycleDocumentMapping.AuditCycleDocumentsToListDto(items);
            var response = new ApiResponse<IEnumerable<AuditCycleDocumentItemListDto>>(itemsDto)
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
        } // GetAuditCycleDocuments

        [ResponseType(typeof(ApiResponse<AuditCycleDocumentItemDetailDto>))]
        public async Task<IHttpActionResult> GetAuditCycleDocument(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = AuditCycleDocumentMapping.AuditCycleDocumentToItemDetailDto(item);
            var response = new ApiResponse<AuditCycleDocumentItemDetailDto>(itemDto);

            return Ok(response);
        } // GetAuditCycleDocument

        [HttpPost]
        [ResponseType(typeof(ApiResponse<AuditCycleDocumentItemDetailDto>))]
        public async Task<IHttpActionResult> PostAuditCycleDocument(AuditCycleDocumentPostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = AuditCycleDocumentMapping.ItemAddDtoToAuditCycleDocument(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = AuditCycleDocumentMapping.AuditCycleDocumentToItemDetailDto(item);
            var response = new ApiResponse<AuditCycleDocumentItemDetailDto>(itemDto);

            return Ok(response);
        } // PostAuditCycleDocument

        [HttpPut]
        [ResponseType(typeof(ApiResponse<AuditCycleDocumentItemDetailDto>))]
        public async Task<IHttpActionResult> PutAuditCycleDocument() // Guid id, [FromBody] AuditCycleDocumentPutDto itemEditDto)
        {
            var data = HttpContext.Current.Request.Params["data"];
            var file = HttpContext.Current.Request.Files.Count > 0
                ? HttpContext.Current.Request.Files[0]
                : null;
            string filename = null;

            AuditCycleDocumentPutDto itemEditDto = JsonConvert.DeserializeObject<AuditCycleDocumentPutDto>(data)
                ?? throw new BusinessException("Can't read data object");

            var item = await _service.GetAsync(itemEditDto.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (file != null)
            {
                var documentType = item.DocumentType ?? itemEditDto.DocumentType;

                if (documentType == AuditCycleDocumentType.Nothing)
                    throw new BusinessException("Document type is required");

                //var organizationId = item.AuditCycles.FirstOrDefault()?.OrganizationID
                //    ?? throw new BusinessException("Can't determine organization for file upload");

                filename = FileRepository.UploadFile(
                    file,
                    //$"~/files/organizations/{organizationId}/Cycles/{item.AuditCycle.ID}", // TODO: Ruta actual, cambiar por la no comentada, mover los archivos de cu
                    $"~/files/organizations/{item.OrganizationID}/cycles",
                    item.ID.ToString()
                );
            }

            var itemToEdit = AuditCycleDocumentMapping.ItemEditDtoToAuditCycleDocument(itemEditDto);

            // Si se subió un archivo, se actualizan los siguientes campos
            itemToEdit.Filename = filename ?? item.Filename;
            itemToEdit.UploadedBy = !string.IsNullOrEmpty(filename)
                ? itemToEdit.UpdatedUser
                : item.UploadedBy;

            item = await _service.UpdateAsync(itemToEdit);
            var itemDto = AuditCycleDocumentMapping.AuditCycleDocumentToItemDetailDto(item);
            var response = new ApiResponse<AuditCycleDocumentItemDetailDto>(itemDto);

            return Ok(response);
        } // PutAuditCycleDocument

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteAuditCycleDocument(Guid id, [FromBody] AuditCycleDocumentDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDeleteDto.ID)
                throw new BusinessException("ID mismatch");

            // - Validar si es eliminación definitiva, borrar el archivo físico

            var item = AuditCycleDocumentMapping.ItemDeleteDtoToAuditCycleDocument(itemDeleteDto);
            await _service.DeleteAsync(item);            
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteAuditCycleDocument

        // AUDIT CYCLES

        [HttpPost]
        [Route("api/AuditCycleDocuments/{id}/audit-cycle")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> AddAuditCycleAsync(Guid id, [FromBody] AuditCycleDocumentEditAuditCycleDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.AuditCycleDocumentID)
                throw new BusinessException("ID mismatch");

            await _service.AddAuditCycleAsync(itemDto.AuditCycleDocumentID, itemDto.AuditCycleID);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // AddAuditCycleAsync

        [HttpDelete]
        [Route("api/AuditCycleDocuments/{id}/audit-cycle")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> RemoveAuditCycleAsync(Guid id, [FromBody] AuditCycleDocumentEditAuditCycleDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.AuditCycleDocumentID)
                throw new BusinessException("ID mismatch");

            await _service.DelAuditCycleAsync(itemDto.AuditCycleDocumentID, itemDto.AuditCycleID);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // RemoveAuditCycleAsync
    }
}
