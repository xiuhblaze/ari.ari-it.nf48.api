using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ADCRepository : BaseRepository<ADC>
    {
        public new async Task<ADC> GetAsync(Guid id, bool asNoTracking = false)
        {
            var query = _model.AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query
                .Include(m => m.AppForm)
                .Include(m => m.ADCSites)
                .Include("ADCSites.Site")
                .Include("ADCSites.Site.Shifts")
                .Include("ADCSites.ADCSiteAudits")
                .Include("ADCSites.ADCConceptValues")
                .Include(m => m.Notes)
                .FirstOrDefaultAsync(m => m.ID == id);
        } // GetAsync

        public async Task<IEnumerable<ADC>> GetsByProposalAsync(Guid proposalID)
        {
            var query = _model
                .Include(m => m.ADCSites)
                .Include("ADCSites.ADCSiteAudits")
                .Where(m => m.ProposalID == proposalID);

            return await query.ToListAsync();
        } // GetsByProposalAsync

        public async Task<int> CountADCsAvailableByAuditCycleAsync(Guid auditCycleID)
        {
            var query = _model
                .Include(m => m.Proposal)
                .Where(m => m.AuditCycleID == auditCycleID
                    && m.Status == ADCStatusType.Active
                    && (m.ProposalID == null 
                        || (m.Proposal != null && m.Proposal.Status == 0)));

            return await query.CountAsync();
        } // CountADCsByAuditCycle

        /// <summary>
        /// Obtiene el ID de un ADC disponible (sin propuesta asignada)
        /// </summary>
        /// <param name="auditCycleID"></param>
        /// <returns></returns>
        public async Task<Guid> GetADCIDAvailableByAuditCycleAsync(Guid auditCycleID)
        {
            var query = _model
                .Include(m => m.Proposal)
                .Where(m => m.AuditCycleID == auditCycleID
                    && m.Status == ADCStatusType.Active
                    && (m.ProposalID == null
                        || (m.Proposal != null && m.Proposal.Status == 0)));
            var adc = await query.FirstOrDefaultAsync();

            return adc != null ? adc.ID : Guid.Empty;
        } // GetADCIDAvailableByAuditCycleAsync

        public async Task<ADC> GetADCAvailableByAuditCycleAsync(Guid auditCycleID)
        {
            var query = _model
                .Where(m => m.AuditCycleID == auditCycleID
                    && m.Status == ADCStatusType.Active
                    && m.ProposalID == null);
            //var adc = await query.FirstOrDefaultAsync();

            return await query.FirstOrDefaultAsync();
        } // GetADCIDAvailableByAuditCycleAsync

        /// <summary>
        /// Indica si el ADC asociado al ADCSiteAudit incluye pre-auditoría
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Autor: xBlaze
        /// Creacion: 2024-06-12
        /// Ultima Modificacion: 2024-06-12
        /// </remarks>
        public async Task<bool> IncludePreAuditByADCSiteAuditIDAsync(Guid id)
        { 
            var query = _model
                .Include(m => m.ADCSites)
                .Include("ADCSites.ADCSiteAudits")
                .Where(m => m.ADCSites
                    .Any(s => s.ADCSiteAudits
                        .Any(a => a.ID == id)));

            var adc = await query.FirstOrDefaultAsync();

            return adc.IncludePreAudit ?? false;
        } // IncludePreAuditByADCSiteAuditIDAsync

        /// <summary>
        /// Indica si el tipo de ciclo de auditoría del ADC es el indicado
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cycleType"></param>
        /// <returns></returns>
        /// <remarks>
        /// Autor: xBlaze
        /// Creacion: 2024-06-12
        /// Ultima Modificacion: 2024-06-12
        /// </remarks>
        public async Task<bool> IsAuditCycleTypeByADCID(Guid id, AuditCycleType cycleType)
        {
            var query = _model
                .Include(m => m.AuditCycle)
                .Include("AuditCycle.AuditCycleStandards")
                .Where(m => m.ID == id);
            var adc = await query.FirstOrDefaultAsync();
            var auditCycleStandard = adc.AuditCycle
                .AuditCycleStandards.Where(acs => acs.StandardID == adc.StandardID)
                .FirstOrDefault();

            return auditCycleStandard.CycleType == cycleType;
        } // IsAuditCycleInitialByADCID

        /// <summary>
        /// Obtiene el tipo de ciclo de auditoría asociado al ADCSiteAudit por el Standard del ADC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Autor: xBlaze
        /// Creacion: 2024-06-12
        /// Ultima Modificacion: 2024-06-12
        /// </remarks>
        public async Task<AuditCycleType> GetAuditCycleTypeByADCSiteAuditIDAsync(Guid id)
        {
            var query = _model
                .Include(m => m.ADCSites)
                .Include("ADCSites.ADCSiteAudits")
                .Include(m => m.AuditCycle)
                .Include("AuditCycle.AuditCycleStandards")
                .Where(m => m.ADCSites
                    .Any(s => s.ADCSiteAudits
                        .Any(a => a.ID == id)));
            var adc = await query.FirstOrDefaultAsync();
            var auditCycleStandard = adc.AuditCycle
                .AuditCycleStandards.Where(acs => acs.StandardID == adc.StandardID)
                .FirstOrDefault();

            return auditCycleStandard.CycleType ?? AuditCycleType.Nothing;
        } // GetAuditCycleTypeByADCIDAsync

        public void UpdateValues(ADC item)
        {
            var existing = _context.Set<ADC>().Local
                .FirstOrDefault(m => m.ID == item.ID);

            if (existing != null)
            {
                _context.Entry(existing).State = EntityState.Detached;
            }

            _context.Entry(item).State = EntityState.Modified;
        } // UpdateValues

        // TODO: Creo que este no se va a necesitar, revisar!
        public async Task<bool> AppFormHasValidADCAsync(Guid appFormID)
        { 
            return await _model
                .AnyAsync(m => m.AppFormID == appFormID
                    && m.Status > ADCStatusType.Nothing
                    && m.Status < ADCStatusType.Cancel);
        } // AppFormHasValidADCAsync

        public new async Task DeleteTmpByUserAsync(string username)
        {
            foreach (var item in await _model
                .Where(m => m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                            && m.Status == ADCStatusType.Nothing)
                .ToListAsync())
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUserAsync

        public void DetachAllEntities()
        { 
            var changedEntriesCopy = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted ||
                            e.State == EntityState.Unchanged)
                .ToList();
            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        } // DetachAllEntities
    }
}