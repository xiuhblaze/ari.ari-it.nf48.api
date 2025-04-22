using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
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
    }
}