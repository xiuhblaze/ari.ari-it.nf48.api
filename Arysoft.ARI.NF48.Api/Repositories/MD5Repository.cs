using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class MD5Repository : BaseRepository<MD5>
    {
        public async Task<bool> IsInRangeAsync(int startValue, int endValue)
        { 
            var isInRange = await _model
                .AnyAsync(m => m.StartValue <= endValue 
                    && m.EndValue >= startValue);

            return isInRange;
        }
    }
}