using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ApplicationRepository : BaseRepository<Application>
    {
        //public override IEnumerable<Application> Gets()
        //{
        //    return _model
        //        .Include(m => m.Organization) // Parece que asi va a jala Sites...
        //        .Include(m => m.Standard)
        //        .Include(m => m.NaceCode)
        //        .AsEnumerable();
        //} // Gets

        //public override async Task<Application> GetAsync(Guid id)
        //{
        //    return await _model
        //        .Include(m => m.Organization)
        //        .Include(m => m.Standard)
        //        .Include(m => m.NaceCode)
        //        // .Include(m => m.RiskLevel)
        //        .Where(m => m.ID == id)
        //        .FirstOrDefaultAsync();
        //} // GetAsync

        public new async Task DeleteTmpByUserAsync(string username)
        {
            var items = await _model
                .Where(m => m.UpdatedUser.ToLower() == username.ToLower()
                    && m.Status == ApplicationStatusType.Nothing
                ).ToListAsync();

            foreach (var item in items)
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUser
    }
}