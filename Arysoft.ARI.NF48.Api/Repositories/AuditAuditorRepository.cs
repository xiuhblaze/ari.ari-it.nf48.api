using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditAuditorRepository : BaseRepository<AuditAuditor>
    {
        // Metodos con acceso a datos especificos de AuditAuditor

        public async Task AddAuditStandardAsync(Guid id, Guid AuditStandardID)
        {
            // var _auditStandardRepository = new AuditStandardRepository();
            var _auditStandardRepository = _context.Set<AuditStandard>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The auditor to add a standard was not found");
            var itemStandard = await _auditStandardRepository.FindAsync(AuditStandardID)
                ?? throw new BusinessException("The standard you are trying to relate to the auditor was not found.");

            if (foundItem.AuditStandards.Contains(itemStandard))
                throw new BusinessException("The auditor already has the standard related.");

            foundItem.AuditStandards.Add(itemStandard);
        } // AddAuditStandardAsync
    }
}