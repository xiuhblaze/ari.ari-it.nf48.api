using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ProposalRepository : BaseRepository<Proposal>
    {
        // Validar que las Propuestas coincidan con los ADC
        // del AuditCycle

        // - Valida que el appform, auditcycle y la organization esten activos, o algo así
        public async Task<bool> HasValidParentsForCreateAsync(Proposal item)
        {
            var auditCycle = await _context.Set<AuditCycle>()
                .Include(ac => ac.Organization)
                .Where(ac => ac.ID == item.AuditCycleID)
                .FirstOrDefaultAsync();
            if (auditCycle == null) 
                return false;
            if (auditCycle.Status != StatusType.Active 
                && auditCycle.Status != StatusType.Inactive) 
                return false;
            if (auditCycle.Organization == null) 
                return false;
            if (auditCycle.Organization.Status != OrganizationStatusType.Applicant
                && auditCycle.Organization.Status !=  OrganizationStatusType.Active)
                return false;

            // xBlaze: Esto no porque al ser una nueva propuesta aun no cuenta con ADCs 
            // TODO: Validar esto cuando se quiera mandar a Review
            //var adcs = await _context.Set<ADC>()
            //    .Include(a => a.AppForm)
            //    .Where(a => a.ProposalID == item.ID)
            //    .ToListAsync();

            //foreach (var adc in adcs)
            //{
            //    if (adc.Status != ADCStatusType.Active) return false;
            //    if (adc.AppForm.Status != AppFormStatusType.Active) return false;
            //}

            return true;
        } // HasValidParentsAsync

        public async Task<bool> HasValidParentsForUpdateAsync(Proposal item)
        {
            var auditCycle = await _context.Set<AuditCycle>()
                .Include(ac => ac.Organization)
                .Where(ac => ac.ID == item.AuditCycleID)
                .FirstOrDefaultAsync();

            if (auditCycle == null 
                || (auditCycle.Status != StatusType.Active 
                    && auditCycle.Status != StatusType.Inactive))
                return false;
            if (auditCycle.Organization == null
                || (auditCycle.Organization.Status != OrganizationStatusType.Applicant
                    && auditCycle.Organization.Status != OrganizationStatusType.Active))
                return false;

            if (item.Status >= ProposalStatusType.Review 
                && item.Status < ProposalStatusType.Inactive)
            { 
                var adcs = await _context.Set<ADC>()
                    .Include(a => a.AppForm)
                    .Where(a => a.ProposalID == item.ID)
                    .ToListAsync();

                if (adcs.Any()) return false;

                foreach (var adc in adcs)
                {
                    if (adc.Status != ADCStatusType.Active) return false;
                    if (adc.AppForm.Status != AppFormStatusType.Active) return false;
                }
            }

            return true;
        } // HasValidParentsForUpdateAsync

        /// <summary>
        /// Elimina temporales creados por el usuario, funcion sobreescrita
        /// por tener un estatus diferente
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public new async Task DeleteTmpByUserAsync(string username)
        {
            foreach (var item in await _model
                .Where(m => m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                            && m.Status == ProposalStatusType.Nothing)
                .ToListAsync())
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUserAsync
    }
}