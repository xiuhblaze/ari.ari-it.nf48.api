using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class OrganizationStandardRepository : BaseRepository<OrganizationStandard>
    {

        public async Task<bool> ExistStandardAsync(Guid OrganizationID, Guid StandardID, Guid OrganizationStandardExceptionID)
        {
            return await _model
                .Where(m => m.OrganizationID == OrganizationID
                    && m.StandardID == StandardID
                    && m.ID != OrganizationStandardExceptionID)
                .AnyAsync();
        } // ExistStandardAsync
    }
}