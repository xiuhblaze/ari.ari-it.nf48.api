using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class NaceCodeRepository : BaseRepository<NaceCode>
    {
        public async Task<bool> ExistNacecodeAsync(int? sector, int? division, int? group, int? @class, Guid? exceptionID)
        {
            var response = _model
                .Where(nc =>
                    nc.Sector == sector
                    && nc.Division == division
                    && nc.Group == group
                    && nc.Class == @class
                );
            if (exceptionID != null && exceptionID != Guid.Empty)
            {
                response = response.Where(nc => nc.ID != exceptionID);
            }
            return await response.AnyAsync();
        } // ExistNacecodeAsync
    }
}