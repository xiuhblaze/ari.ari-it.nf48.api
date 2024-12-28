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

        ///// <summary>
        ///// Elimina todos los registros temporales generados por el usuario
        ///// </summary>
        ///// <param name="username"></param>
        ///// <returns></returns>
        //public async Task DeleteTmpByUser(string username)
        //{
        //    var items = await _model
        //        .Where(m => 
        //            m.UpdatedUser.ToUpper() == username.ToUpper()
        //            && m.Status == StatusType.Nothing
        //        ).ToListAsync();

        //    foreach(var item in items ) 
        //    {
        //        _model.Remove(item);
        //    }
        //} // DeleteTmpByUser
    }
}