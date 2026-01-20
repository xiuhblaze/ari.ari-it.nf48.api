using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ADCSiteAuditRepository : BaseRepository<ADCSiteAudit>
    {
        public bool ExistsAuditStep(Guid idADCSite, AuditStepType stepType, Guid? adcSiteAuditExceptionID)
        {
            var query = _model.Where(x => 
                x.ADCSiteID == idADCSite
                && x.AuditStep == stepType);

            if (adcSiteAuditExceptionID != null && adcSiteAuditExceptionID != Guid.Empty)
            {
                query = query.Where(x => x.ID != adcSiteAuditExceptionID);
            }

            return query.Any();
        } // ExistsAuditStep

        public async Task DeleteByADCIDAndAuditStepAsync(Guid ADCID, AuditStepType auditStepType)
        {
            var query = _model.Where(x =>
                x.ADCSite.ADCID == ADCID
                && x.AuditStep == auditStepType
            );

            foreach(var item in query)
            {
                _context.Entry(item).State = EntityState.Deleted;
            }

        } // DeleteByADCIDAndAuditStepAsync
    }
}