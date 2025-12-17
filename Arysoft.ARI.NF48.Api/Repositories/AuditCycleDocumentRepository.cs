using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditCycleDocumentRepository : BaseRepository<AuditCycleDocument>
    {
        public new async Task DeleteTmpByUserAsync(string username)
        {
            foreach(var item in await _model
                .Where(m =>
                    m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                    && m.Status == StatusType.Nothing
                ).ToListAsync())
            {
                _context.Database.ExecuteSqlCommand( // Para borrar en cascada la tabla intermedia
                    "DELETE FROM AuditCycleDocumentsStandards WHERE AuditCycleDocumentID = {0}", item.ID);

                _model.Remove(item);
            }
        } // DeleteTmpByUserAsync

        //public async Task<bool> IsDocumentTypeInCycleActiveAsync(
        //    Guid auditCycleID,
        //    AuditCycleDocumentType documentType,
        //    Guid? exceptionID
        //    )
        //{
        //    var items = _model
        //        .Where(m =>
        //            m.AuditCycleID == auditCycleID
        //            && m.DocumentType == documentType
        //            && m.Status == StatusType.Active
        //        );

        //    if (exceptionID != null)
        //        items = items.Where(m => m.ID != exceptionID);

        //    return await items.AnyAsync();
        //} // IsDocumentTypeInCycleActive

        ///// <summary>
        ///// Busca si existe algun documento de un estándar en un ciclo de 
        ///// auditoría
        ///// </summary>
        ///// <param name="standardID"></param>
        ///// <param name="auditCycleID"></param>
        ///// <returns></returns>
        //public async Task<bool> IsAnyStandardDocumentInAuditCycleAsync(Guid standardID, Guid auditCycleID)
        //{ 
        //    var items = _model
        //        .Where(m =>
        //            m.StandardID == standardID
        //            && m.AuditCycleID == auditCycleID
        //        );
        //    return await items.AnyAsync();

        //} // IsAnyStandardDocumentInAuditCycleAsync

        // Audit Cycles

        /// <summary>
        /// Indica si ya esta asociado un auditcycle de un determinado standard al documento
        /// no puede haber audit cycles de un mismo standard asociados a un documento
        /// </summary>
        /// <param name="id"></param>
        /// <param name="standardID"></param>
        /// <returns></returns>
        public async Task<bool> IsAnyAuditCycleStandardAsync(Guid id, Guid standardID)
        {
            var foundItem = await _model
                .Include(acd => acd.AuditCycles)
                .Where(acd => acd.ID == id)
                .FirstOrDefaultAsync()
                ?? throw new BusinessException("The document to check audit standard cycles was not found");

            return foundItem.AuditCycles
                .Where(ac => ac.StandardID == standardID)
                .Any();
        } // IsAnyAuditCycleStandardAsync

        /// <summary>
        /// Asocia un ciclo de auditoría a un documento
        /// </summary>
        /// <param name="id"></param>
        /// <param name="auditCycleID"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task AddAuditCycleAsync(Guid id, Guid auditCycleID)
        {
            var _auditCycleRepository = _context.Set<AuditCycle>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The document to add a cycle was not found");
            var itemCycle = await _auditCycleRepository.FindAsync(auditCycleID)
                ?? throw new BusinessException("The cycle you are trying to add into the document was not found");
            
            if (foundItem.AuditCycles.Contains(itemCycle))
                throw new BusinessException("The cycle already was assigned to the document");

            foundItem.AuditCycles.Add(itemCycle);
        } // AddAuditCycleAsync

        /// <summary>
        /// Remove la asociación de un ciclo de auditoría a un documento
        /// </summary>
        /// <param name="id"></param>
        /// <param name="auditCycleID"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task DelAuditCycleAsync(Guid id, Guid auditCycleID)
        {
            var _auditCycleRepository = _context.Set<AuditCycle>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The document to remove a cycle was not found");
            var itemCycle = await _auditCycleRepository.FindAsync(auditCycleID)
                ?? throw new BusinessException("The cycle associated with the audit was not found when trying to delete it from the document");            
            
            if (!foundItem.AuditCycles.Contains(itemCycle))
                throw new BusinessException("The cycle not related to the audit document");
            
            foundItem.AuditCycles.Remove(itemCycle);
        } // DelAuditCycleAsync
    }
}