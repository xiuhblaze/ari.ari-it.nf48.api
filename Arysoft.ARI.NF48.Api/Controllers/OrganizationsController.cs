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
    public class OrganizationsController : ApiController
    {
        private const string LOGO_FILENAME = "logotype";
        // private const string QR_FILENAME = "qrcode";

        private readonly OrganizationService _organizationService;

        // CONSTRUCTOR 

        public OrganizationsController()
        {
            _organizationService = new OrganizationService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<OrganizationItemListDto>>))]
        public IHttpActionResult GetOrganizations([FromUri] OrganizationQueryFilters filters)
        {
            var items = _organizationService.Gets(filters);
            var itemsDto = OrganizationMapping.OrganizationToListDto(items);
            var response = new ApiResponse<IEnumerable<OrganizationItemListDto>>(itemsDto)
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
        } // GetOrganizations

        [ResponseType(typeof(ApiResponse<OrganizationItemDetailDto>))]
        public async Task<IHttpActionResult> GetOrganization(Guid id)
        {
            var item = await _organizationService.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = OrganizationMapping.OrganizationToItemDetailDto(item);
            var response = new ApiResponse<OrganizationItemDetailDto>(itemDto);

            return Ok(response);
        } // GetOrganization

        // POST: api/Organization
        [ResponseType(typeof(ApiResponse<OrganizationItemDetailDto>))]
        public async Task<IHttpActionResult> PostOrganization([FromBody] OrganizationPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var itemToAdd = OrganizationMapping.ItemAddDtoToOrganization(itemAddDto);
            var item = await _organizationService.AddAsync(itemToAdd);
            var itemDto = OrganizationMapping.OrganizationToItemDetailDto(item);
            var response = new ApiResponse<OrganizationItemDetailDto>(itemDto);

            return Ok(response);
        } // PostOrganization

        // PUT: api/Organization/5
        [ResponseType(typeof(ApiResponse<OrganizationItemDetailDto>))]
        public async Task<IHttpActionResult> PutOrganization()
        {
            var data = HttpContext.Current.Request.Params["data"];
            var files = HttpContext.Current.Request.Files.Count > 0
                ? HttpContext.Current.Request.Files
                : null;

            OrganizationPutDto itemEditDto = JsonConvert.DeserializeObject<OrganizationPutDto>(data)
                ?? throw new BusinessException("Can't read data object");

            var item = await _organizationService.GetAsync(itemEditDto.ID)
                ?? throw new BusinessException("The record to update was not found");

            var itemToEdit = OrganizationMapping.ItemEditDtoToOrganization(itemEditDto);

            if (files != null)
            {
                HttpPostedFile logoFile = null;
                // HttpPostedFile qrFile = null;

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile file = files[i];
                    string fileNameWithoutExtension = Path
                        .GetFileNameWithoutExtension(file.FileName);

                    if (fileNameWithoutExtension == LOGO_FILENAME) logoFile = file;
                    // if (fileNameWithoutExtension == QR_FILENAME) qrFile = file;
                }

                if (logoFile != null)
                {
                    itemToEdit.LogoFile = FileRepository.UploadFile(
                        logoFile,
                        $"~/files/organizations/{itemEditDto.ID}",
                        LOGO_FILENAME,
                        new string[] { ".jpg", ".png" }
                    );
                }
                else itemToEdit.LogoFile = item.LogoFile;

                //if (qrFile != null)
                //{
                //    itemToEdit.QRFile = FileRepository.UploadFile(
                //        qrFile,
                //        $"~/files/organizations/{itemEditDto.ID}",
                //        QR_FILENAME,
                //        new string[] { ".jpg", ".png" }
                //    );
                //}
                //else itemToEdit.QRFile = item.QRFile;
            }
            else
            {
                itemToEdit.LogoFile = item.LogoFile;
                // itemToEdit.QRFile = item.QRFile;
            }

            item = await _organizationService.UpdateAsync(itemToEdit);
            var itemDto = OrganizationMapping.OrganizationToItemDetailDto(item);
            var response = new ApiResponse<OrganizationItemDetailDto>(itemDto);

            return Ok(response);
        } // PutOrganization

        //// PUT: api/Organization/5
        //[ResponseType(typeof(ApiResponse<OrganizationItemDetailDto>))]
        //public async Task<IHttpActionResult> PutOrganization(Guid id, [FromBody] OrganizationPutDto itemEditDto)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BusinessException(Strings.GetModelStateErrors(ModelState));

        //    if (id != itemEditDto.ID)
        //        throw new BusinessException("ID mismatch");

        //    var itemToEdit = OrganizationMapping.ItemEditDtoToOrganization(itemEditDto);
        //    var item = await _organizationService.UpdateAsync(itemToEdit);
        //    var itemDto = OrganizationMapping.OrganizationToItemDetailDto(item);
        //    var response = new ApiResponse<OrganizationItemDetailDto>(itemDto);

        //    return Ok(response);
        //} // PutOrganization

        // DELETE: api/Organization/5
        public async Task<IHttpActionResult> DeleteOrganization(Guid id, [FromBody] OrganizationDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = OrganizationMapping.ItemDeleteDtoToOrganization(itemDelDto);
            await _organizationService.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteOrganization

    }
}
