using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class RoleRepository : BaseRepository<Role>
    {
        //public async Task<Role> UpdateAsync(Role item)
        //{
        //    var foundItem = await _model.FindAsync(item.ID)
        //        ?? throw new BusinessException("The record to update was not found");

        //    foundItem.Name = item.Name;
        //    foundItem.Description = item.Description;
        //    foundItem.Status = item.Status;
        //    foundItem.Updated = item.Updated;
        //    foundItem.UpdatedUser = item.UpdatedUser;

        //    Update(foundItem);

        //    return foundItem;
        //} // UpdateAsync

    //    public async Task DeleteTmpByUser(string username)
    //    {
    //        var items = await _model
    //            .Where(m => 
    //                m.UpdatedUser.ToUpper() == username.ToUpper() 
    //                && m.Status == StatusType.Nothing
    //            ).ToListAsync();

    //        foreach (var item in items)
    //        {
    //            _model.Remove(item);
    //        }
    //    } // DeleteTmpByUser
    }
}