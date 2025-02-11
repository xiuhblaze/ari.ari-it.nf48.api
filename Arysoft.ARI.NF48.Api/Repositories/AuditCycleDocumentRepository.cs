using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditCycleDocumentRepository : BaseRepository<AuditCycleDocument>
    {
        public async Task<bool> IsDocumentTypeInCycleActiveAsync(
            Guid auditCycleID,
            AuditCycleDocumentType documentType,
            Guid? exceptionID
            )
        {
            var items = _model
                .Where(m =>
                    m.AuditCycleID == auditCycleID
                    && m.DocumentType == documentType
                    && m.Status == StatusType.Active
                );

            if (exceptionID != null)
                items = items.Where(m => m.ID != exceptionID);

            return await items.AnyAsync();
        } // IsDocumentTypeInCycleActive
    }
}