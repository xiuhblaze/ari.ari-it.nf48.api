using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class OrganizationRepository : BaseRepository<Organization>
    {
        public async Task<Organization> GetAsync(int folio)
        {
            return await _model
                .FirstOrDefaultAsync(o => o.Folio == folio);
        } // GetAsync

        public async Task<int> GetNextFolioAsync()
        {
            var folio = await _model
                .Where(o => 
                    o.Status > OrganizationStatusType.Prospect
                )
                .MaxAsync(o => o.Folio);

            return folio.HasValue ? folio.Value + 1 : 1;
        } // GetNextFolio

        public async Task<bool> ExistOrganizationNameAsync(string name, Guid? exceptionID)
        {
            name = name.ToLower().Trim();
            var response = _model.Where(o => o.Name.ToLower() == name);

            if (exceptionID != null && exceptionID != Guid.Empty) 
            {
                response = response.Where(o => o.ID != exceptionID);
            }

            return await response.AnyAsync();
        } // ExistOrganizationNameAsync

        public new async Task DeleteTmpByUserAsync(string username)
        {
            var items = await _model
                .Include(o => o.Companies)
                .Include(o => o.Contacts)
                .Include(o => o.Sites.Select(s => s.Shifts))
                .Include(o => o.Certificates)
                .Where(m =>
                    m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                    && m.Status == OrganizationStatusType.Nothing
                ).ToListAsync();

            foreach (var item in items)
            {   
                _model.Remove(item);
            }
        } // DeleteTmpByUser
    }
}