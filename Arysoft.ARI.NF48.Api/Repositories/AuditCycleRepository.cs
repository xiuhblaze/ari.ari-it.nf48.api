using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditCycleRepository : BaseRepository<AuditCycle>
    {
        /// <summary>
        /// Permite inactivar los ciclos de auditoría de una organización para 
        /// solo permitir uno activo.
        /// </summary>
        /// <param name="organizationID">Identificador de la organización</param>
        /// <returns></returns>
        public async Task SetInactiveByOrganizationID(Guid organizationID)
        {
            var items = await _model
                .Where(m => m.OrganizationID == organizationID
                    && m.Status == StatusType.Active)
                .ToListAsync();

            foreach (var item in items)
            {
                item.Status = StatusType.Inactive;
                Update(item);
            }
        } // SetInactiveByOrganizationID

        /// <summary>
        /// Validar que no exista un ciclo entre dos fechas del mismo standard y organización
        /// </summary>
        /// <param name="organizationID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<bool> IsAnyCycleBetweenDatesByOrganizationAsync(
            Guid organizationID,
            DateTime startDate,
            DateTime endDate
        )
        {
            return await _model
                .AnyAsync(m => m.OrganizationID == organizationID
                    && m.StartDate <= endDate
                    && m.EndDate >= startDate
                    && (m.Status == StatusType.Active
                        || m.Status == StatusType.Inactive)
                );
        } // IsAnyCycleBetweenDatesByOrganizationAsync

        public async Task<bool> IsAnyCycleBetweenDatesByOrganizationAndStandardAsync(
            Guid organizationID,
            Guid standardID,
            DateTime startDate,
            DateTime endDate
        )
        {
            return await _model
                .AnyAsync(m => m.OrganizationID == organizationID
                    && m.StandardID == standardID
                    && m.StartDate <= endDate
                    && m.EndDate >= startDate
                    && (m.Status == StatusType.Active
                        || m.Status == StatusType.Inactive)
                );
        } // IsAnyCycleBetweenDatesByOrganizationAndStandardAsync

        //public async Task<bool> IsAnyCycleActiveByOrganizationAndStandardAsync(
        //    Guid organizationID,
        //    ICollection<AuditCycleStandard> standards,
        //    Guid? exceptionID
        //)
        //{
        //    return await _model
        //        .AnyAsync(m => m.OrganizationID == organizationID
        //            && m.AuditCycleStandards.Any(acs => standards.Where(s => s.StandardID == acs.StandardID).Any())
        //            && m.Status == StatusType.Active
        //        );
        //}

        public async Task<bool> IsAnyCycleActiveByOrganizationAndStandardAsync(
            Guid organizationID,
            Guid standardID,
            Guid? exceptionID
        )
        {
            var query = _model
                .Where(m => m.OrganizationID == organizationID
                    && m.StandardID == standardID
                    && m.Status == StatusType.Active);

            if (exceptionID != null && exceptionID != Guid.Empty)
            {
                query = query.Where(m => m.ID != exceptionID);
            }

            //foreach (var ac in auditCycles)
            //{
            //    foreach (var acs in ac.AuditCycleStandards.Where(acs => acs.Status == StatusType.Active))
            //    {
            //        foreach (var s in standards)
            //        {
            //            if (s.StandardID == acs.StandardID)
            //            {
            //                return true;
            //            }
            //        }
            //    }
            //}

            return await query.AnyAsync();
        }

    }
}