using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditAuditorRepository : BaseRepository<AuditAuditor>
    {
        // Metodos con acceso a datos especificos de AuditAuditor

        public async Task AddAuditStandardAsync(Guid id, Guid auditStandardID)
        {   
            var _auditStandardRepository = _context.Set<AuditStandard>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The auditor to add a standard was not found");
            var itemStandard = await _auditStandardRepository.FindAsync(auditStandardID)
                ?? throw new BusinessException("The standard you are trying to relate to the auditor was not found.");



            if (foundItem.AuditStandards.Contains(itemStandard))
                throw new BusinessException("The auditor already has the standard related.");

            foundItem.AuditStandards.Add(itemStandard);
        } // AddAuditStandardAsync

        public async Task DelAuditStandardAsync(Guid id, Guid auditStandardID)
        {
            var _auditStandardRepository = _context.Set<AuditStandard>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The auditor to delete a standard was not found");
            var itemStandard = await _auditStandardRepository.FindAsync(auditStandardID)
                ?? throw new BusinessException("The standard associated with the auditor was not found when trying to delete it.");

            if (!foundItem.AuditStandards.Contains(itemStandard))
                throw new BusinessException("The standard not related to the auditor.");

            foundItem.AuditStandards.Remove(itemStandard);
        } // DelAuditStandardAsync
    }
}