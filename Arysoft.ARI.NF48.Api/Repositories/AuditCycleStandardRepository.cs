using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditCycleStandardRepository : BaseRepository<AuditCycleStandard>
    {
        public async Task<bool> IsStandardInCycleAsync(
            Guid auditCycleID, 
            Guid standardID, 
            Guid? exceptionID,
            bool onlyActive = false
        )
        {
            var items = _model
                .Where(m => 
                    m.AuditCycleID == auditCycleID 
                    && m.StandardID == standardID
                );

            if (exceptionID != null)
                items = items.Where(m => m.ID != exceptionID);

            if (onlyActive)
                items = items.Where(m => m.Status == StatusType.Active);
            else
                items = items.Where(m => m.Status != StatusType.Deleted && m.Status != StatusType.Nothing);

            return await items.AnyAsync();
        } // IsStandardInCycleAsync

        public async Task<bool> IsStandardActiveInOrganizationAnyAuditCycleAsync(
            Guid organizationID,
            Guid standardID,
            Guid? exceptionAuditCycleID
            )
        {
            var items = _model
                .Where(m => 
                    m.AuditCycle.OrganizationID == organizationID
                    && m.AuditCycle.Status == StatusType.Active
                    && m.StandardID == standardID
                    && m.Status == StatusType.Active
                );

            if (exceptionAuditCycleID != null)
                items = items.Where(m => m.AuditCycleID != exceptionAuditCycleID);

            return await items.AnyAsync();
        } // IsStandardInAnyOrganizationActiveCycleAsync
    }
}