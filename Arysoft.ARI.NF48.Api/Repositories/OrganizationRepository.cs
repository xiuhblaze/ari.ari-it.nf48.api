using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class OrganizationRepository : BaseRepository<Organization>
    {
        public async Task DeleteTmpByUser(string username)
        {
            var items = await _model
                .Where(m => 
                    m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                    && m.Status == OrganizationStatusType.Nothing
                ).ToListAsync();

            foreach (var item in items)
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUser

        private async Task DeleteTmpByPublicFromADay()
        {
            var items = await _model
                .Where(o =>
                    o.UpdatedUser == "public" // Considerar que este nombre sea un correo y elimine por el mismo
                    && o.Status == OrganizationStatusType.Nothing
                    && o.Updated > DateTime.Now.AddDays(-1))
                .ToListAsync();

            foreach (var item in items)
            {
                //db.Entry(item).State = EntityState.Deleted;
                _model.Remove(item);
            }
        } // DeleteTmpByPublicFromADay
    }
}