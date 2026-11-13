using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
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
using System.Threading.Tasks;
using System.Web;
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

        [HttpGet]
        [Route("api/contacts/proposal/{id}")]
        [ResponseType(typeof(ApiResponse<IEnumerable<ContactItemListDto>>))]
        public async Task<IHttpActionResult> GetContactsByProposalID(Guid id)
        {
            var items = await _contactService.GetsByProposalID(id);
            var itemsDto = ContactMapping.ContactToListDto(items);
            var response = new ApiResponse<IEnumerable<ContactItemListDto>>(itemsDto);

            return Ok(response);
        } // GetContactsByProposalID

        // GET: api/Contact/5
        [HttpGet]
        [ResponseType(typeof(ApiResponse<ContactItemDetailDto>))]
        public async Task<IHttpActionResult> GetContact(Guid id)
        {
            var item = await _contactService.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = await ContactMapping.ContactToItemDetailDto(item);
            var response = new ApiResponse<ContactItemDetailDto>(itemDto);

            return Ok(response);
        } // GetContact

        // POST: api/Contact
        [ResponseType(typeof(ApiResponse<ContactItemDetailDto>))]
        public async Task<IHttpActionResult> PostContact([FromBody] ContactPostDto contactDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var itemToCreate = ContactMapping.ItemCreateDtoToContact(contactDto);
            var item = await _contactService.CreateAsync(itemToCreate);
            var itemDto = await ContactMapping.ContactToItemDetailDto(item);
            var response = new ApiResponse<ContactItemDetailDto>(itemDto);

            return Ok(response);
        } // PostContact

        [HttpPut]
        [ResponseType(typeof(ApiResponse<ContactItemDetailDto>))]
        public async Task<IHttpActionResult> PutAuditor()
        {
            var data = HttpContext.Current.Request.Params["data"];
            var file = HttpContext.Current.Request.Files.Count > 0
                ? HttpContext.Current.Request.Files[0]
                : null;
            string filename = null;

            ContactPutDto itemEditDto = JsonConvert.DeserializeObject<ContactPutDto>(data)
                ?? throw new BusinessException("Can't read data object");

            var item = await _contactService.GetAsync(itemEditDto.ID)
                ?? throw new BusinessException("The record to update was not found");

            if (file != null)
            {
                filename = FileRepository.UploadFile(file, GetContactURL(item), item.ID.ToString());
            }

            var itemToEdit = ContactMapping.ItemEditDtoToContact(itemEditDto);
            itemToEdit.PhotoFilename = filename ?? item.PhotoFilename;
            item = await _contactService.UpdateAsync(itemToEdit);
            var itemDto = await ContactMapping.ContactToItemDetailDto(item);
            var response = new ApiResponse<ContactItemDetailDto>(itemDto);

            return Ok(response);
        } // PutAuditor

        [HttpDelete]
        [Route("api/Contacts/{id}/Photofile")]
        public async Task<IHttpActionResult> DeletePhotofile(Guid id, [FromBody] ContactDeleteDto itemDelDto)
        {
            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = await _contactService.GetAsync(id)
                ?? throw new BusinessException("Record to delete file not found");

            if (FileRepository.DeleteFile(GetContactURL(item), item.PhotoFilename))
            {
                item.PhotoFilename = null;
                item.UpdatedUser = itemDelDto.UpdatedUser;
                await _contactService.UpdateAsync(item);
            }

            return Ok(new ApiResponse<bool>(true));
        } // DeletePhotofile

        // DELETE: api/Contact/5
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteContact(Guid id, [FromBody] ContactDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var foundItem = await _contactService.GetAsync(id)
                ?? throw new BusinessException("Contact to delete not found");

            if (foundItem.Status == StatusType.Deleted && !string.IsNullOrEmpty(foundItem.PhotoFilename))
            {
                FileRepository.DeleteFile(GetContactURL(foundItem), foundItem.PhotoFilename);
            }

            var item = ContactMapping.ItemDeleteDtoToContact(itemDelDto);
            await _contactService.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            //TODO: Si el Status es Delete borrar el archivo fisico

            return Ok(response);
        } // DeleteContact

        // PRIVATE

        private string GetContactURL(Contact item)
        {
            return $"~/files/organizations/{item.OrganizationID}/contacts/{item.ID}";
        } // GetContactURL
    }
}
