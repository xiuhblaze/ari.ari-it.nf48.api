using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System.ComponentModel;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Tools
{
    public class OrganizationCalculations
    {
        /// <summary>
        /// Obtiene la suma del total de trabajadores en sitio y fuera de sitio, de ser null
        /// alguno de ellos, devuvelve cero.
        /// </summary>
        /// <param name="workersOnSite">Total de trabajadores en sitio</param>
        /// <param name="workersOffsite">Total de trabajadores fuera de sitio</param>
        /// <returns></returns>
        public static int GetTotalWorkers(int? workersOnSite, int? workersOffsite)
        {
            return (workersOnSite ?? 0) + (workersOffsite ?? 0);
        } // GetTotalWorkers

        /// <summary>
        /// Obtiene la suma del total de trabajadores del turno especificado, de 
        /// estar en null, devuelve cero.
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static int GetTotalWorkers(Shift shift)
        {

            return GetTotalWorkers(shift.WorkersOnSite, shift.WorkersOffSite);
        } // GetTotalWorkers

        /// <summary>
        /// Obtiene la suma total de trabajadores del sitio especificado,
        /// se puede indicar que sea del total o solo de los turnos (shifts)
        /// activos, por default true (solo los activos).
        /// </summary>
        /// <param name="site">Sitio a sumar el total de trabajadores</param>
        /// <param name="onlyActive">Indica si solo se deben de contar los activos (true)</param>
        /// <returns></returns>
        public static int GetTotalWorkers(Site site, bool onlyActive = true)
        {
            if (site == null || site.Shifts == null || !site.Shifts.Any())
                return 0;

            if (site.Shifts != null) {
                int totalWorkers = 0;

                if (onlyActive)
                {
                    totalWorkers = site.Shifts
                        .Where(i => i.Status == StatusType.Active)
                        .Sum(i => i.WorkersOnSite + i.WorkersOffSite) ?? 0;
                }
                else
                {
                    totalWorkers = site.Shifts
                        .Sum(i => i.WorkersOnSite + i.WorkersOffSite) ?? 0;
                }

                //foreach (Shift shift in site.Shifts)
                //{
                //    if (onlyActive)
                //    {
                //        if (shift.Status == StatusType.Active)
                //        {
                //            totalWorkers += GetTotalWorkers(shift.WorkersOnSite, shift.WorkersOffSite);
                //        }
                //    }
                //    else
                //    {
                //        totalWorkers += GetTotalWorkers(shift.WorkersOnSite, shift.WorkersOffSite);
                //    }
                //}

                return totalWorkers;
            }

            return 0;
        } // GetTotalWorkers
    }
}