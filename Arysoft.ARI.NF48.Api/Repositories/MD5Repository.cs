using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class MD5Repository : BaseRepository<MD5>
    {
        /// <summary>
        /// Determina si existe un rango de valores que se superponga con el rango dado.
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="exceptionID"></param>
        /// <returns></returns>
        public async Task<bool> IsInRangeAsync(int startValue, int endValue, Guid? exceptionID)
        { 
            var items = _model
                .Where(m => m.StartValue <= endValue && m.EndValue >= startValue
                    && m.Status == StatusType.Active);

            if (exceptionID.HasValue && exceptionID != Guid.Empty)
            { 
                items = items.Where(m => m.ID != exceptionID.Value);
            }

            return await items.AnyAsync();
        }

        /// <summary>
        /// Obtiene el número de dias aplicable dado el número de empleados
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        public async Task<decimal> GetDaysAsync(int employees)
        {
            var item = await _model
                .Where(m => m.StartValue <= employees && m.EndValue >= employees
                    && m.Status == StatusType.Active)
                .FirstOrDefaultAsync();

            return item.Days ?? 0;
        } // GetDaysAsync
    }
}