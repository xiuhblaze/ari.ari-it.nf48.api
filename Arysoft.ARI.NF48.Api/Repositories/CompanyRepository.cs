using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class CompanyRepository : BaseRepository<Company>
    {
        public async Task<bool> ExistCompanyNameAsync(string name, Guid organizationID, Guid? exceptionID)
        {
            name = name.ToLower().Trim();
            var response = _model.Where(o => 
                o.Name.ToLower() == name 
                && o.ID == organizationID);
            if (exceptionID != null && exceptionID != Guid.Empty)
            {
                response = response.Where(o => o.ID != exceptionID);
            }
            return await response.AnyAsync();
        } // ExistCompayNameAsync

        public async Task<bool> ExistLegalEntityAsync(string legalEntity, Guid? exceptionID)
        {
            legalEntity = legalEntity.ToLower().Trim();
            var response = _model.Where(o => o.LegalEntity.ToLower() == legalEntity);
            if (exceptionID != null && exceptionID != Guid.Empty)
            {
                response = response.Where(o => o.ID != exceptionID);
            }
            return await response.AnyAsync();
        } // ExistLegalEntityAsync
    }
}