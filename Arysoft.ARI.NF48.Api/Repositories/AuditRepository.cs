using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditRepository : BaseRepository<Audit>
    {
        public Audit GetNextAudit(Guid? OrganizationID, DateTime? date)
        {  
            var items = _model
                .Include(a => a.AuditAuditors)
                .Include(a => a.AuditStandards)
                .Include(a => a.AuditDocuments)
                .Include(a => a.Notes);

            if (date == null) date = DateTime.Now;

            items = items.Where(a =>
                a.StartDate >= date
                && a.Status != AuditStatusType.Nothing
                && a.Status < AuditStatusType.Canceled
                && a.AuditCycle.Status != StatusType.Nothing
            );

            if (OrganizationID != null && OrganizationID != Guid.Empty)
            {
                items = items.Where(a => a.AuditCycle.OrganizationID == OrganizationID);
            }

            items = items.OrderBy(a => a.StartDate);

            return items.FirstOrDefault();
        } // GetNextAudit

        public async Task<bool> HasAuditorAnAudit(
            Guid auditorID, 
            DateTime startDate, 
            DateTime endDate, 
            Guid? auditExceptionID)
        {
            var items = _model
                .Include(a => a.AuditAuditors)
                .Where(a =>
                    a.AuditAuditors.Any(aa => 
                        aa.AuditorID == auditorID
                        && aa.Status == StatusType.Active)
                    && (a.StartDate <= endDate && a.EndDate >= startDate) 
                    && a.Status != AuditStatusType.Nothing 
                    && a.Status < AuditStatusType.Canceled
                );

            if (auditExceptionID != null && auditExceptionID != Guid.Empty)
            {
                items = items.Where(a => a.ID != auditExceptionID);
            }

            return await items.AnyAsync();
        } // HasAuditorAudit

        public async Task<bool> IsAnyStandardStepAuditInAuditCycle(
            Guid auditCycleID,
            Guid standardID,
            AuditStepType step,
            Guid? auditExceptionID)
        {
            var items = _model
                .Include(a => a.AuditStandards)
                .Where(a =>
                    a.AuditCycleID == auditCycleID
                    && a.AuditStandards.Any(asd =>
                        asd.StandardID == standardID
                        && asd.Step == step
                        && asd.Status == StatusType.Active)
                    && a.Status != AuditStatusType.Nothing
                    && a.Status < AuditStatusType.Canceled
                );

            if (auditExceptionID != null && auditExceptionID != Guid.Empty)
            {
                items = items.Where(a => a.ID != auditExceptionID);
            }

            return await items.AnyAsync();
        } // IsAnyStandardStepAuditInAuditCycle

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