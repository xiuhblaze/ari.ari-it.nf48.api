using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class DayCalculationConceptRepository : BaseRepository<DayCalculationConcept>
    {
        public async Task DeleteTmpByUser(string username)
        {
            var items = await _model
                .Where(m => m.UpdatedUser.ToLower() == username.ToLower().Trim()
                    && m.Status == StatusType.Nothing)
                .ToListAsync();

            foreach (var item in items)
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUser
    }
}