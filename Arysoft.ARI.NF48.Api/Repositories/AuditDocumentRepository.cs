using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditDocumentRepository : BaseRepository<AuditDocument>
    {
        // Metodos con acceso a datos especificos de AuditDocument

        public async Task AddAuditStandardAsync(Guid id, Guid AuditStandardID)
        {
            var _auditStandardRepository = _context.Set<AuditStandard>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The document to add a standard was not found");
            var itemStandard = await _auditStandardRepository.FindAsync(AuditStandardID)
                ?? throw new BusinessException("The standard you are trying to add into the document was not found");

            if (foundItem.AuditStandards.Contains(itemStandard))
                throw new BusinessException("The standard already was assigned to the document");

            foundItem.AuditStandards.Add(itemStandard);
        } // AddStandardAsync
    }
}