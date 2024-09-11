using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class DayCalculationConceptApplicationRepository : BaseRepository<DayCalculationConceptApplication>
    {
        public override IEnumerable<DayCalculationConceptApplication> Gets()
        {
            return _model
                .Include(m => m.DayCalculationConcept)
                .Include(m => m.Application)
                .AsEnumerable();
        } // Gets

        public override async Task<DayCalculationConceptApplication> GetAsync(Guid id)
        {
            return await _model
                .Include(m => m.DayCalculationConcept)
                .Include(m => m.Application)
                .Where(m => m.ID == id)
                .FirstOrDefaultAsync();
        } // Get

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