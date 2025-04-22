using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditDocumentRepository : BaseRepository<AuditDocument>
    {
        // Metodos con acceso a datos especificos de AuditDocument

        public new void Delete(AuditDocument item)
        {
            _context.Database.ExecuteSqlCommand( // Para borrar en cascada la tabla intermedia
                "DELETE FROM AuditDocumentsStandards WHERE AuditDocumentID = {0}", item.ID);

            base.Delete(item);
        } // Delete

        public async Task AddAuditStandardAsync(Guid id, Guid auditStandardID)
        { 
            var _auditStandardRepository = _context.Set<AuditStandard>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The document to add a standard was not found");
            var itemStandard = await _auditStandardRepository.FindAsync(auditStandardID)
                ?? throw new BusinessException("The standard you are trying to add into the document was not found");

            if (foundItem.AuditStandards.Contains(itemStandard))
                throw new BusinessException("The standard already was assigned to the document");

            foundItem.AuditStandards.Add(itemStandard);
        } // AddStandardAsync

        public async Task DelAuditStandardAsync(Guid id, Guid auditStandardID)
        {
            var _auditStandardRepository = _context.Set<AuditStandard>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The document to add a standard was not found");
            var itemStandard = await _auditStandardRepository.FindAsync(auditStandardID)
                ?? throw new BusinessException("The standard associated with the audit was not found when trying to delete it from the document");

            if (!foundItem.AuditStandards.Contains(itemStandard))
                throw new BusinessException("The standard not related to the audit document");

            foundItem.AuditStandards.Remove(itemStandard);
        } // DelAuditStandardAsync

        public new async Task DeleteTmpByUserAsync(string username)
        {
            foreach (var item in await _model
                .Where(m =>
                    m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                    && m.Status == StatusType.Nothing
                ).ToListAsync())
            {
                _context.Database.ExecuteSqlCommand( // To delete the intermediate table cascade
                    "DELETE FROM AuditDocumentsStandards WHERE AuditDocumentID = {0}", item.ID);

                _model.Remove(item);
            }
        }
    }
}