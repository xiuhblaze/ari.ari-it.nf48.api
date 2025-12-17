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
    public class AuditDocumentsController : ApiController
    {
        private readonly AuditDocumentService _service;

        // CONSTRUCTOR

        public AuditDocumentsController()
        {
            _service = new AuditDocumentService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<AuditDocumentItemListDto>>))]
        public IHttpActionResult GetAuditDocuments([FromUri] AuditDocumentQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = AuditDocumentMapping.AuditDocumentToListDto(items);
            var response = new ApiResponse<IEnumerable<AuditDocumentItemListDto>>(itemsDto)
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
        } // GetAuditDocuments

        [HttpGet]
        [ResponseType(typeof(ApiResponse<AuditDocumentItemDetailDto>))]
        public async Task<IHttpActionResult> GetAuditDocument(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = AuditDocumentMapping.AuditDocumentToItemDetailDto(item);
            var response = new ApiResponse<AuditDocumentItemDetailDto>(itemDto);

            return Ok(response);
        } // GetAuditDocument

        [HttpPost]
        [ResponseType(typeof(ApiResponse<AuditDocumentItemDetailDto>))]
        public async Task<IHttpActionResult> PostAuditDocument(AuditDocumentPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = AuditDocumentMapping.ItemAddDtoToAuditDocument(itemAddDto);
            item = await _service.AddAsync(item);
            var itemDto = AuditDocumentMapping.AuditDocumentToItemDetailDto(item);
            var response = new ApiResponse<AuditDocumentItemDetailDto>(itemDto);

            return Ok(response);
        } // PostAuditDocument

        [HttpPut]
        [ResponseType(typeof(ApiResponse<AuditDocumentItemDetailDto>))]
        public async Task<IHttpActionResult> PutAuditDocument()
        {
            var data = HttpContext.Current.Request.Params["data"];
            var file = HttpContext.Current.Request.Files.Count > 0
                ? HttpContext.Current.Request.Files[0]
                : null;
            string fileName = null;

            AuditDocumentPutDto itemEditDto = JsonConvert.DeserializeObject<AuditDocumentPutDto>(data)
                ?? throw new BusinessException("Can't read data object");

            var item = await _service.GetAsync(itemEditDto.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (file != null)
            {
                var documentType = item.DocumentType ?? itemEditDto.DocumentType;

                if (documentType == null ||documentType == AuditDocumentType.Nothing)
                    throw new BusinessException("DocumentType is required");

                //fileName = FileRepository.UploadFile(
                //    file,
                //    $"~/files/organizations/{ item.Audit.AuditCycle.OrganizationID }/Cycles/{item.Audit.AuditCycle.ID}/{item.Audit.ID}",
                //    documentType.ToString() + "_" + item.ID.ToString()
                //);
                fileName = FileRepository.UploadFile(
                    file,
                    $"~/files/organizations/{item.Audit.OrganizationID}/audits/{item.Audit.ID}",
                    documentType.ToString() + "_" + item.ID.ToString()
                );
            }

            var itemToEdit = AuditDocumentMapping.ItemEditDtoToAuditDocument(itemEditDto);
            
            // Si se subió un archivo, se actualizan los siguientes campos
            itemToEdit.Filename = fileName ?? item.Filename;
            itemToEdit.UploadedBy = !string.IsNullOrEmpty(fileName)
                ? itemToEdit.UpdatedUser
                : item.UploadedBy;

            item = await _service.UpdateAsync(itemToEdit);
            var itemDto = AuditDocumentMapping.AuditDocumentToItemDetailDto(item);
            var response = new ApiResponse<AuditDocumentItemDetailDto>(itemDto);

            return Ok(response);
        } // PutAuditDocument

        public async Task<IHttpActionResult> DeleteAuditDocument(Guid id, [FromBody] AuditDocumentDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            // Si se va a eliminar el registro, borrar el archivo físico

            var item = AuditDocumentMapping.ItemDeleteDtoToAuditDocument(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteAuditDocument

        // AUDIT STANDARDS

        [HttpPost]
        [Route("api/AuditDocuments/{id}/audit-standard")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> AddAuditStandard(Guid id, [FromBody] AuditDocumentEditAuditStandardDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.AuditDocumentID)
                throw new BusinessException("ID mismatch");

            await _service.AddAuditStandardAsync(itemDto.AuditDocumentID, itemDto.AuditStandardID);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // AddAuditStandard

        [HttpDelete]
        [Route("api/AuditDocuments/{id}/audit-standard")]
        public async Task<IHttpActionResult> DelAuditStandard(Guid id, [FromBody] AuditDocumentEditAuditStandardDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.AuditDocumentID)
                throw new BusinessException("ID mismatch");

            await _service.DelAuditStandardAsync(itemDto.AuditDocumentID, itemDto.AuditStandardID);
            var response = new ApiResponse<bool>(true);
            
            return Ok(response);
        } // DelAuditStandard
    }
}
