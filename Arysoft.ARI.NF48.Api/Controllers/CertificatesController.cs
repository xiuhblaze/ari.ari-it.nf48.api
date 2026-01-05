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
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class CertificatesController : ApiController
    {
        private const string CERTIFICATE_FILENAME = "certificate";
        private const string QR_FILENAME = "qrcode";

        private readonly CertificateService _service;

        // CONSTRUCTOR

        public CertificatesController()
        {
            _service = new CertificateService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<CertificateItemListDto>>))]
        public IHttpActionResult GetCertificates([FromUri] CertificateQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = CertificateMapping.CertificatesToListDto(items);
            var response = new ApiResponse<IEnumerable<CertificateItemListDto>>(itemsDto)
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
        } // GetCertificates

        [ResponseType(typeof(ApiResponse<CertificateItemDetailDto>))]
        public async Task<IHttpActionResult> GetCertificate(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = await CertificateMapping.CertificateToItemDetailDto(item);
            var response = new ApiResponse<CertificateItemDetailDto>(itemDto);

            return Ok(response);
        } // GetCertificate

        [HttpPost]
        [ResponseType(typeof(ApiResponse<CertificateItemDetailDto>))]
        public async Task<IHttpActionResult> PostCertificate([FromBody] CertificatePostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = CertificateMapping.ItemAddDtoToCertificate(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = await CertificateMapping.CertificateToItemDetailDto(item);
            var response = new ApiResponse<CertificateItemDetailDto>(itemDto);

            return Ok(response);
        } // PostCertificate

        [HttpPut]
        [ResponseType(typeof(ApiResponse<CertificateItemDetailDto>))]
        public async Task<IHttpActionResult> PutCertificate()
        {
            var data = HttpContext.Current.Request.Params["data"];
            var files = HttpContext.Current.Request.Files.Count > 0
                ? HttpContext.Current.Request.Files
                : null;

            CertificatePutDto itemEditDto = JsonConvert.DeserializeObject<CertificatePutDto>(data)
                ?? throw new BusinessException("Can't read data object");

            var item = await _service.GetAsync(itemEditDto.ID)
                ?? throw new BusinessException("The record to update was not found");

            var itemToEdit = CertificateMapping.ItemEditDtoToCertificate(itemEditDto);

            if (files != null)
            {
                HttpPostedFile certificateFile = null;
                HttpPostedFile qrFile = null;

                for (int i = 0; i < files.Count; i++) // Ver que archivos se subieron
                {
                    HttpPostedFile file = files[i];
                    string fileNameWithoutExtension = Path
                        .GetFileNameWithoutExtension(file.FileName);

                    if (fileNameWithoutExtension == CERTIFICATE_FILENAME) certificateFile = file;
                    if (fileNameWithoutExtension == QR_FILENAME) qrFile = file;
                }

                if (certificateFile != null) // Si se subió un archivo de certificado
                {
                    itemToEdit.Filename = FileRepository.UploadFile(
                        certificateFile,
                        $"~/files/organizations/{item.OrganizationID}/certificates",
                        item.ID.ToString() + "_" + CERTIFICATE_FILENAME,
                        new string[] { ".jpg", ".png", ".pdf" }
                    );
                }
                else itemToEdit.Filename = item.Filename;

                if (qrFile != null) // Si se subió un archivo del código QR
                {
                    itemToEdit.QRFile = FileRepository.UploadFile(
                        qrFile,
                        $"~/files/organizations/{item.OrganizationID}/certificates",
                        item.ID.ToString() + "_" + QR_FILENAME,
                        new string[] { ".jpg", ".png" }
                    );
                }
                else itemToEdit.QRFile = item.QRFile;

                //itemToEdit.Filename = FileRepository.UploadFile(
                //    file,
                //    $"~/files/organizations/{item.OrganizationID}/certificates",
                //    item.ID.ToString()
                //);
            }
            else
            { 
                itemToEdit.Filename = item.Filename;
                itemToEdit.QRFile = item.QRFile;
            }

            itemToEdit = await _service.UpdateAsync(itemToEdit);
            var itemDto = await CertificateMapping.CertificateToItemDetailDto(itemToEdit);
            var response = new ApiResponse<CertificateItemDetailDto>(itemDto);

            return Ok(response);
        } // PutCertificate

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteCertificate(Guid id, [FromBody] CertificateDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = CertificateMapping.ItemDeleteDtoToCertificate(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteCertificate
    }
}
