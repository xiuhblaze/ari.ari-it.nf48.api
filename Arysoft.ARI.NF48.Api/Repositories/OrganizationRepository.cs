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
                    o.Status != OrganizationStatusType.Nothing
                )
                .MaxAsync(o => o.Folio);

            return folio.HasValue ? folio.Value + 1 : 1;
        } // GetNextFolio

        public new async Task DeleteTmpByUserAsync(string username)
        {
            var items = await _model
                .Where(m =>
                    m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                    && m.Status == OrganizationStatusType.Nothing
                ).ToListAsync();

            foreach (var item in items)
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUser

        /// <summary>
        /// Permite eliminar los registros temporales que tengan más de un 
        /// día de antigüedad
        /// </summary>
        /// <returns></returns>
        private async Task DeleteTmpByPublicFromADay()
        {
            var items = await _model
                .Where(o =>
                    o.UpdatedUser == "public" // Considerar que este nombre sea un correo y elimine por el mismo
                    && o.Status == OrganizationStatusType.Nothing
                    && o.Updated > DateTime.Now.AddDays(-1))
                .ToListAsync();

            foreach (var item in items)
            {
                //db.Entry(item).State = EntityState.Deleted;
                _model.Remove(item);
            }
        } // DeleteTmpByPublicFromADay
    }
}