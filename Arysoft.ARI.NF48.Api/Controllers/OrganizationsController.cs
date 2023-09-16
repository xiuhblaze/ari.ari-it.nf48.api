using Arysoft.ARI.NF48.Api.CustomEntities;
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
    public class OrganizationsController : ApiController
    {
        private AriContext db = new AriContext();

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<Organization>>))]
        public IHttpActionResult GetOrganizations([FromUri] OrganizationQueryFilters filters)
        { 
            var items = db.Organizations.AsEnumerable();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            { 
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => 
                    e.Name.ToLower().Contains(filters.Text)
                    || e.LegalEntity.ToLower().Contains(filters.Text)
                    || e.Website.ToLower().Contains(filters.Text)
                    || e.Phone.ToLower().Contains(filters.Text)
                );
            }

            if (filters.Status != null && filters.Status != OrganizationStatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != OrganizationStatusType.Nothing)
                    : items.Where(e => e.Status != OrganizationStatusType.Nothing && e.Status != OrganizationStatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            {
                case OrganizationOrderType.Name:
                    items = items.OrderBy(e => e.Name);
                    break;
                case OrganizationOrderType.LegalEntity:
                    items = items.OrderBy(e => e.LegalEntity);
                    break;
                case OrganizationOrderType.Status:
                    items = items.OrderBy(e => e.Status)
                        .ThenBy(e => e.Name);
                    break;
                case OrganizationOrderType.NameDesc:
                    items = items.OrderByDescending(e => e.Name);
                    break;
                case OrganizationOrderType.LegalEntityDesc:
                    items = items.OrderByDescending(e => e.LegalEntity);
                    break;
                case OrganizationOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status)
                        .ThenByDescending(e => e.Name);
                    break;
                default:
                    items = items.OrderBy(e => e.Name);
                    break;
            }

            // Pagination

            var pagedItems = PagedList<Organization>.Create(items, filters.PageNumber, filters.PageSize);
            var response = new ApiResponse<IEnumerable<Organization>>(pagedItems);
            var metadata = new Metadata
            {
                TotalCount = pagedItems.TotalCount,
                PageSize = pagedItems.PageSize,
                CurrentPage = pagedItems.CurrentPage,
                TotalPages = pagedItems.TotalPages,
                HasPreviousPage = pagedItems.HasPreviousPage,
                HasNextPage = pagedItems.HasNextPage
            };
            response.Meta = metadata;

            return Ok(response);
        } // GetOrganizations

        [ResponseType(typeof(ApiResponse<Organization>))]
        public async Task<IHttpActionResult> GetOrganization(Guid id)
        {
            var item = await db.Organizations.FindAsync(id);
            if (item == null) return NotFound(); 

            var response = new ApiResponse<Organization>(item);
            return Ok(response);
        } // GetOrganization

        // POST: api/Organization
        [ResponseType(typeof(ApiResponse<Organization>))]
        public async Task<IHttpActionResult> PostOrganization([FromBody] OrganizationPostDto organizationDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            await DeleteTmpByUserAsync(organizationDto.UpdatedUser);

            var item = new Organization { 
                OrganizationID = Guid.NewGuid(),
                Status = OrganizationStatusType.Nothing,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                UpdatedUser = organizationDto.UpdatedUser
            };

            db.Organizations.Add(item);

            try 
            { 
                await db.SaveChangesAsync();
            } 
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var response = new ApiResponse<Organization>(item);
            return Ok(response);
        } // PostOrganization

        // PUT: api/Organization/5
        [ResponseType(typeof(ApiResponse<Organization>))]
        public async Task<IHttpActionResult> PutOrganization(Guid id, [FromBody] OrganizationPutDto organizationDto)
        { 
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != organizationDto.OrganizationID) return BadRequest("ID mismatch.");

            var item = await db.Organizations.FindAsync(id);

            if (item == null) return NotFound();

            item.Name = organizationDto.Name;
            item.LegalEntity = organizationDto.LegalEntity;
            item.LogoFile = organizationDto.LogoFile;
            item.Website = organizationDto.Website;
            item.Phone = organizationDto.Phone;
            item.Status = organizationDto.Status == OrganizationStatusType.Nothing ? OrganizationStatusType.New : organizationDto.Status;
            item.Updated = DateTime.Now;
            item.UpdatedUser = organizationDto.UpdatedUser;

            db.Entry(item).State = EntityState.Modified;

            try 
            {
                await db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var response = new ApiResponse<Organization>(item);
            return Ok(response);
        } // PutOrganization


        public async Task<IHttpActionResult> DeleteOrganization(Guid id)
        {
            var item = await db.Organizations.FindAsync(id);

            // The item with the specified ID, does not exist.
            if (item == null) return NotFound();

            if (item.Status == OrganizationStatusType.Deleted)
            {
                // TODO: Validar que no tenga Sites o Contacts asociados (eliminarlos o no dejarlo borrar)
                db.Organizations.Remove(item);
            }
            else 
            {
                item.Status = item.Status == OrganizationStatusType.Inactive ? OrganizationStatusType.Deleted : OrganizationStatusType.Inactive;
                db.Entry(item).State = EntityState.Modified;
            }

            await db.SaveChangesAsync();

            var response = new ApiResponse<bool>(true);
            return Ok(response);
        } // DeleteOrganization

        // PRIVATE 

        private async Task DeleteTmpByUserAsync(string username)
        {
            var items = await db.Organizations
                .Where(o => 
                    o.UpdatedUser == username 
                    && o.Status == OrganizationStatusType.Nothing)
                .ToListAsync();

            foreach(var item in items)
            {
                db.Entry(item).State = EntityState.Deleted;
            }
        }

        private async Task DeleteTmpByPublicFromADay()
        { 
            var items = await db.Organizations
                .Where(o => 
                    o.UpdatedUser == "public"
                    && o.Status == OrganizationStatusType.Nothing
                    && o.Updated > DateTime.Now.AddDays(-1))
                .ToListAsync();

            foreach (var item in items)
            {
                db.Entry(item).State = EntityState.Deleted;
            }
        }
    }
}
