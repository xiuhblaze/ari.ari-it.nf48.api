using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Data;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class ContactsController : ApiController
    {
        private AriContext db = new AriContext();

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<Contact>>))]
        public IHttpActionResult GetContacts([FromUri] ContactQueryFilters filters)
        {
            var items = db.Contacts.AsEnumerable();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            { 
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(e =>
                    e.FirstName.ToLower().Contains(filters.Text)
                    || e.LastName.ToLower().Contains(filters.Text)
                    || e.Phone.ToLower().Contains(filters.Text)
                    || e.Position.ToLower().Contains(filters.Text)
                );
            }

            //TODO: Agregar el filtro por OrganizationID

            if (filters.Status != null && filters.Status != StatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != StatusType.Nothing)
                    : items.Where(e => e.Status != StatusType.Nothing && e.Status != StatusType.Deleted);
            }

            // Orden

            switch (filters.Order)
            {
                case ContactOrderType.FirstName:
                    items = items.OrderBy(e => e.FirstName);
                    break;
                case ContactOrderType.LastName:
                    items = items.OrderBy(e => e.LastName);
                    break;
                case ContactOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case ContactOrderType.FirstNameDesc:
                    items = items.OrderByDescending(e => e.FirstName);
                    break;
                case ContactOrderType.LastNameDesc:
                    items = items.OrderByDescending(e => e.LastName);
                    break;
                case ContactOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.FirstName);
                    break;
            }

            // Pagination

            var pagedContacts = PagedList<Contact>.Create(items, filters.PageNumber, filters.PageSize);
            var response = new ApiResponse<IEnumerable<Contact>>(pagedContacts);
            var metadata = new Metadata
            {
                TotalCount = pagedContacts.TotalCount,
                PageSize = pagedContacts.PageSize,
                CurrentPage = pagedContacts.CurrentPage,
                TotalPages = pagedContacts.TotalPages,
                HasPreviousPage = pagedContacts.HasPreviousPage,
                HasNextPage = pagedContacts.HasNextPage
            };
            response.Meta = metadata;

            return Ok(response);
        } // GetContacts

        [ResponseType(typeof(ApiResponse<Contact>))]
        public async Task<IHttpActionResult> GetContact(Guid id)
        {
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            
            var response = new ApiResponse<Contact>(contact);
            return Ok(response);
        } // GetContact

        // POST: api/Contact
        [ResponseType(typeof(ApiResponse<Contact>))]
        public async Task<IHttpActionResult> PostContact([FromBody] ContactPostDto contactDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            await DeleteTmpByUserAsync(contactDto.UpdatedUser);

            var item = new Contact { 
                ID = Guid.NewGuid(),
                OrganizationID = contactDto.OrganizationID,
                Status = StatusType.Nothing,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                UpdatedUser = contactDto.UpdatedUser
            };

            db.Contacts.Add(item);

            try {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var response = new ApiResponse<Contact>(item);
            return Ok(response);
        } // PostContact

        // PUT: api/Contact/5
        [ResponseType(typeof(ApiResponse<Contact>))]
        public async Task<IHttpActionResult> PutContact(Guid id, [FromBody] ContactPutDto contactDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (id != contactDto.ID) { return BadRequest("ID mismatch"); }

            var item = await db.Contacts.FindAsync(id);

            if (item == null) return NotFound();

            item.FirstName = contactDto.FirstName;
            item.LastName = contactDto.LastName;
            item.Phone = contactDto.Phone;
            item.PhoneExtensions = contactDto.PhoneExtensions;
            item.Email = contactDto.Email;
            item.Position = contactDto.Position;
            item.Status = contactDto.Status == StatusType.Nothing ? StatusType.Active : contactDto.Status;
            item.Updated = DateTime.Now;
            item.UpdatedUser = contactDto.UpdatedUser;

            db.Entry(item).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            { 
                return BadRequest(ex.Message);
            }

            var response = new ApiResponse<Contact>(item);
            return Ok(response);
        } // PutContact

        // DELETE: api/Contact/5
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteContact(Guid id)
        {
            var contact = await db.Contacts.FindAsync(id);
            if (contact == null) return NotFound();
            
            if (contact.Status == StatusType.Deleted)
            {
                db.Contacts.Remove(contact);
            }
            else
            {
                contact.Status = contact.Status == StatusType.Active ? StatusType.Inactive : StatusType.Deleted;
                db.Entry(contact).State = EntityState.Modified;
            }

            await db.SaveChangesAsync();

            var response = new ApiResponse<bool>(true);
            return Ok(response);
        } // DeleteContact

        // PRIVATED

        private async Task<bool> ContactExistAsync(Guid id)
        {
            return await db.Contacts.AnyAsync(c => c.ID == id);
        }

        private async Task DeleteTmpByUserAsync(string username)
        {
            var items = await db.Contacts
                .Where(c => c.UpdatedUser == username && c.Status == StatusType.Nothing)
                .ToListAsync();

            foreach (var item in items)
            {
                db.Entry(item).State = EntityState.Deleted;
            }
        }
    }
}
