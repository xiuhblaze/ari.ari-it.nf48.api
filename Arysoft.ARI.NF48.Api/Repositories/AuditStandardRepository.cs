using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditStandardRepository : BaseRepository<AuditStandard>
    {
        public new void Delete(AuditStandard item)
        {
            // Para borrar en cascada la tabla intermedia
            _context.Database.ExecuteSqlCommand(
                "DELETE FROM AuditAuditorsStandards WHERE AuditStandardID = {0}", item.ID);

            _context.Database.ExecuteSqlCommand(
                "DELETE FROM AuditDocumentsStandards WHERE AuditStandardID = {0}", item.ID);
            
            // Eliminando el item
            base.Delete(item);
        } // Delete

        public new async Task DeleteTmpByUserAsync(string username)
        {
            foreach (var item in await _model
                .Where(m =>
                    m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                    && m.Status == StatusType.Nothing
                ).ToListAsync())
            {
                // Para borrar en cascada la tabla intermedia
                _context.Database.ExecuteSqlCommand(
                    "DELETE FROM AuditAuditorsStandards WHERE AuditStandardID = {0}", item.ID);

                _context.Database.ExecuteSqlCommand(
                    "DELETE FROM AuditDocumentsStandards WHERE AuditStandardID = {0}", item.ID);

                // Eliminando el item
                _model.Remove(item);
            }
        } // DeleteTmpByUserAsync

        /// <summary>
        /// Determina si un Step de Auditoria se encuentra ya asignado a una 
        /// auditoria para el mismo ciclo, puede omitir un registro que se 
        /// asume se esta trabajando con el, ignora las auditorias que son 
        /// temporales (.Nothing) o eliminadas logicamente (.Deleted)
        /// </summary>
        /// <param name="auditCycleID">Id del ciclo del certificado</param>
        /// <param name="stepType">Step a validar</param>
        /// <param name="ExceptionID">Id del registro AuditStandard a omitir</param>
        /// <returns></returns>
        public async Task<bool> IsStepInAuditCycleAsync(
            Guid auditCycleID,
            AuditStepType stepType,
            Guid? ExceptionID
            )
        {
            var query = _model
                .Include(m => m.Audit)
                .Where(m =>
                    m.AuditCycleID == auditCycleID
                    && m.Step == stepType
                    && m.Audit.Status != AuditStatusType.Nothing
                    && m.Audit.Status != AuditStatusType.Deleted
                );

            if (ExceptionID.HasValue)
            {
                query = query.Where(m => m.ID != ExceptionID.Value);
            }

            return await query.AnyAsync();
        } // IsAnyStepInAuditCycleAsync

        [Obsolete("La función ya no cumple con lo requerido")]
        public async Task<bool> IsAnyStandardStepAuditInAuditCycleAsync(
            Guid auditCycleID,
            Guid standardID,
            AuditStepType stepType,
            Guid? ExceptionID
            )
        {
            var query = _model
                .Where(m =>
                    m.AuditCycleID == auditCycleID &&
                    m.StandardID == standardID &&
                    m.Step == stepType
                );

            if (ExceptionID.HasValue)
            {
                query = query.Where(m => m.ID != ExceptionID.Value);
            }

            return await query.AnyAsync();
        } // IsAnyStandardStepAuditInAuditCycleAsync
    }
}