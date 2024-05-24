using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ShiftRepository : BaseRepository<Shift>
    {
        //public async Task UpdateAsync(Shift item)
        //{
        //    //var foundItem = await _model.FindAsync(item.ID)
        //    //    ?? throw new BusinessException("The record to update was not found");

        //    //foundItem.Type = item.Type;
        //    //foundItem.NoEmployees = item.NoEmployees;
        //    //foundItem.ShiftBegin = item.ShiftBegin;
        //    //foundItem.ShiftEnd = item.ShiftEnd ;
        //    //foundItem.ActivitiesDescription = item.ActivitiesDescription;
        //    //foundItem.Status = item.Status;
        //    //foundItem.Updated = item.Updated;
        //    //foundItem.UpdatedUser = item.UpdatedUser;


        //    Update(foundItem);

        //    return foundItem;
        //} // UpdateAsync

        public async Task DeleteTmpByUser(string username)
        {
            var items = await _model
                .Where(m => 
                    m.UpdatedUser.ToUpper() == username.ToUpper()
                    && m.Status == Enumerations.StatusType.Nothing
                ).ToListAsync();

            foreach ( var item in items)
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUser
    }
}