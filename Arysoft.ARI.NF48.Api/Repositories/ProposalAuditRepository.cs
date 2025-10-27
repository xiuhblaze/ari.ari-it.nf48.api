using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ProposalAuditRepository : BaseRepository<ProposalAudit>
    {
        // Aun no se necesita nada especial

        public async Task<ProposalAudit> GetByProposalAndStepAsync(Guid proposalId, AuditStepType auditStep)
        {
            var query = _model
                .Where(pa => pa.ProposalID == proposalId && pa.AuditStep == auditStep);
            
            return await query.FirstOrDefaultAsync();
        } // GetByProposalAndStepAsync

        public async Task<IEnumerable<ProposalAudit>> GetsByProposalAsync(Guid proposalId)
        {
            var query = _model
                .Where(pa => pa.ProposalID == proposalId);
            
            return await query.ToListAsync();
        } // GetsByProposalAsync
    }
}