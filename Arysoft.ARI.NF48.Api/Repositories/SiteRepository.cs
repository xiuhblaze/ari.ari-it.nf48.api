using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class SiteRepository : BaseRepository<Site>
    {
        public async Task SetToNotSiteMainAsync(Guid organizationID)
        {
            var items = await _model
                .Where(m => m.OrganizationID == organizationID)
                .ToListAsync();

            foreach (var item in items)
            {
                item.IsMainSite = false;
                Update(item);
            }
        } // SetToNotSiteMainAsync
    }
}