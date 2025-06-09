using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class MD5Repository : BaseRepository<MD5>
    {
        public async Task<bool> IsInRangeAsync(int startValue, int endValue, Guid? exceptionID)
        { 
            var items = _model
                .Where(m => m.StartValue <= endValue && m.EndValue >= startValue);

            if (exceptionID.HasValue && exceptionID != Guid.Empty)
            { 
                items = items.Where(m => m.ID != exceptionID.Value);
            }

            return await items.AnyAsync();
        }
    }
}