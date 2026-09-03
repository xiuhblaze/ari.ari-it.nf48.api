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
                .Include(m => m.AuditCycle)
                .Include(m => m.AppForm)
                .Include("AppForm.Standard")
                .Include("AppForm.RiskLevels")
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
        /// Valida si hay un ADC disponible dada la organización indicada ya sea
        /// si no tiene registrada una Propuesta o si la Propuesta asociada está 
        /// en estado "0" (Nothing)
        /// </summary>
        /// <param name="organziationID">Identificador de la organización a revisar</param>
        /// <returns></returns>
        /// <remarks>
        /// Autor: xBlaze
        /// Creacion: 2026-01-28
        /// Ultima Modificacion: 2026-01-28
        /// </remarks>
        public async Task<int> CountADCsAvailableByOrganizationAsync(Guid organziationID)
        {
            var query = _model
                .Include(m => m.Proposal)
                .Include(m => m.AppForm)
                .Where(m => m.AppForm.OrganizationID == organziationID
                    && m.Status == ADCStatusType.Active
                    && (m.ProposalID == null
                        || (m.Proposal != null && m.Proposal.Status == 0)));

            return await query.CountAsync();
        } // CountADCsAvailableByOrganizationAsync

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
        /// Creacion: unknown
        /// Ultima Modificacion: unknown
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
        /// <param name="id">Identificador del ADC a consultar</param>
        /// <param name="cycleType">Tipo de ciclo de auditoria a comprobar</param>
        /// <returns></returns>
        /// <remarks>
        /// Autor: xBlaze
        /// Creacion: unknown
        /// Ultima Modificacion: 2026-02-04
        /// </remarks>
        public async Task<bool> IsAuditCycleTypeByADCID(Guid id, AuditCycleType cycleType)
        {
            var query = _model
                .Include(m => m.AuditCycle)
                .Where(m => m.ID == id);
            var adc = await query.FirstOrDefaultAsync();

            return adc.AuditCycle.CycleType == cycleType;
        } // IsAuditCycleInitialByADCID

        /// <summary>
        /// Obtiene el tipo de ciclo de auditoría asociado al ADCSiteAudit por el Standard del ADC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Autor: xBlaze
        /// Creacion: unkonwn
        /// Ultima Modificacion: unknown
        /// </remarks>
        public async Task<AuditCycleType> GetAuditCycleTypeByADCSiteAuditIDAsync(Guid id)
        {
            var query = _model
                .Include(m => m.ADCSites)
                .Include("ADCSites.ADCSiteAudits")
                .Include(m => m.AuditCycle)
                .Where(m => m.ADCSites
                    .Any(s => s.ADCSiteAudits
                        .Any(a => a.ID == id)));
            var adc = await query.FirstOrDefaultAsync();
            
            return adc.AuditCycle.CycleType ?? AuditCycleType.Nothing;
        } // GetAuditCycleTypeByADCIDAsync

        /// <summary>
        /// Obtiene el tipo de Ciclo de Auditoria asociado al ADCSite indicado por el ID
        /// </summary>
        /// <param name="adcSiteID"></param>
        /// <returns></returns>
        /// <remarks>
        /// Autor: xBlaze
        /// Creacion: 19-01-2026
        /// Ultima Modificacion: 19-01-2026
        /// </remarks>
        public async Task<AuditCycleType> GetAuditCycleTypeByADCSiteIDAsync(Guid adcSiteID)
        { 
            var query = _model
                .Include(m => m.ADCSites)
                .Include(m => m.AuditCycle)
                .Where(m => m.ADCSites
                    .Any(s => s.ID == adcSiteID));
            var adc = await query.FirstOrDefaultAsync();

            if (adc == null) return AuditCycleType.Nothing;

            return adc.AuditCycle.CycleType ?? AuditCycleType.Nothing;
        } // GetAuditCycleTypeByADCSiteIDAsync

        /// <summary>
        /// Marca el entidad ADC especificada como modificada en el contexto 
        /// de datos actual, preparándola para su actualización durante la 
        /// próxima operación de guardado.
        /// </summary>
        /// <param name="item">
        /// La entidad ADC a actualizar. Debe tener un identificador válido que 
        /// corresponda a una entidad existente en el contexto.
        /// </param>
        /// <remarks>
        /// Autor: xBlaze
        /// Creacion: unknow
        /// Ultima Modificacion: 21-01-2026
        /// </remarks>
        public void UpdateValues(ADC item)
        {
            var existing = _context.Set<ADC>().Local
                .FirstOrDefault(m => m.ID == item.ID);

            if (existing != null)
            {
                _context.Entry(existing).State = EntityState.Detached;
            }

            _context.Set<ADC>().Attach(item);
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