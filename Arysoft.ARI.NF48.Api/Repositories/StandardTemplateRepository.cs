using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class StandardTemplateRepository : BaseRepository<StandardTemplate>
    {
        /// <summary>
        /// Valida si es un standard con el status de activo o inactivo
        /// </summary>
        /// <param name="standardID">ID del standard a validar</param>
        /// <returns>true si es valido</returns>
        public async Task<bool> IsValidStandard(Guid standardID)
        { 
            return await _context.Set<Standard>()
                .Where(s => s.ID == standardID
                    && (s.Status == StatusType.Active
                        || s.Status == StatusType.Inactive))
                .AnyAsync();
        } // IsValidStandard
    }
}