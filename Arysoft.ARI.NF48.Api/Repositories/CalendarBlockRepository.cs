using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class CalendarBlockRepository : BaseRepository<CalendarBlock>
    {
        public async Task<bool> HasCalendarBlockAsync(
            Guid ownerID,
            DateTime startDate,
            DateTime endDate
        )
        {
            var start = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var end = new DateTime(endDate.Year, endDate.Month, endDate.Day, 11, 59, 59);
            var items = _model
                .Where(i => (i.OwnerID == ownerID || i.OwnerID == null)
                    && (i.StartDate <= end && i.EndDate >= start)
                    && !i.AllowOverride
                    && i.Status == StatusType.Active);

            return await items.AnyAsync();
        } // HasCalendarBlockAsync

        public async Task<List<CalendarBlock>> GetConflictsAsync(
            Guid ownerID,
            DateTime startDate,
            DateTime endDate)
        {
            var start = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var end = new DateTime(endDate.Year, endDate.Month, endDate.Day, 11, 59, 59);
            var items = _model
                .Where(i => (i.OwnerID == ownerID || i.OwnerID == null)
                    && (i.StartDate <= end && i.EndDate >= start)
                    && !i.AllowOverride
                    && i.Status == StatusType.Active);

            return await items.ToListAsync();
        } // GetConflictsAsync

        // Validar que no haya duplicado del mismo propietario, tipo y rango de tiempo, incluye los de OwnerID = null
        public async Task<bool> ExistCalendarBlockAsync(Guid? ownerID, DateTime startDate, DateTime endDate, CalendarBlockType blockType, Guid? ExceptionID)
        {
            var items = _model
                .Where(i => i.OwnerID == ownerID
                    && i.StartDate == startDate
                    && i.EndDate == endDate
                    && i.BlockType == blockType);

            if (ExceptionID.HasValue)
            { 
                items = items.Where(i => i.ID != ExceptionID.Value);
            }

            return await items.AnyAsync();
        } // ExistCalendarBlockAsync

        //public async Task<DateTime> GetWorkingDaysAsync(
        //    Guid ownerID,
        //    DateTime startDate,
        //    DateTime endDate)
        //{
        //    var conflictsDays = await GetConflictsAsync(ownerID, startDate, endDate);



        //} // GetWorkingDaysAsync
    }
}