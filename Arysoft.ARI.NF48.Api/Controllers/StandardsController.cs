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
            var standards = db.Standards.AsEnumerable();

            // Filtros

            if (!string.IsNullOrEmpty(filters.Texto)) 
            {
                filters.Texto = filters.Texto.Trim().ToLower();
                standards = standards.Where(s =>
                    s.Name.ToLower().Contains(filters.Texto)
                    || s.Description.ToLower().Contains(filters.Texto)
                );
            }

            if (filters.Status != null && filters.Status != StatusType.Nothing)
            {
                standards = standards.Where(s => s.Status == filters.Status);
            }
            else
            {
                standards = standards.Where(s => s.Status != StatusType.Nothing);
            }

            // Orden

            switch (filters.Order) {
                case StandardsOrderType.Name:
                    standards = standards.OrderBy(s => s.Name);
                    break;
                case StandardsOrderType.Status:
                    standards = standards.OrderBy(s => s.Status);
                    break;
                case StandardsOrderType.Update:
                    standards = standards.OrderBy(s => s.Updated);
                    break;
                case StandardsOrderType.StatusDesc:
                    standards = standards.OrderByDescending(s => s.Status);
                    break;
                case StandardsOrderType.UpdateDesc:
                    standards = standards.OrderByDescending(s => s.Updated);
                    break;
                default:
                    standards = standards.OrderBy(s => s.Name);
                    break;
            }

            // Paginación

            var pagedStandards = PagedList<Standard>.Create(standards, filters.PageNumber, filters.PageSize);

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
        [ResponseType(typeof(Standard))]
        public async Task<IHttpActionResult> GetStandard(Guid id)
        {
            Standard standard = await db.Standards.FindAsync(id);
            if (standard == null)
            {
                return NotFound();
            }

            return Ok(standard);
        }

        // POST: api/Standards
        [ResponseType(typeof(Standard))]
        public async Task<IHttpActionResult> PostStandard(Standard standard)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            await DeleteTmpByUserAsync(standard.UpdatedUser);

            standard.StandardID = Guid.NewGuid();
            standard.Updated = DateTime.Now;

            db.Standards.Add(standard);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StandardExists(standard.StandardID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = standard.StandardID }, standard);
        }

        // PUT: api/Standards/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutStandard(Guid id, Standard standard)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != standard.StandardID)
            {
                return BadRequest();
            }

            standard.Updated = DateTime.Now;

            db.Entry(standard).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StandardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Standards/5
        [ResponseType(typeof(Standard))]
        public async Task<IHttpActionResult> DeleteStandard(Guid id)
        {
            Standard standard = await db.Standards.FindAsync(id);
            if (standard == null)
            {
                return NotFound();
            }

            db.Standards.Remove(standard);
            await db.SaveChangesAsync();

            return Ok(standard);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StandardExists(Guid id)
        {
            return db.Standards.Count(e => e.StandardID == id) > 0;
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