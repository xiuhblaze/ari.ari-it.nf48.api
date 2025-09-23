using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ADCSiteRepository : BaseRepository<ADCSite>
    {
        public int OrganizationStandardCount(Guid ADCSiteID)
        {
            var sql = @"SELECT COUNT(*) FROM dbo.OrganizationStandards 
	            WHERE Status = 1 
		        AND OrganizationID = (SELECT OrganizationID FROM dbo.Sites WHERE SiteID = 
			        (SELECT SiteID FROM dbo.ADCSites WHERE ADCSiteID = @ADCSiteID))";

            var sqlParameter = new SqlParameter("@ADCSiteID", SqlDbType.UniqueIdentifier)
            {
                Value = ADCSiteID
            };

            return _context.Database.SqlQuery<int>(sql, sqlParameter)
                .FirstOrDefault();
        } // IsMultiStandard

        public void UpdateValues(ADCSite item)
        {
            var existing = _context.Set<ADCSite>().Local
                .FirstOrDefault(m => m.ID == item.ID);
            if (existing != null)
            {
                _context.Entry(existing).State = EntityState.Detached;
            }
            _context.Entry(item).State = EntityState.Modified;
        } // UpdateValues

        public async Task DeleteByListToRemoveAsync(List<Guid> IdsToRemove)
        {
            var sitesToRemove = await _model
                .Where(m => IdsToRemove.Contains(m.ID))
                .ToListAsync();

            _model.RemoveRange(sitesToRemove);

            await _context.SaveChangesAsync();
        } // DeleteByListToRemoveAsync
    }
}