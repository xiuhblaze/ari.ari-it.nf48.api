using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ContactRepository : BaseRepository<Contact>
    {
        public override IEnumerable<Contact> Gets()
        {
            return _model
                .Include(m => m.Organization)
                .AsEnumerable();
        } // Gets

        public override async Task<Contact> GetAsync(Guid id)
        {
            return await _model
                .Include(m => m.Organization)
                .Where(m => m.ID == id)
                .FirstOrDefaultAsync();
        } // GetAsync

        /// <summary>
        /// Elimina todos los registros temporales generados por el usuario
        /// </summary>
        /// <param name="username">Nombre del usuario que ha generado los registros temporales</param>
        /// <returns></returns>
        public async Task DeleteTmpByUser(string username)
        {
            var items = await _model
                .Where(m => 
                    m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                    && m.Status == StatusType.Nothing
                ).ToListAsync();

            foreach(var item in items)
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUser
    } 
}