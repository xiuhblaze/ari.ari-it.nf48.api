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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class SitesController : ApiController
    {
        private AriContext db = new AriContext();

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<Site>>))]
        public IHttpActionResult GetSites([FromUri] SiteQueryFilters filters)
        {
            var items = db.Sites.AsEnumerable();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => 
                    e.Description.ToLower().Contains(filters.Text)
                    || e.LocationDescription.ToLower().Contains(filters.Text)
                );
            }

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty) 
            {
                items = items.Where(e => e.OrganizationID == filters.OrganizationID);
            }

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

            // ORDER

            switch (filters.Order)
            {
                case SiteOrderType.Description:
                    items = items.OrderBy(e => e.Description);
                    break;
                case SiteOrderType.Order:
                    items = items.OrderBy(e => e.Order);
                    break;
                case SiteOrderType.Status:
                    items = items.OrderBy(e => e.Status);
                    break;
                case SiteOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case SiteOrderType.DescriptionDesc:
                    items = items.OrderByDescending(e => e.Description);
                    break;
                case SiteOrderType.OrderDesc:
                    items = items.OrderByDescending(e => e.Order);
                    break;
                case SiteOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status);
                    break;
                case SiteOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.Order);
                    break;
            }


            var pagedItems = PagedList<Site>.Create(items, filters.PageNumber, filters.PageSize);
            var response = new ApiResponse<IEnumerable<Site>>(pagedItems);
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
        } // GetSites

        [ResponseType(typeof(ApiResponse<Site>))]
        public async Task<IHttpActionResult> GetSite(Guid id)
        {
            var item = await db.Sites.FindAsync(id);
            if (item == null) return NotFound();

            // HACK: Validar si tiene el status 0, aun no es un registro consultable
            // HACK: Validar si esta en estatus de eliminado, que tenga el permiso para verlo

            var response = new ApiResponse<Site>(item);
            return Ok(response);
        } // GetShift

        // POST: api/Site
        [ResponseType(typeof(ApiResponse<Site>))]
        public async Task<IHttpActionResult> PostSite([FromBody] SitePostDto siteDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            await DeleteTmpByUserAsync(siteDto.UpdatedUser);

            var item = new Site
            {
                SiteID = Guid.NewGuid(),
                OrganizationID = siteDto.OrganizationID,                
                Status = StatusType.Nothing,
                Order = 0,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                UpdatedUser = siteDto.UpdatedUser
            };

            db.Sites.Add(item);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var response = new ApiResponse<Site>(item);
            return Ok(response);
        } // PostSite

        // PUT: api/Site/5
        [ResponseType(typeof(ApiResponse<Site>))]
        public async Task<IHttpActionResult> PutSite(Guid id, [FromBody] SitePutDto siteDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (id != siteDto.SiteID) { return BadRequest("ID mismatch"); }

            var item = await db.Sites.FindAsync(id);

            if (item == null) return NotFound();

            // HACK: Validar que no se duplique el orden

            item.Description = siteDto.Description;
            item.LocationDescription = siteDto.LocationDescription;
            item.Order = siteDto.Order;
            item.Status = siteDto.Status == StatusType.Nothing ? StatusType.Active : siteDto.Status;
            item.Updated = DateTime.Now;
            item.UpdatedUser = siteDto.UpdatedUser;

            db.Entry(item).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var response = new ApiResponse<Site>(item);
            return Ok(response);
        } // PutSite

        // DELETE: api/Site/5
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteSite(Guid id)
        {
            var site = await db.Sites.FindAsync(id);
            if (site == null) return NotFound();

            if (site.Status == StatusType.Deleted)
            {
                // TODO: Validar que no tenga shifts asociados (o eliminarlos o no dejarlo borrar)
                db.Sites.Remove(site);
            }
            else
            {
                site.Status = site.Status == StatusType.Active ? StatusType.Inactive : StatusType.Deleted;
                db.Entry(site).State = EntityState.Modified;
            }

            await db.SaveChangesAsync();

            var response = new ApiResponse<bool>(true);
            return Ok(response);
        } // DeleteSite

        // PRIVATE

        private async Task DeleteTmpByUserAsync(string username)
        {
            var items = await db.Sites
                .Where(o =>
                    o.UpdatedUser == username
                    && o.Status == StatusType.Nothing)
                .ToListAsync();

            foreach (var item in items)
            {
                db.Entry(item).State = EntityState.Deleted;
            }
        }
    }
}
