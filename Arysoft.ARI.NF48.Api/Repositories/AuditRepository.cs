using Arysoft.ARI.NF48.Api.Models;
using System.Threading.Tasks;
using System;
using System.Data.Entity;
using System.Linq;
using Arysoft.ARI.NF48.Api.Enumerations;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditRepository : BaseRepository<Audit>
    {

        public new async Task DeleteTmpByUserAsync(string username)
        {
            var items = await _model
                .Include(o => o.AuditAuditors)
                .Include(o => o.AuditStandards)
                .Include(o => o.AuditDocuments)
                .Include(o => o.Notes)
                .Where(m =>
                    m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                    && m.Status == AuditStatusType.Nothing
                ).ToListAsync();

            foreach (var item in items)
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUserAsync
    }
}