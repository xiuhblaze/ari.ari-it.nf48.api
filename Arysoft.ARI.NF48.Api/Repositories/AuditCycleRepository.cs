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
        /// Validar que no exista un ciclo entre dos fechas
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
        }

        public async Task<bool> IsAnyCycleBetweenDatesByOrganizationAndStandardAsync(
            Guid organizationID,
            Guid standardID,
            DateTime startDate,
            DateTime endDate
        )
        {
            return await _model
                .AnyAsync(m => m.OrganizationID == organizationID
                    && m.AuditCycleStandards.Any(s => s.StandardID == standardID)
                    && m.StartDate <= endDate
                    && m.EndDate >= startDate
                    && (m.Status == StatusType.Active
                        || m.Status == StatusType.Inactive)
                );
        }

        public async Task<bool> IsAnyCycleActiveByOrganizationAndStandardAsync(
            Guid organizationID,
            ICollection<AuditCycleStandard> standards
        )
        {
            return await _model
                .AnyAsync(m => m.OrganizationID == organizationID
                    && m.AuditCycleStandards.Any(acs => standards.Where(s => s.StandardID == acs.StandardID).Any())
                    && m.Status == StatusType.Active
                );
        }
    }
}