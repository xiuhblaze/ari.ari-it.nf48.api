using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class CertificateRepository : BaseRepository<Certificate>
    {
        public async Task SetInactiveByOrganizationAndStandardAsync(
            Guid organizationID, Guid standardID)
        {
            var items = await _model
                .Where(m => m.OrganizationID == organizationID
                    && m.StandardID == standardID
                    && m.Status == StatusType.Active)
                .ToListAsync();

            foreach (var item in items)
            {
                item.Status = StatusType.Inactive;
                Update(item);
            }
        } // SetInactiveByOrganizationAndStandardAsync

    }
}