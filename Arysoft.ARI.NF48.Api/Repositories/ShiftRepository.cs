using Arysoft.ARI.NF48.Api.Models;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ShiftRepository : BaseRepository<Shift>
    {   

        //public async Task DeleteTmpByUser(string username)
        //{
        //    var items = await _model
        //        .Where(m => 
        //            m.UpdatedUser.ToUpper() == username.ToUpper()
        //            && m.Status == Enumerations.StatusType.Nothing
        //        ).ToListAsync();

        //    foreach ( var item in items)
        //    {
        //        _model.Remove(item);
        //    }
        //} // DeleteTmpByUser
    }
}