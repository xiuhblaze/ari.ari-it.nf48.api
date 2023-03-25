using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Arysoft.ARI.NF48.Api;
using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Models.Mappings;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class NaceCodesController : ApiController
    {
        private AriContext db = new AriContext();

        // GET: api/NaceCodes
        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<NaceCode>>))]
        public IHttpActionResult GetNaceCodes([FromUri]NaceCodeQueryFilters filters)
        {
            var items = db.NaceCodes.AsEnumerable();

            // Filters

            if(!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => e.Description.ToLower().Contains(filters.Text));
            }

            if (filters.Status != StatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else {
                items = items.Where(e => e.Status != StatusType.Nothing);
            }

            // Orden

            switch (filters.Order)
            {   
                case NaceCodeOrderType.Description:
                    items = items.OrderBy(e => e.Description);
                    break;
                case NaceCodeOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case NaceCodeOrderType.SectorDesc:
                    items = items.OrderByDescending(e => e.Sector)
                        .ThenByDescending(e => e.Division)
                        .ThenByDescending(e => e.Group)
                        .ThenByDescending(e => e.Class);
                    break;
                case NaceCodeOrderType.DescriptionDesc:
                    items = items.OrderByDescending(e => e.Description);
                    break;
                case NaceCodeOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default: // NaceCodeOrderType.Sector
                    items = items.OrderBy(e => e.Sector)
                        .ThenBy(e => e.Division)
                        .ThenBy(e => e.Group)
                        .ThenBy(e => e.Class);
                    break;
            }

            // Pagination

            var pagedNacecodes = PagedList<NaceCode>.Create(items, filters.PageNumber, filters.PageSize);
            var response = new ApiResponse<IEnumerable<NaceCode>>(pagedNacecodes);
            var metadata = new Metadata
            {
                TotalCount = pagedNacecodes.TotalCount,
                PageSize = pagedNacecodes.PageSize,
                CurrentPage = pagedNacecodes.CurrentPage,
                TotalPages = pagedNacecodes.TotalPages,
                HasPreviousPage = pagedNacecodes.HasPreviousPage,
                HasNextPage = pagedNacecodes.HasNextPage
            };
            response.Meta = metadata;

            return Ok(response);
        }

        // GET: api/NaceCodes/5
        [ResponseType(typeof(NaceCode))]
        public async Task<IHttpActionResult> GetNaceCode(Guid id)
        {
            NaceCode naceCode = await db.NaceCodes.FindAsync(id);
            if (naceCode == null)
            {
                return NotFound();
            }

            var response = new ApiResponse<NaceCode>(naceCode);
            return Ok(response);
        }

        // POST: api/NaceCodes
        [ResponseType(typeof(NaceCode))]
        public async Task<IHttpActionResult> PostNaceCode(NaceCodePostDto naceCodeDto)
        {
            var naceCode = NaceCodeMappings.PostToNaceCode(naceCodeDto);

            naceCode.NaceCodeID = Guid.NewGuid();
            naceCode.Updated = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.NaceCodes.Add(naceCode);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NaceCodeExists(naceCode.NaceCodeID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = naceCode.NaceCodeID }, naceCode);
        }

        // PUT: api/NaceCodes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutNaceCode(Guid id, NaceCode naceCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != naceCode.NaceCodeID)
            {
                return BadRequest();
            }

            db.Entry(naceCode).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NaceCodeExists(id))
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

        // DELETE: api/NaceCodes/5
        [ResponseType(typeof(NaceCode))]
        public async Task<IHttpActionResult> DeleteNaceCode(Guid id)
        {
            NaceCode naceCode = await db.NaceCodes.FindAsync(id);
            if (naceCode == null)
            {
                return NotFound();
            }

            db.NaceCodes.Remove(naceCode);
            await db.SaveChangesAsync();

            return Ok(naceCode);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NaceCodeExists(Guid id)
        {
            return db.NaceCodes.Count(e => e.NaceCodeID == id) > 0;
        }
    }
}