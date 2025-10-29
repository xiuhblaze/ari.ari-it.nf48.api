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
        public new async Task<Proposal> GetAsync(Guid id, bool asNoTracking = false)
        {
            var query = _model.AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query
                .Include(m => m.AuditCycle)
                .Include("AuditCycle.Organization")
                .Include(m => m.ADCs)
                .Include(m => m.ProposalAudits)
                .Include(m => m.Notes)
                .FirstOrDefaultAsync(m => m.ID == id);
        } // GetAsync

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

        public async Task<bool> ExistsActiveProposalForAuditCycleAsync(Guid auditCycleId)
        {
            return await _model
                .Where(p => p.AuditCycleID == auditCycleId
                            && (p.Status >= ProposalStatusType.New
                                && p.Status <= ProposalStatusType.Active))
                .AnyAsync();
        } // ExistsActiveProposalForAuditCycleAsync

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
            var items = await _model
                .Include(m => m.ProposalAudits)
                .Include(m => m.Notes)
                .Where(m => 
                    m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                    && m.Status == ProposalStatusType.Nothing)
                .ToListAsync();

            foreach (var item in items)
            {
                _context.Database.ExecuteSqlCommand( // Para elimar la asocición con los ADCs
                    "UPDATE ADCs SET ProposalID = NULL WHERE ProposalID = {0}", item.ID);

                _model.Remove(item);
            }
        } // DeleteTmpByUserAsync
    }
}