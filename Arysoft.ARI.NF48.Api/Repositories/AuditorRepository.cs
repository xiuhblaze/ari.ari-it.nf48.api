using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditorRepository : BaseRepository<Auditor>
    {
        public async Task<Auditor> GetByFullNameAsync(
            string firstName, string middleName, string lastName, Guid? exceptionID = null)
        {
            firstName = firstName.Trim().ToLower();
            middleName = middleName.Trim().ToLower();
            lastName = lastName.Trim().ToLower();

            var item = _model
                .Where(m => m.FirstName.ToLower() == firstName
                    && m.MiddleName.ToLower() == middleName
                    && m.LastName.ToLower() == lastName);

            if (exceptionID != null)
                item = item.Where(m => m.ID != exceptionID);

            return await item.FirstOrDefaultAsync();
        } // GetByFullNameAsync

        /// <summary>
        /// Elimina todos los registros temporales generados por el usuario
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task DeleteTmpByUser(string username)
        {
            var items = await _model
                .Where(m =>
                    m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                    && m.Status == StatusType.Nothing)
                .ToListAsync();

            foreach(var item in items)
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUser
    }
}