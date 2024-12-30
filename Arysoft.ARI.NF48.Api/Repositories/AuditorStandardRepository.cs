using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditorStandardRepository : BaseRepository<AuditorStandard>
    {
        public async Task<bool> ExistStandardAsync(Guid id, Guid StandardID, Guid AuditorStandardExceptionID)
        {
            return await _model
                .Where(m => m.AuditorID == id && m.StandardID == StandardID && m.ID != AuditorStandardExceptionID)
                .AnyAsync();
        } // ExistStandard
    }
}