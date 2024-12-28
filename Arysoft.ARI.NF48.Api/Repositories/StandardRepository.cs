using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class StandardRepository : BaseRepository<Standard>
    {
        public async Task<bool> ExistNameAsync(string name, Guid idException)
        {
            return await _model
                .Where(m => 
                    m.Name.ToUpper() == name.ToUpper().Trim()
                    && m.ID != idException
                ).AnyAsync();
        } // ExistNameAsync

        //public override async Task DeleteTmpByUser(string username)
        //{
        //    var items = await _model
        //        .Where(m =>
        //            m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
        //            && m.Status == StatusType.Nothing
        //        ).ToListAsync();

        //    foreach (var item in items)
        //    {
        //        _model.Remove(item);
        //    }
        //} // DeleteTmpByUser
    }
}