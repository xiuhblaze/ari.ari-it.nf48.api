using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AppFormRepository : BaseRepository<AppForm>
    {
        //public new async Task<AppForm> GetAsync(Guid id, bool asNoTracking = false)
        //{
        //    var query = _model
        //        .Include(m => m.Organization)
        //        .Include(m => m.AuditCycle)
        //        .Include(m => m.Standard)
        //        .Include(m => m.Category22K)
        //        .Include(m => m.Contacts)
        //        .Include(m => m.NaceCodes)
        //        .Include(m => m.RiskLevels)
        //        .Include(m => m.Sites);

        //    if (asNoTracking)
        //        query = query.AsNoTracking();

        //    return await query.FirstOrDefaultAsync(m => m.ID == id);
        //} // GetAsync

        public async Task<Guid> GetAuditCycleIDAsync(Guid appFormID)
        {
            return await _model
                .Where(m => m.ID == appFormID)
                .Select(m => m.AuditCycleID)
                .FirstOrDefaultAsync();
        } // GetAuditCycleIDAsync

        /// <summary>
        /// Devuelve el siguiente año del cyclo
        /// </summary>
        /// <param name="auditCycleID"></param>
        /// <param name="periodicity"></param>
        /// <returns></returns>
        public async Task<CycleYearType> GetNextCycleYearAsync(
            Guid auditCycleID, 
            AuditCyclePeriodicityType periodicity
        )
        {
            var auditCycleRepository = _context.Set<AuditCycle>();

            var appFormsInCycle = await _model
                .Where(m => m.AuditCycleID == auditCycleID 
                    && m.Status > AppFormStatusType.Nothing
                    && m.Status < AppFormStatusType.Cancel
                ).OrderBy(m => m.CycleYear)
                .ToListAsync();

            if (appFormsInCycle.Count == 0)
                return CycleYearType.FirstYear;

            var lastAppForm = appFormsInCycle
                .OrderByDescending(m => m.CycleYear)
                .FirstOrDefault();

            CycleYearType nextCycleYear = CycleYearType.Nothing;

            if (lastAppForm != null)
            {
                if (
                    (lastAppForm.CycleYear == CycleYearType.ThirdYear 
                        && periodicity == AuditCyclePeriodicityType.Annual)
                    || (lastAppForm.CycleYear == CycleYearType.MiddleThirdYear
                        && periodicity == AuditCyclePeriodicityType.Biannual)
                ) return CycleYearType.Nothing;

                if (periodicity == AuditCyclePeriodicityType.Annual)
                {
                    nextCycleYear = (CycleYearType)((int)lastAppForm.CycleYear + 2);
                }
                else if (periodicity == AuditCyclePeriodicityType.Biannual)
                {
                    nextCycleYear = (CycleYearType)((int)lastAppForm.CycleYear + 1);
                }
            }

            return nextCycleYear;
        } // GetNextCycleYearAsync

        /// <summary>
        /// Devuelve el máximo nivel de riesgo de un AppForm, considerando solo los niveles 
        /// activos y distintos a 'Nothing' y considerando 1 el maximo y 5 el minimo
        /// </summary>
        /// <param name="appFormID">Identificador del AppForm a evaluar su nivel maximo de riesgo</param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<RiskLevelCategory> GetMaximumRiskLevelCategoryAsync(Guid appFormID)
        {
            var _appForm = await _model
                .Include(m => m.RiskLevels)
                .FirstOrDefaultAsync(m => m.ID == appFormID)
                ?? throw new BusinessException("The AppForm item not found");

            if (_appForm.RiskLevels == null || _appForm.RiskLevels.Count == 0)
                return RiskLevelCategory.Nothing;

            // Retorna el valor mínimo de la enumeración, que es el máximo riesgo
            return _appForm.RiskLevels
                .Where(rl => rl.Status == StatusType.Active 
                    && rl.Category != RiskLevelCategory.Nothing)
                .Min(m => m.Category) ?? RiskLevelCategory.Medium; 
        } // GetMaximumRiskLevelCategoryAsync

        /// <summary>
        /// Indica si existe algún AppForm válido en un ciclo de auditoría, 
        /// excluyendo un ID indicado
        /// </summary>
        /// <param name="auditCycleID"></param>
        /// <param name="exeptionID"></param>
        /// <returns></returns>
        public async Task<bool> ExistsValidAppFormAsync(
            Guid auditCycleID, 
            Guid? exeptionID = null
        )
        {
            var query = _model
                .Where(m => m.AuditCycleID == auditCycleID
                    && m.Status > AppFormStatusType.Nothing
                    && m.Status < AppFormStatusType.Inactive);

            if (exeptionID.HasValue)
            { 
                query = query.Where(m => m.ID != exeptionID.Value);
            }
                
            return await query.AnyAsync();
        } // ExistsValidAppFormAsync

        public async Task<bool> ExistsValidCycleYearAppForm(
            Guid auditCycleID, 
            CycleYearType cycleYear,
            Guid? exceptionID = null)
        {
            var query = _model
                .Where(m => m.AuditCycleID == auditCycleID
                    && m.CycleYear == cycleYear
                    && m.Status > AppFormStatusType.Nothing
                    && m.Status < AppFormStatusType.Inactive);

            if (exceptionID.HasValue)
            { 
                query = query.Where(m => m.ID != exceptionID.Value);
            }

            return await query.AnyAsync();
        } // ExistsValidCycleYearAppForm

        /// <summary>
        /// Marca como inactivos todos los AppForms de un ciclo de auditoría
        /// xBlaze: creo que no se va a necesitar, pues se valida que no exista un appForm activo
        /// antes de activar uno nuevo.
        /// </summary>
        /// <param name="auditCycleID"></param>
        /// <returns></returns>
        public async Task InactiveAllAppFormsFromAuditCycle(Guid auditCycleID)
        {
            var appForms = await _model
                .Where(m => m.AuditCycleID == auditCycleID
                    && m.Status > AppFormStatusType.Nothing
                    && m.Status < AppFormStatusType.Inactive)
                .ToListAsync();
            foreach (var appForm in appForms)
            {
                appForm.Status = AppFormStatusType.Inactive;
            }
        } // InactiveAllAppFormsFromAuditCycle

        public new void Delete(AppForm item)
        {
            _context.Database.ExecuteSqlCommand( // Para borrar en cascada la tabla intermedia
                "DELETE FROM AppFormsNaceCodes WHERE AppFormID = {0}", item.ID);

            _context.Database.ExecuteSqlCommand(
                "DELETE FROM AppFormsContacts WHERE AppFormID = {0}", item.ID);

            _context.Database.ExecuteSqlCommand(
                "DELETE FROM AppFormsSites WHERE AppFormID = {0}", item.ID);

            _context.Database.ExecuteSqlCommand(
                "DELETE FROM AppFormsRiskLevels WHERE AppFormID = {0}", item.ID);

            base.Delete(item);
        } // Delete

        // NACECODES

        public async Task AddNaceCodeAsync(Guid id, Guid naceCodeID)
        {
            var _naceCodeRepository = _context.Set<NaceCode>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The application form to add a NACE code was not found");
            if (foundItem.Status >= AppFormStatusType.Inactive)
                throw new BusinessException("The application form is not active");
            var naceCodeItem = await _naceCodeRepository.FindAsync(naceCodeID)
                ?? throw new BusinessException("The NACE code you're trying to relate to the application form was not found");

            if (foundItem.NaceCodes.Contains(naceCodeItem))
                throw new BusinessException("The application form already has the NACE code related");

            foundItem.NaceCodes.Add(naceCodeItem);
        } // AddNaceCodeAsync

        public async Task AddNaceCodeAsync(AppForm item, Guid naceCodeID)
        {
            var _naceCodeRepository = _context.Set<NaceCode>();
            
            //if (item.Status >= AppFormStatusType.Inactive)
            //    throw new BusinessException("The application form is not active");
            var naceCodeItem = await _naceCodeRepository.FindAsync(naceCodeID)
                ?? throw new BusinessException("The NACE code you're trying to relate to the application form was not found");

            //if (item.NaceCodes.Contains(naceCodeItem))
            //    throw new BusinessException("The application form already has the NACE code related");

            item.NaceCodes.Add(naceCodeItem);
        } // AddNaceCodeAsync

        public async Task DelNaceCodeAsync(Guid id, Guid naceCodeID)
        {
            var _naceCodeRepository = _context.Set<NaceCode>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The application form to remove a NACE code was not found");
            if (foundItem.Status >= AppFormStatusType.Inactive)
                throw new BusinessException("The application form is not active");
            var naceCodeItem = await _naceCodeRepository.FindAsync(naceCodeID)
                ?? throw new BusinessException("The NACE code you're trying to relate to the application form was not found");

            if (!foundItem.NaceCodes.Contains(naceCodeItem))
                throw new BusinessException("The NACE code is not related to the application form");

            foundItem.NaceCodes.Remove(naceCodeItem);
        } // DelNaceCodeAsync

        // CONTACTS

        public async Task AddContactAsync(Guid id, Guid contactID)
        {
            var _contactRepository = _context.Set<Contact>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The application form to add a Contact was not found");
            if (foundItem.Status >= AppFormStatusType.Inactive)
                throw new BusinessException("The application form is not active");
            var contactItem = await _contactRepository.FindAsync(contactID)
                ?? throw new BusinessException("The Contact you're trying to relate to the application form was not found");

            if (foundItem.Contacts.Contains(contactItem))
                throw new BusinessException("The application form already has the Contact related");

            foundItem.Contacts.Add(contactItem);
        } // AddContactAsync

        /// <summary>
        /// Uso particular para duplicar un AppForm con sus Contacts
        /// </summary>
        /// <param name="item"></param>
        /// <param name="contactID"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task AddContactAsync(AppForm item, Guid contactID)
        {
            var _contactRepository = _context.Set<Contact>();

            //if (item.Status >= AppFormStatusType.Inactive)
            //    throw new BusinessException("The application form is not active");
            var contactItem = await _contactRepository.FindAsync(contactID)
                ?? throw new BusinessException("The Contact you're trying to relate to the application form was not found");
            
            //if (item.Contacts.Contains(contactItem))
            //    throw new BusinessException("The application form already has the Contact related");
            
            item.Contacts.Add(contactItem);
        } // AddContactAsync

        public async Task DelContactAsync(Guid id, Guid contactID)
        {
            var _contactRepository = _context.Set<Contact>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The application form to remove a Contact was not found");
            if (foundItem.Status >= AppFormStatusType.Inactive)
                throw new BusinessException("The application form is not active");
            var contactItem = await _contactRepository.FindAsync(contactID)
                ?? throw new BusinessException("The contact related was not found");

            if (!foundItem.Contacts.Contains(contactItem))
                throw new BusinessException("The contact is not related to the application form");

            foundItem.Contacts.Remove(contactItem);
        } // DelContactAsync

        // SITES

        public async Task AddSiteAsync(Guid id, Guid siteID)
        {
            var _siteRepository = _context.Set<Site>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The application form to add a Site was not found");

            if (foundItem.Status >= AppFormStatusType.Inactive)
                throw new BusinessException("The application form is not active");
            var siteItem = await _siteRepository.FindAsync(siteID)
                ?? throw new BusinessException("The Site you're trying to relate was not found");

            //TODO: Ver si es necesario validar que el siteItem este activo

            if (foundItem.Sites != null && foundItem.Sites.Contains(siteItem))
                throw new BusinessException("The application form already has the Site related");

            if (foundItem.Sites == null)
                foundItem.Sites = new List<Site>();

            foundItem.Sites.Add(siteItem); // Aqui también marca una excepción pues 'Sites' es null
        } // AddSiteAsync

        /// <summary>
        /// Uso particular para duplicar un AppForm con sus Sites
        /// </summary>
        /// <param name="item"></param>
        /// <param name="siteID"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task AddSiteAsync(AppForm item, Guid siteID)
        {
            var _siteRepository = _context.Set<Site>();

            //if (item.Status >= AppFormStatusType.Inactive)
            //    throw new BusinessException("The application form is not active");
            var siteItem = await _siteRepository.FindAsync(siteID)
                ?? throw new BusinessException("The Site you're trying to relate was not found");

            //if (item.Sites.Contains(siteItem))
            //    throw new BusinessException("The application form already has the Site related");
            
            item.Sites.Add(siteItem);
        } // AddSiteAsync

        public async Task DelSiteAsync(Guid id, Guid siteID)
        {
            var _siteRepository = _context.Set<Site>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The application form to remove a Site was not found");            
            if (foundItem.Status >= AppFormStatusType.Inactive)
                throw new BusinessException("The application form is not active, can't be remove sites");
            var siteItem = await _siteRepository.FindAsync(siteID)
                ?? throw new BusinessException("The site related was not found");
            if (siteItem.IsMainSite)
                throw new BusinessException("The main site can't be removed from the application form");

            if (!foundItem.Sites.Contains(siteItem))
                throw new BusinessException("The site is not related to the application form");

            foundItem.Sites.Remove(siteItem);
        } // DelSiteAsync

        // RISKLEVELS

        public async Task AddRiskLevelAsync(Guid id, Guid riskLevelID)
        {
            var _riskLevelRepository = _context.Set<RiskLevel>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The application form to add a Risk Level was not found");
            if (foundItem.Status >= AppFormStatusType.Inactive)
                throw new BusinessException("The application form is not active");
            var riskLevelItem = await _riskLevelRepository.FindAsync(riskLevelID)
                ?? throw new BusinessException("The Risk Level you're trying to relate was not found");

            if (foundItem.StandardID != riskLevelItem.StandardID)
                throw new BusinessException("The Risk Level you're trying to relate has a different Standard than the application form");
            if (foundItem.RiskLevels.Contains(riskLevelItem))
                throw new BusinessException("The application form already has the Risk Level related");
            
            foundItem.RiskLevels.Add(riskLevelItem);
        } // AddRiskLevelAsync

        public async Task AddRiskLevelAsync(AppForm item, Guid riskLevelID)
        {
            var _riskLevelRepository = _context.Set<RiskLevel>();            
            var riskLevelItem = await _riskLevelRepository.FindAsync(riskLevelID)
                ?? throw new BusinessException("The Risk Level you're trying to relate was not found");

            item.RiskLevels.Add(riskLevelItem);
        } // AddRiskLevelAsync

        public async Task DelRiskLevelAsync(Guid id, Guid riskLevelID)
        {
            var _riskLevelRepository = _context.Set<RiskLevel>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The application form to remove a Risk Level was not found");
            if (foundItem.Status >= AppFormStatusType.Inactive)
                throw new BusinessException("The application form is not active");
            var riskLevelItem = await _riskLevelRepository.FindAsync(riskLevelID)
                ?? throw new BusinessException("The risk level related was not found");
            if (!foundItem.RiskLevels.Contains(riskLevelItem))
                throw new BusinessException("The risk level is not related to the application form");
            
            foundItem.RiskLevels.Remove(riskLevelItem);
        } // DelRiskLevelAsync

        // GENERAL

        public new async Task DeleteTmpByUserAsync(string username)
        {
            foreach (var item in await _model
                .Where(m => m.UpdatedUser.ToLower() == username.ToLower()
                    && m.Status == AppFormStatusType.Nothing
                ).ToListAsync())
            {
                _context.Database.ExecuteSqlCommand( // Para borrar en cascada la tabla intermedia
                    "DELETE FROM AppFormsNaceCodes WHERE AppFormID = {0}", item.ID);

                _context.Database.ExecuteSqlCommand(
                    "DELETE FROM AppFormsContacts WHERE AppFormID = {0}", item.ID);

                _context.Database.ExecuteSqlCommand(
                    "DELETE FROM AppFormsSites WHERE AppFormID = {0}", item.ID);

                _context.Database.ExecuteSqlCommand(
                    "DELETE FROM AppFormsRiskLevels WHERE AppFormID = {0}", item.ID);

                _model.Remove(item);
            }
        } // DeleteTmpByUserAsync
    }
}