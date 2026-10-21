using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class CalendarBlockRepository : BaseRepository<CalendarBlock>
    {

        public async Task<bool> HasCalendarBlock(
            Guid ownerID,
            DateTime startDate,
            DateTime endDate
        )
        {
            var items = _model
                .Where(i => (i.OwnerID == ownerID || i.OwnerID == null)
                    && (i.StartDate <= endDate && i.EndDate >= startDate)
                    && !i.AllowOverride);


            return await items.AnyAsync();
        } // HasOwnerAnAudit
    }
}