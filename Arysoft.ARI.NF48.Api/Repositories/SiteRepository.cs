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
        /// <summary>
        /// Marca todos los sitios que esten como principal como secundario o no principal
        /// Debe de ser solo uno, pero por cualquier cosa, mejor la busqueda en general.
        /// </summary>
        /// <param name="organizationID"></param>
        /// <returns></returns>
        public async Task SetToNotSiteMainAsync(Guid organizationID)
        {
            var items = await _model
                .Where(m => m.OrganizationID == organizationID
                    && (m.IsMainSite || m.Type == SiteType.Main))
                .ToListAsync();

            foreach (var item in items)
            {
                item.IsMainSite = false;                
                item.Type = SiteType.Secondary;                
                Update(item);
            }
        } // SetToNotSiteMainAsync
    }
}