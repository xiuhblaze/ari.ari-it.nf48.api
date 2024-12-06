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
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class AuditorsController : ApiController
    {
        private readonly AuditorService _auditorService;

        // CONSTRUCTOR

        public AuditorsController()
        {
            _auditorService = new AuditorService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<AuditorItemListDto>>))]
        public IHttpActionResult GetAuditors([FromUri] AuditorQueryFilters filters)
        {
            var items = _auditorService.Gets(filters);
            var itemsDto = AuditorMapping.AuditorToListDto(items);
            var response = new ApiResponse<IEnumerable<AuditorItemListDto>>(itemsDto)
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
        } // GetAuditors

        [ResponseType(typeof(ApiResponse<AuditorItemDetailDto>))]
        public async Task<IHttpActionResult> GetAuditor(Guid id)
        {
            var item = await _auditorService.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = AuditorMapping.AuditorToItemDetailDto(item);
            var response = new ApiResponse<AuditorItemDetailDto>(itemDto);

            return Ok(response);
        } // GetAuditor

        [ResponseType(typeof(ApiResponse<AuditorItemDetailDto>))]
        public async Task<IHttpActionResult> PostAuditor([FromBody] AuditorPostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = AuditorMapping.ItemAddDtoToAuditor(itemPostDto);
            item = await _auditorService.AddAsync(item);
            var itemDto = AuditorMapping.AuditorToItemDetailDto(item);
            var response = new ApiResponse<AuditorItemDetailDto>(itemDto);

            return Ok(response);
        } // PostAuditor

        //// PUT: api/Auditorss
        //[ResponseType(typeof(ApiResponse<AuditorItemDetailDto>))]
        //public async Task<IHttpActionResult> PutAuditor(Guid id, [FromBody] AuditorPutDto itemEditDto)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BusinessException(Strings.GetModelStateErrors(ModelState));

        //    if (id != itemEditDto.ID)
        //        throw new BusinessException("ID mismatch");

        //    var item = AuditorMapping.ItemEditDtoToAuditor(itemEditDto);
        //    item = await _auditorService.UpdateAsync(item);
        //    var itemDto = AuditorMapping.AuditorToItemDetailDto(item);
        //    var response = new ApiResponse<AuditorItemDetailDto>(itemDto);

        //    return Ok(response);
        //} // PutAuditor

        //[ResponseType(typeof(ApiResponse<AuditorItemDetailDto>))]
        //public async Task<IHttpActionResult> PutAuditor()
        //{
        //    if (!Request.Content.IsMimeMultipartContent())
        //        throw new BusinessException("Unssuported media type");

        //    // Obteniendo los datos recibidos

        //    var tmpPath = System.Web.Hosting.HostingEnvironment.MapPath("~/files/tmp");
        //    if (!Directory.Exists(tmpPath))
        //        Directory.CreateDirectory(tmpPath);
        //    var provider = new MultipartFormDataStreamProvider(tmpPath); // Guardando de forma temporal los archivos
        //    await Request.Content.ReadAsMultipartAsync(provider);

        //    AuditorPutDto itemEditDto = null;
        //    string newFilename = null;

        //    foreach (var formData in provider.FormData.AllKeys)
        //    {
        //        if (formData == "data")
        //        {
        //            itemEditDto = JsonConvert.DeserializeObject<AuditorPutDto>(provider.FormData[formData]);
        //        }
        //    }

        //    if (itemEditDto == null)
        //        throw new BusinessException("Can't read data object");

        //    foreach (var file in provider.FileData)
        //    {
        //        var originalFilename = file.Headers.ContentDisposition.FileName.Trim('"');
        //        var extension = originalFilename != null
        //            ? Path.GetExtension(originalFilename)
        //            : null;
        //        var uploadedFilePath = file.LocalFileName;
        //        var auditorPath = System.Web.Hosting.HostingEnvironment.MapPath($"~/files/auditors/{itemEditDto.ID.ToString()}");
        //        if (!Directory.Exists(auditorPath))
        //            Directory.CreateDirectory(auditorPath);
        //        newFilename = "PhotoProfile" + extension;
        //        var newFilePath = Path.Combine(auditorPath, newFilename);

        //        if (File.Exists(newFilePath))
        //            File.Delete(newFilePath);

        //        File.Move(uploadedFilePath, newFilePath);
        //    }

        //    // Procesando los datos
        //    var item = await _auditorService.GetAsync(itemEditDto.ID)
        //            ?? throw new BusinessException("The record to update was not found");

        //    var itemToEdit = AuditorMapping.ItemEditDtoToAuditor(itemEditDto);
        //    itemToEdit.PhotoFilename = newFilename ?? item.PhotoFilename;
        //    item = await _auditorService.UpdateAsync(itemToEdit);
        //    var itemDto = AuditorMapping.AuditorToItemDetailDto(item);
        //    var response = new ApiResponse<AuditorItemDetailDto>(itemDto);

        //    return Ok(response);

        //} // PutAuditor

        [HttpPut]
        [ResponseType(typeof(ApiResponse<AuditorItemDetailDto>))]
        public async Task<IHttpActionResult> PutAuditor()
        {
            var data = HttpContext.Current.Request.Params["data"];
            var file = HttpContext.Current.Request.Files.Count > 0
                ? HttpContext.Current.Request.Files[0]
                : null;
            string filename = null;

            AuditorPutDto itemEditDto = JsonConvert.DeserializeObject<AuditorPutDto>(data)
                ?? throw new BusinessException("Can't read data object");

            var item = await _auditorService.GetAsync(itemEditDto.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (file != null)
            {
                filename = FileRepository.UploadFile(
                    file,
                    $"~/files/auditors/{item.ID}",
                    item.ID.ToString()
                );
            }

            var itemToEdit = AuditorMapping.ItemEditDtoToAuditor(itemEditDto);
            itemToEdit.PhotoFilename = filename ?? item.PhotoFilename;
            item = await _auditorService.UpdateAsync(itemToEdit);
            var itemDto = AuditorMapping.AuditorToItemDetailDto(item);
            var response = new ApiResponse<AuditorItemDetailDto>(itemDto);

            return Ok(response);
        } // PutAuditor

        [HttpDelete]
        [Route("api/Auditors/{id}/Photofile")]
        public async Task<IHttpActionResult> DeletePhotofile(Guid id, [FromBody] AuditorDeleteDto itemDelDto)
        {
            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = await _auditorService.GetAsync(id)
                ?? throw new BusinessException("Record to delete file not found");

            if (FileRepository.DeleteFile($"~/files/auditors/{item.ID}", item.PhotoFilename))
            {
                item.PhotoFilename = null;
                item.UpdatedUser = itemDelDto.UpdatedUser;
                await _auditorService.UpdateAsync(item);
            }

            return Ok(new ApiResponse<bool>(true));
        } // DeletePhotofile

        // DELETE: api/Auditor/5
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteAuditor(Guid id, [FromBody] AuditorDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            //TODO: Si va en status de Eliminar, borrar el archivo fisico

            var item = AuditorMapping.ItemDeleteDtoToAuditor(itemDelDto);
            await _auditorService.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteContact
    }
}
