using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Arysoft.ARI.NF48.Api;
using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;

namespace Arysoft.ARI.NF48.Api.Controllers
{   
    public class StandardsController : ApiController
    {
        private AriContext db = new AriContext();

        // GET: api/Standards
        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<Standard>>))]
        public IHttpActionResult GetStandards([FromUri]StandardQueryFilters filters)
        {
            var items = db.Standards.AsEnumerable();

            // Filtros

            if (!string.IsNullOrEmpty(filters.Texto)) 
            {
                filters.Texto = filters.Texto.Trim().ToLower();
                items = items.Where(s =>
                    s.Name.ToLower().Contains(filters.Texto)
                    || s.Description.ToLower().Contains(filters.Texto)
                );
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

            // Orden

            switch (filters.Order) {
                case StandardsOrderType.Name:
                    items = items.OrderBy(s => s.Name);
                    break;
                case StandardsOrderType.Status:
                    items = items.OrderBy(s => s.Status);
                    break;
                case StandardsOrderType.Update:
                    items = items.OrderBy(s => s.Updated);
                    break;
                case StandardsOrderType.NameDesc:
                    items = items.OrderByDescending(s => s.Name);
                    break;
                case StandardsOrderType.StatusDesc:
                    items = items.OrderByDescending(s => s.Status);
                    break;
                case StandardsOrderType.UpdateDesc:
                    items = items.OrderByDescending(s => s.Updated);
                    break;
                default:
                    items = items.OrderBy(s => s.Name);
                    break;
            }

            // Paginación

            var pagedStandards = PagedList<Standard>.Create(items, filters.PageNumber, filters.PageSize);

            var response = new ApiResponse<IEnumerable<Standard>>(pagedStandards);
            var metadata = new Metadata
            {
                TotalCount = pagedStandards.TotalCount,
                PageSize = pagedStandards.PageSize,
                CurrentPage = pagedStandards.CurrentPage,
                TotalPages = pagedStandards.TotalPages,
                HasPreviousPage = pagedStandards.HasPreviousPage,
                HasNextPage = pagedStandards.HasNextPage
            };
            response.Meta = metadata;

            return Ok(response); // db.Standards;
        } // GetStandards

        // GET: api/Standards/5
        [ResponseType(typeof(ApiResponse<Standard>))]
        public async Task<IHttpActionResult> GetStandard(Guid id)
        {
            var item = await db.Standards.FindAsync(id);
            if (item == null) return NotFound();

            var response = new ApiResponse<Standard>(item);
            return Ok(response);
        }

        // POST: api/Standards
        [ResponseType(typeof(ApiResponse<Standard>))]
        public async Task<IHttpActionResult> PostStandard([FromBody] StandardPostDto itemDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            await DeleteTmpByUserAsync(itemDto.UpdatedUser);

            var item = new Standard { 
                StandardID = Guid.NewGuid(),
                Status = StatusType.Nothing,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                UpdatedUser = itemDto.UpdatedUser
            };

            db.Standards.Add(item);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var response = new ApiResponse<Standard>(item);
            return Ok(response);
        } // PostStandard

        // PUT: api/Standards/5
        [ResponseType(typeof(ApiResponse<Standard>))]
        public async Task<IHttpActionResult> PutStandard(Guid id, [FromBody] StandardPutDto itemDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != itemDto.StandardID) return BadRequest("ID mismatch");
            
            var item = await db.Standards.FindAsync(id);

            if (item == null) return NotFound();

            item.Name = itemDto.Name;
            item.Description = itemDto.Description;
            item.Status = itemDto.Status;
            item.Updated = DateTime.Now;

            db.Entry(item).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var response = new ApiResponse<Standard>(item);
            return Ok(response);
        } // PutStandard

        // DELETE: api/Standards/5
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteStandard(Guid id)
        {
            var item = await db.Standards.FindAsync(id);
            if (item == null) return NotFound();

            if (item.Status == StatusType.Deleted)
            {
                db.Standards.Remove(item);
            }
            else 
            {
                item.Status = item.Status == StatusType.Active ? StatusType.Inactive : StatusType.Deleted;
                db.Entry(item).State = EntityState.Modified;
            }

            await db.SaveChangesAsync();

            var response = new ApiResponse<bool>(true);
            return Ok(response);
        } // DeleteStandard 

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private async Task DeleteTmpByUserAsync(string username)
        {
            var items = await db.Standards
                .Where(s => s.UpdatedUser == username && s.Status == StatusType.Nothing)
                .ToListAsync();

            foreach (var item in items)
            {
                db.Entry(item).State = EntityState.Deleted;
            }

            // await db.SaveChangesAsync();
        }
    }
}