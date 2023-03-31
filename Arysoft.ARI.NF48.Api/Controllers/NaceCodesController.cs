using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Models.Mappings;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

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
            else 
            {
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
        [ResponseType(typeof(ApiResponse<NaceCode>))]
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
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            await DeleteTmpByUserAsync(naceCodeDto.UpdatedUser);

            var naceCode = NaceCodeMappings.PostToNaceCode(naceCodeDto);

            naceCode.NaceCodeID = Guid.NewGuid();
            naceCode.Updated = DateTime.Now;

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

            //return CreatedAtRoute("DefaultApi", new { id = naceCode.NaceCodeID }, naceCode);
            var response = new ApiResponse<NaceCode>(naceCode);
            return Ok(response);
        }

        // PUT: api/NaceCodes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutNaceCode(Guid id, NaceCodePutDto naceCodeDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var naceCode = NaceCodeMappings.PutToNaceCode(naceCodeDto);

            // Validate if not exist a duplicate item with Sector, Division, Group and Class

            naceCode.Status = naceCode.Status == StatusType.Nothing ? StatusType.Active : naceCode.Status;
            naceCode.Updated = DateTime.Now;

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

            //var response = new ApiResponse<bool>(true);
            //return Ok(response);
            //StatusCode(HttpStatusCode.NoContent);
            var response = new ApiResponse<NaceCode>(naceCode);
            return Ok(response);
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

            if (naceCode.Status == StatusType.Deleted)
            {
                db.NaceCodes.Remove(naceCode);
            }
            else
            {
                naceCode.Status = naceCode.Status == StatusType.Active ? StatusType.Inactive : StatusType.Deleted;
                db.Entry(naceCode).State = EntityState.Modified;
            }

            await db.SaveChangesAsync();

            var response = new ApiResponse<NaceCode>(naceCode);
            return Ok(response);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // PRIVATED

        private bool NaceCodeExists(Guid id)
        {
            return db.NaceCodes.Count(e => e.NaceCodeID == id) > 0;
        }

        private async Task DeleteTmpByUserAsync(string username)
        {
            var items = await db.NaceCodes
                .Where(n => n.UpdatedUser == username && n.Status == StatusType.Nothing)
                .ToListAsync();

            foreach(var item in items)
            {
                db.Entry(item).State = EntityState.Deleted;
            }
            //await db.SaveChangesAsync();
        }
    }
}