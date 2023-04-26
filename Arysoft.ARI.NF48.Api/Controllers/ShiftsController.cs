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
    public class ShiftsController : ApiController
    {
        private AriContext db = new AriContext();

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<Shift>>))]
        public IHttpActionResult GetShifts([FromUri] ShiftQueryFilters filters)
        {
            var items = db.Shifts.AsEnumerable();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            { 
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => e.ActivitesDescription.ToLower().Contains(filters.Text));
            }

            if (filters.SiteID != null && filters.SiteID != Guid.Empty)
            {
                items = items.Where(e => e.SiteID == filters.SiteID);
            }

            //if (filters.ShiftStart != null)
            //{
            //    items = items.Where(e => e.ShiftBegin >= filters.ShiftStart);
            //}

            if (filters.Type != null && filters.Type != ShiftType.Nothing)
            {
                items = items.Where(e => e.Type == filters.Type);
            }

            if (filters.Status != null && filters.Status != StatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                items = items.Where(e => e.Status != StatusType.Nothing);
            }

            // Order

            switch (filters.Order)
            {
                case ShiftOrderType.Type:
                    items = items.OrderBy(e => e.Type);
                    break;
                case ShiftOrderType.NoEmployees:
                    items = items.OrderBy(e => e.NoEmployees);
                    break;
                case ShiftOrderType.Status:
                    items = items.OrderBy(e => e.Status);
                    break;
                case ShiftOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case ShiftOrderType.TypeDesc:
                    items = items.OrderByDescending(e => e.Type);
                    break;
                case ShiftOrderType.NoEmployeesDesc:
                    items = items.OrderByDescending(e => e.NoEmployees);
                    break;
                case ShiftOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status);
                    break;
                case ShiftOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
            }

            var pagedItems = PagedList<Shift>.Create(items, filters.PageNumber, filters.PageSize);
            var response = new ApiResponse<IEnumerable<Shift>>(pagedItems);
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
        } // GetShifts

        [ResponseType(typeof(ApiResponse<Shift>))]
        public async Task<IHttpActionResult> GetShift(Guid id)
        {
            var item = await db.Shifts.FindAsync(id);
            if (item == null) return NotFound();

            var response = new ApiResponse<Shift>(item);
            return Ok(response);
        } // GetShift

        // POST: api/Shift
        [ResponseType(typeof(ApiResponse<Shift>))]
        public async Task<IHttpActionResult> PostShift([FromBody] ShiftPostDto shiftDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            await DeleteTmpByUserAsync(shiftDto.UpdatedUser);

            var item = new Shift
            {
                ShiftID = Guid.NewGuid(),
                SiteID = shiftDto.SiteID,
                Type = ShiftType.Nothing,
                Status = StatusType.Nothing,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                UpdatedUser = shiftDto.UpdatedUser
            };

            db.Shifts.Add(item);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var response = new ApiResponse<Shift>(item);
            return Ok(response);
        } // PostShift

        // PUT: api/Shift/5
        [ResponseType(typeof(ApiResponse<Shift>))]
        public async Task<IHttpActionResult> PutShift(Guid id, [FromBody] ShiftPutDto shiftDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (id != shiftDto.ShiftID) { return BadRequest("ID mismatch"); }

            var item = await db.Shifts.FindAsync(id);

            if (item == null) return NotFound();

            item.Type = shiftDto.Type;
            item.NoEmployees = shiftDto.NoEmployees;
            item.ShiftBegin = shiftDto.ShiftBegin;
            item.ShiftEnd = shiftDto.ShiftEnd;
            item.ActivitesDescription = shiftDto.ActivitesDescription;            
            item.Status = shiftDto.Status == StatusType.Nothing ? StatusType.Active : shiftDto.Status;
            item.Updated = DateTime.Now;
            item.UpdatedUser = shiftDto.UpdatedUser;

            db.Entry(item).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var response = new ApiResponse<Shift>(item);
            return Ok(response);
        } // PutShift

        // DELETE: api/Shift/5
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteShift(Guid id)
        {
            var shift = await db.Shifts.FindAsync(id);
            if (shift == null) return NotFound();

            if (shift.Status == StatusType.Deleted)
            {
                db.Shifts.Remove(shift);
            }
            else
            {
                shift.Status = shift.Status == StatusType.Active ? StatusType.Inactive : StatusType.Deleted;
                db.Entry(shift).State = EntityState.Modified;
            }

            await db.SaveChangesAsync();

            var response = new ApiResponse<bool>(true);
            return Ok(response);
        } // DeleteShift

        // PRIVATE

        private async Task DeleteTmpByUserAsync(string username)
        {
            var items = await db.Shifts
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
