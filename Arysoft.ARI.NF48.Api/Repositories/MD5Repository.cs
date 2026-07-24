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
        public async Task<bool> IsInRangeAsync(int startValue, int endValue, MD5TableType tableType, Guid? exceptionID)
        { 
            var items = _model
                .Where(m => m.StartValue <= endValue && m.EndValue >= startValue
                    && m.Status == StatusType.Active
                    && m.TableType == tableType);

            if (exceptionID.HasValue && exceptionID != Guid.Empty)
            { 
                items = items.Where(m => m.ID != exceptionID.Value);
            }

            return await items.AnyAsync();
        }

        /// <summary>
        /// Obtiene el registro MD5 dado el numero de empleados y el tipo de tabla 
        /// indicada (QMS, EMS o OHSMS).
        /// </summary>
        /// <param name="employees"></param>
        /// <param name="tableType"></param>
        /// <returns></returns>
        public async Task<MD5> GetItemByEmployeesAsync(int employees, MD5TableType tableType)
        {
            var mD5Maximum = await _model
                .OrderByDescending(m => m.EndValue)
                .FirstOrDefaultAsync();
            // Si el número de empleados es mayor que el rango máximo, se devuelve el registro
            // con el rango máximo
            if (mD5Maximum.EndValue < employees)
                return mD5Maximum;

            return await _model
                .Where(m => m.StartValue <= employees && m.EndValue >= employees
                    && m.TableType == tableType
                    && m.Status == StatusType.Active)
                .FirstOrDefaultAsync();
        } // GetItemByEmployeesAsync

        ///// <summary>
        ///// Obtiene el número de dias aplicable dado el número de empleados y
        ///// el tipo de tabla MD5, por default era QMS , pero se agrego el 
        ///// tipo EMS y OHSMS para poder obtener los dias de acuerdo al tipo de 
        ///// standard que se este evaluando.
        ///// </summary>
        ///// <param name="employees">Numero de empleados a buscar el rango</param>
        ///// <param name="tableType">Tipo de tabla MD5, ya sea QMS, EMS o OHSMS</param>
        ///// <param name="riskLevel">Nivel de riesgo, solo necesario en algunos standards, por default es Medium</param>
        ///// <returns></returns>
        //public async Task<decimal> GetDaysAsync(
        //    int employees, 
        //    MD5TableType tableType, 
        //    RiskLevelCategoryType riskLevel = RiskLevelCategoryType.Medium
        //)
        //{
        //    var item = await GetItemByEmployeesAsync(employees, tableType);

        //    if (item == null) return 0;

        //    return riskLevel == RiskLevelCategoryType.Nothing ? item.Days ?? 0 :
        //           riskLevel == RiskLevelCategoryType.High ? item.HighDays ?? 0 :
        //           riskLevel == RiskLevelCategoryType.Medium ? item.Days ?? 0 :
        //           riskLevel == RiskLevelCategoryType.Low ? item.LowDays ?? 0 :
        //           riskLevel == RiskLevelCategoryType.Limited ? item.LimDays ?? 0
        //           : 0;
        //} // GetDaysAsync

        ///// <summary>
        /////  Obtiene el registro MD5 dado el numero de empleados
        /////  en el tipo de tabla indicada (QMS, EMS o OHSMS)
        ///// </summary>
        ///// <param name="employees"></param>
        ///// <param name="tableType"></param>
        ///// <returns></returns>
        //public async Task<MD5> GetByEmployeesAsync(int employees, MD5TableType tableType)
        //{
        //    var mD5Maximum = await _model
        //        .OrderByDescending(m => m.EndValue)
        //        .FirstOrDefaultAsync();

        //    // Si el número de empleados es mayor que el rango máximo, se devuelve el registro
        //    // con el rango máximo
        //    if (mD5Maximum.EndValue < employees) 
        //    {
        //        return mD5Maximum;
        //    }

        //    return await _model
        //        .Where(m => m.StartValue <= employees && m.EndValue >= employees
        //            && m.TableType == tableType
        //            && m.Status == StatusType.Active)
        //        .FirstOrDefaultAsync();
        //} // GetByEmployees

        // STATIC METHODS

        //public static decimal GetDaysByRiskLevel(MD5 item, RiskLevelCategoryType riskLevel)
        //{
        //    return riskLevel == RiskLevelCategoryType.High ? item.HighDays ?? 0 
        //        : riskLevel == RiskLevelCategoryType.Medium ? item.Days ?? 0 
        //        : riskLevel == RiskLevelCategoryType.Low ? item.LowDays ?? 0 
        //        : riskLevel == RiskLevelCategoryType.Limited ? item.LimDays ?? 0
        //        : item.Days ?? 0;
        //} // GetDaysByRiskLevel
    }
}