using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class RoleRepository : BaseRepository<Role>
    {

        public async Task DeleteTmpByUser(string username)
        {
            var items = await _model
                .Where(m => m.UpdatedUser.ToUpper() == username.ToUpper() && m.Status == Enumerations.StatusType.Nothing)
                .ToListAsync();

            foreach (var item in items)
            {
                _model.Remove(item);
            }
        }
    }
}