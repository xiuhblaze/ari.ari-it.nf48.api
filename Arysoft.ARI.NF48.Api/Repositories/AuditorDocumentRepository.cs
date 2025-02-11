using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditorDocumentRepository : BaseRepository<AuditorDocument>
    {
        /// <summary>
        /// Marca todos los documentos de una categoria para un auditor en inactivos, considerando
        /// que se va a marcar un nuevo registro como activo
        /// </summary>
        /// <param name="auditorID"></param>
        /// <param name="catAuditorDocumentID"></param>
        /// <returns></returns>
        public async Task SetToInactiveDocumentsAsync(Guid auditorID, Guid catAuditorDocumentID)
        {
            var items = await _model
                .Where(m => 
                    m.AuditorID == auditorID 
                    && m.CatAuditorDocumentID == catAuditorDocumentID
                    && m.Status == StatusType.Active)
                .ToListAsync();

            foreach (var item in items)
            {
                item.Status = StatusType.Inactive;
                Update(item);
            }
        } // SetToInactiveDocumentsAsync

        ///// <summary>
        ///// Elimina todos los registros temporales generados por el usuario
        ///// </summary>
        ///// <param name="username"></param>
        ///// <returns></returns>
        //public async Task DeleteTmpByUser(string username)
        //{
        //    var items = await _model
        //        .Where(m =>
        //            m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
        //            && m.Status == StatusType.Nothing)
        //        .ToListAsync();

        //    foreach (var item in items)
        //    {
        //        _model.Remove(item);
        //    }
        //} // DeleteTmpByUser
    }
}