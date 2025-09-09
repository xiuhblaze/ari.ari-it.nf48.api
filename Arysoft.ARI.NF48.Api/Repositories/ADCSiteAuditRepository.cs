using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ADCSiteAuditRepository : BaseRepository<ADCSiteAudit>
    {
        // Aun no se me ocurre nada

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
    }
}