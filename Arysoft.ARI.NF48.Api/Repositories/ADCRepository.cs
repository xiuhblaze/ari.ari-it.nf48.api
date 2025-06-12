using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ADCRepository : BaseRepository<ADC>
    {
        public async Task<bool> AppFormHasValidADCAsync(Guid appFormID)
        { 
            return await _model
                .AnyAsync(m => m.AppFormID == appFormID
                    && m.Status < ADCStatusType.Cancel);
        } // AppFormHasValidADCAsync

        public new async Task DeleteTmpByUserAsync(string username)
        {
            foreach (var item in await _model
                .Where(m => m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                            && m.Status == ADCStatusType.Nothing)
                .ToListAsync())
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUserAsync
    }
}