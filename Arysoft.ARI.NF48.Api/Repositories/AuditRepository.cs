using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditRepository : BaseRepository<Audit>
    {
        public async Task<bool> HasAuditorAnAudit(Guid auditorID, DateTime startDate, DateTime endDate, Guid? auditExceptionID)
        {
            var items = _model
                .Include(a => a.AuditAuditors)
                .Where(a =>
                    a.AuditAuditors.Any(aa => aa.AuditorID == auditorID)
                    && (a.StartDate <= endDate && a.EndDate >= startDate) // Probar esto
                    && a.Status != AuditStatusType.Nothing 
                );

            if (auditExceptionID != null && auditExceptionID != Guid.Empty)
            {
                items = items.Where(a => a.ID != auditExceptionID);
            }

            return await items.AnyAsync();
        } // HasAuditorAudit

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