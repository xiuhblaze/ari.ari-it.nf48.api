using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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
    }
}