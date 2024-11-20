using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Exceptions;
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class ContactsController : ApiController
    {
        // private readonly AriContext db = new AriContext();
        private readonly ContactService _contactService;

        // CONSTRUCTOR 

        public ContactsController()
        {
            _contactService = new ContactService();
        } // ContactsController

        // ENDPOINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<ContactItemListDto>>))]
        public IHttpActionResult GetContacts([FromUri] ContactQueryFilters filters)
        {
            var items = _contactService.Gets(filters);
            var itemsDto = ContactMapping.ContactToListDto(items);
            var response = new ApiResponse<IEnumerable<ContactItemListDto>>(itemsDto)
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
        } // GetContacts

        [ResponseType(typeof(ApiResponse<ContactItemDetailDto>))]
        public async Task<IHttpActionResult> GetContact(Guid id)
        {
            var item = await _contactService.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = ContactMapping.ContactToItemDetailDto(item);
            var response = new ApiResponse<ContactItemDetailDto>(itemDto);

            return Ok(response);
        } // GetContact

        // POST: api/Contact
        [ResponseType(typeof(ApiResponse<ContactItemDetailDto>))]
        public async Task<IHttpActionResult> PostContact([FromBody] ContactPostDto contactDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var itemToAdd = ContactMapping.ItemAddDtoToContact(contactDto);
            var item = await _contactService.AddAsync(itemToAdd);
            var itemDto = ContactMapping.ContactToItemDetailDto(item);
            var response = new ApiResponse<ContactItemDetailDto>(itemDto);

            return Ok(response);
        } // PostContact

        // PUT: api/Contact/5
        [ResponseType(typeof(ApiResponse<ContactItemDetailDto>))]
        public async Task<IHttpActionResult> PutContact(Guid id, [FromBody] ContactPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var itemToEdit = ContactMapping.ItemEditDtoToContact(itemEditDto);
            var item = await _contactService.UpdateAsync(itemToEdit);
            var itemDto = ContactMapping.ContactToItemDetailDto(item);
            var response = new ApiResponse<ContactItemDetailDto>(itemDto);

            return Ok(response);
        } // PutContact

        [HttpPut]
        [Route("api/contacts/contact-with-file")]
        [ResponseType(typeof(ApiResponse<ContactItemDetailDto>))]
        public async Task<IHttpActionResult> PutWithFileContact()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new BusinessException("Unssuported media type");

            try
            {
                string uploadPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Files/Contacts");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var provider = new MultipartFormDataStreamProvider(uploadPath);
                await Request.Content.ReadAsMultipartAsync(provider);

                ContactPutDto itemEditDto = null;
                string newFileName = null;

                foreach (var formData in provider.FormData.AllKeys)
                {
                    if (formData == "data")
                    {
                        // Deserializa los datos del DTO desde el campo "data"
                        itemEditDto = JsonConvert.DeserializeObject<ContactPutDto>(provider.FormData[formData]);
                    }
                }

                if (itemEditDto == null)
                    throw new BusinessException("Can't read data object");

                foreach (var file in provider.FileData)
                {
                    var originalFileName = file.Headers.ContentDisposition.FileName.Trim('"');
                    var extension = originalFileName != null ? Path.GetExtension(originalFileName) : null;
                    var uploadedFilePath = file.LocalFileName;

                    newFileName = itemEditDto.ID.ToString() + extension;

                    var newFilePath = Path.Combine(uploadPath, newFileName);

                    if (File.Exists(newFilePath))
                        File.Delete(newFilePath); // Eliminar el archivo destino existente

                    File.Move(uploadedFilePath, newFilePath);
                }

                var itemToEdit = ContactMapping.ItemEditDtoToContact(itemEditDto);
                //itemToEdit.PhotoFilename = !string.IsNullOrEmpty(newFileName) ? newFileName : itemToEdit.PhotoFilename;
                itemToEdit.PhotoFilename = newFileName ?? itemToEdit.PhotoFilename;
                var item = await _contactService.UpdateAsync(itemToEdit);
                var itemDto = ContactMapping.ContactToItemDetailDto(item);
                var response = new ApiResponse<ContactItemDetailDto>(itemDto);

                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Exception: " + ex.Message);
            }

        } // PutWithFileContact

        // DELETE: api/Contact/5
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteContact(Guid id, [FromBody] ContactDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = ContactMapping.ItemDeleteDtoToContact(itemDelDto);
            await _contactService.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteContact

    }
}
