using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class RoleRepository : BaseRepository<Role>
    {
        public new void Delete(Role item)
        {
            _context.Database.ExecuteSqlCommand(
                "DELETE FROM UserRoles WHERE RoleID = {0}", item.ID
            );

            _context.Entry(item).State = EntityState.Deleted;
        } // Delete
    }
}