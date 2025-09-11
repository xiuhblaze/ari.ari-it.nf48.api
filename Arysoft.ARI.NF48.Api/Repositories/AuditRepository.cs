using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AuditRepository : BaseRepository<Audit>
    {
        // GETS

        public new async Task<Audit> GetAsync(Guid id, bool asNoTracking = false)
        {
            var query = _model.AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query
                .Include(a => a.AuditAuditors)
                .Include(a => a.AuditStandards)
                .Include(a => a.AuditDocuments)
                .Include(a => a.Notes)
                .Include(a => a.Sites)
                .FirstOrDefaultAsync(m => m.ID == id);
        } // GetAsync

        public Audit GetNextAudit(Guid? ownerID, DateTime? initialDate, AuditNextAuditOwnerType owner)
        {  
            var items = _model
                .Include(a => a.AuditAuditors)
                .Include(a => a.AuditStandards)
                .Include(a => a.AuditDocuments)
                .Include(a => a.Notes)
                .Include(a => a.Sites);

            if (initialDate == null) initialDate = DateTime.Now;

            items = items.Where(a =>
                a.StartDate >= initialDate
                && a.Status != AuditStatusType.Nothing
                && a.Status < AuditStatusType.Canceled
                && a.AuditCycle.Status != StatusType.Nothing
            );

            if (ownerID != null && ownerID != Guid.Empty)
            {
                switch (owner)
                {
                    case AuditNextAuditOwnerType.Organization:
                        items = items.Where(a => a.AuditCycle.OrganizationID == ownerID);
                        break;
                    case AuditNextAuditOwnerType.AuditCycle:
                        items = items.Where(a => a.AuditCycleID == ownerID);
                        break;
                    case AuditNextAuditOwnerType.Auditor:
                        items = items.Where(a => a.AuditAuditors.Any(aa => aa.AuditorID == ownerID && aa.Status == StatusType.Active));
                        break;
                    default:
                        throw new BusinessException("Invalid owner type for next audit.");
                }
            }

            items = items.OrderBy(a => a.StartDate);

            return items.FirstOrDefault();
        } // GetNextAudit

        public async Task<bool> HasAuditorAnAudit(
            Guid auditorID, 
            DateTime startDate, 
            DateTime endDate, 
            Guid? auditExceptionID)
        {
            var items = _model
                .Include(a => a.AuditAuditors)
                .Where(a =>
                    a.AuditAuditors.Any(aa => 
                        aa.AuditorID == auditorID
                        && aa.Status == StatusType.Active)
                    && (a.StartDate <= endDate && a.EndDate >= startDate) 
                    && a.Status != AuditStatusType.Nothing 
                    && a.Status < AuditStatusType.Canceled
                );

            if (auditExceptionID != null && auditExceptionID != Guid.Empty)
            {
                items = items.Where(a => a.ID != auditExceptionID);
            }

            return await items.AnyAsync();
        } // HasAuditorAudit

        /// <summary>
        /// Permite saber si ya existe una auditoria en un ciclo con 
        /// el standard y step indicado
        /// </summary>
        /// <param name="auditCycleID"></param>
        /// <param name="standardID"></param>
        /// <param name="step"></param>
        /// <param name="auditExceptionID"></param>
        /// <returns></returns>
        public async Task<bool> IsAnyStandardStepAuditInAuditCycle(
            Guid auditCycleID,
            Guid standardID,
            AuditStepType step,
            Guid? auditExceptionID)
        {
            var items = _model
                .Include(a => a.AuditStandards)
                .Where(a =>
                    a.AuditCycleID == auditCycleID
                    && a.AuditStandards.Any(asd =>
                        asd.StandardID == standardID
                        && asd.Step == step
                        && asd.Status == StatusType.Active)
                    && a.Status != AuditStatusType.Nothing
                    && a.Status < AuditStatusType.Canceled
                );

            if (auditExceptionID != null && auditExceptionID != Guid.Empty)
            {
                items = items.Where(a => a.ID != auditExceptionID);
            }

            return await items.AnyAsync();
        } // IsAnyStandardStepAuditInAuditCycle

        // DELETES

        /// <summary>
        /// Elimina de forma directa con sentencias SQL porque con
        /// entityframwork no se puede borrar en cascada, se apendeja XD
        /// </summary>
        /// <param name="item"></param>
        public new void Delete(Audit item)
        {
            if (item.AuditAuditors != null && item.AuditAuditors.Count > 0)
            {
                foreach (var auditAuditor in item.AuditAuditors)
                {
                    _context.Database.ExecuteSqlCommand( // Para borrar en cascada la tabla intermedia
                        "DELETE FROM AuditAuditorsStandards WHERE AuditAuditorID = {0}", auditAuditor.ID);
                }
                _context.Database.ExecuteSqlCommand( 
                    "DELETE FROM AuditAuditors WHERE AuditID = {0}", item.ID);
            }

            if (item.AuditDocuments != null && item.AuditDocuments.Count > 0)
            {
                foreach (var auditDocument in item.AuditDocuments)
                {
                    _context.Database.ExecuteSqlCommand( // Para borrar en cascada la tabla intermedia
                        "DELETE FROM AuditDocumentsStandards WHERE AuditDocumentID = {0}", auditDocument.ID);
                }
                _context.Database.ExecuteSqlCommand(
                    "DELETE FROM AuditDocuments WHERE AuditID = {0}", item.ID);
            }

            if (item.AuditStandards != null && item.AuditStandards.Count > 0)
            {   
                _context.Database.ExecuteSqlCommand(
                    "DELETE FROM AuditStandards WHERE AuditID = {0}", item.ID);
            }

            if (item.Notes != null && item.Notes.Count > 0)
            {
                _context.Database.ExecuteSqlCommand( // Para borrar en cascada la tabla intermedia
                    "DELETE FROM Notes WHERE OwnerID = {0}", item.ID);
            }

            _context.Database.ExecuteSqlCommand( // Para borrar en cascada la tabla intermedia
                "DELETE FROM AuditSites WHERE AuditID = {0}", item.ID);

            _context.Database.ExecuteSqlCommand(
                "DELETE FROM Audits WHERE AuditID = {0}", item.ID);
            //_context.Entry(item).State = EntityState.Deleted;
        }

        public new async Task DeleteTmpByUserAsync(string username)
        {
            var items = await _model
                .Include(o => o.AuditAuditors)
                .Include(o => o.AuditStandards)
                .Include(o => o.AuditDocuments)
                .Include(o => o.Notes)
                .Where(m =>
                    m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                    && m.Status == AuditStatusType.Nothing
                ).ToListAsync();

            foreach (var item in items)
            {
                //_context.Database.ExecuteSqlCommand( // Para borrar en cascada la tabla intermedia
                //    "DELETE FROM AuditSites WHERE AuditID = {0}", item.ID);

                //_model.Remove(item);
                Delete(item); // Usar el método Delete para borrar en cascada
            }
        } // DeleteTmpByUserAsync

        // SITES

        public async Task AddSiteAsync(Guid id, Guid siteID)
        {
            var _siteRepository = _context.Set<Site>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The audit to add a site was not found");
            var itemSite = await _siteRepository.FindAsync(siteID)
                ?? throw new BusinessException("The site you are trying to relate to the audit was not found.");

            if (foundItem.Sites.Contains(itemSite))
                throw new BusinessException("The audit already has the site related.");

            foundItem.Sites.Add(itemSite);
        } // AddSiteAsync

        public async Task DelSiteAsync(Guid id, Guid siteID)
        {
            var _siteRepository = _context.Set<Site>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The audit to delete a site was not found");
            var itemSite = await _siteRepository.FindAsync(siteID)
                ?? throw new BusinessException("The site associated with the audit was not found when trying to delete it.");

            if (!foundItem.Sites.Contains(itemSite))
                throw new BusinessException("The site is not related to the audit.");

            foundItem.Sites.Remove(itemSite);
        } // DelSiteAsync
    }
}