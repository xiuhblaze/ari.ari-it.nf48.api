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
        public new async Task<ADC> GetAsync(Guid id, bool asNoTracking = false)
        {
            var query = _model.AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query
                .Include(m => m.AppForm)
                .Include(m => m.ADCSites)
                .Include("ADCSites.Site")
                .Include("ADCSites.Site.Shifts")
                .Include("ADCSites.ADCConceptValues")
                .Include(m => m.Notes)
                .FirstOrDefaultAsync(m => m.ID == id);
        } // GetAsync

        public void UpdateValues(ADC item)
        {
            var existing = _context.Set<ADC>().Local
                .FirstOrDefault(m => m.ID == item.ID);

            if (existing != null)
            {
                _context.Entry(existing).State = EntityState.Detached;
            }

            _context.Entry(item).State = EntityState.Modified;
        } // UpdateValues

        public async Task<bool> AppFormHasValidADCAsync(Guid appFormID)
        { 
            return await _model
                .AnyAsync(m => m.AppFormID == appFormID
                    && m.Status > ADCStatusType.Nothing
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

        public void DetachAllEntities()
        { 
            var changedEntriesCopy = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted ||
                            e.State == EntityState.Unchanged)
                .ToList();
            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        } // DetachAllEntities
    }
}