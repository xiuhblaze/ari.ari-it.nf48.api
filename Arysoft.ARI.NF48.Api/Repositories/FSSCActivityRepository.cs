using Arysoft.ARI.NF48.Api.Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class FSSCActivityRepository : BaseRepository<FSSCActivity>
    {
        ///// <summary>
        ///// Elimina todos los registros temporales generados por el nombre del usuario recibido
        ///// </summary>
        ///// <param name="username">Nombre del usuario</param>
        ///// <returns></returns>
        //public async Task DeleteTmpByUserAsync(string username)
        //{
        //    var items = await _model
        //        .Where(m =>
        //            m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
        //            && m.Status == Enumerations.StatusType.Nothing)
        //        .ToListAsync();

        //    foreach (var item in items)
        //    {
        //        _model.Remove(item);
        //    }
        //} // DeleteTmpByUserAsync
    }
}