using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System.Collections.Generic;
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
                        .Sum(i => GetTotalWorkers(i));
                }
                else
                {
                    totalWorkers = site.Shifts
                        .Sum(i => GetTotalWorkers(i));
                }

                return totalWorkers;
            }

            return 0;
        } // GetTotalWorkers

        /// <summary>
        /// Obtiene el total de trabajadores de la lista de sitios especificados,
        /// se puede indicar del total o solo de los sitios y turnos activos (por default)
        /// </summary>
        /// <param name="sites">Lista de sitios a revisar</param>
        /// <param name="onlyActive">Indica si solo se deben de contar los activos (true)</param>
        /// <returns></returns>
        public static int GetTotalWorkers(List<Site> sites, bool onlyActive = true)
        {
            if (sites == null || sites.Count == 0)
                return 0;

            int totalWorkers = 0;

            if (onlyActive)
            {
                totalWorkers = sites.Where(i => i.Status == StatusType.Active)
                    .Sum(i => GetTotalWorkers(i, onlyActive));
            }
            else
            {
                totalWorkers = sites.Sum(i => GetTotalWorkers(i, onlyActive));
            }

            return totalWorkers;
        } // GetTotalWorkers

        // Workers On Site

        public static int GetWorkersOnSite(Site site, bool onlyActive = true)
        {
            if (site == null || site.Shifts == null || !site.Shifts.Any()) 
                return 0;

            int workersOnSite = 0;

            if (onlyActive)
            {
                workersOnSite = site.Shifts
                    .Where(i => i.Status == StatusType.Active)
                    .Sum(i => i.WorkersOnSite ?? 0);
            }
            else {
                workersOnSite = site.Shifts
                    .Sum(i => i.WorkersOnSite ?? 0);
            }

            return workersOnSite;
        } // GetWorkersOnSite

        // Workers Off Site

        public static int GetWorkersOffSite(Site site, bool onlyActive = true)
        {
            if (site == null || site.Shifts == null || !site.Shifts.Any())
                return 0;

            int workersOffSite = 0;

            if (onlyActive)
            {
                workersOffSite = site.Shifts
                    .Where(i => i.Status == StatusType.Active)
                    .Sum(i => i.WorkersOffSite ?? 0);
            }
            else
            {
                workersOffSite = site.Shifts
                    .Sum(i => i.WorkersOffSite ?? 0);
            }

            return workersOffSite;
        } // GetWorkersOnSite
    }
}