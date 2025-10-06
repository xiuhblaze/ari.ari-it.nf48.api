using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class AppFormRepository : BaseRepository<AppForm>
    {
        public async Task<Guid> GetAuditCycleIDAsync(Guid appFormID)
        {
            return await _model
                .Where(m => m.ID == appFormID)
                .Select(m => m.AuditCycleID)
                .FirstOrDefaultAsync();
        } // GetAuditCycleIDAsync

        public async Task<bool> IsThereValidAppFormAsync(Guid auditCycleID, Guid standardID)
        {
            return await _model
                .AnyAsync(m => m.AuditCycleID == auditCycleID
                    && m.StandardID == standardID
                    && m.Status > AppFormStatusType.Nothing
                    && m.Status < AppFormStatusType.Inactive);
        } // IsThereValidAppFormAsync

        public new void Delete(AppForm item)
        {
            _context.Database.ExecuteSqlCommand( // Para borrar en cascada la tabla intermedia
                "DELETE FROM AppFormsNaceCodes WHERE AppFormID = {0}", item.ID);

            _context.Database.ExecuteSqlCommand(
                "DELETE FROM AppFormsContacts WHERE AppFormID = {0}", item.ID);

            _context.Database.ExecuteSqlCommand(
                "DELETE FROM AppFormsSites WHERE AppFormID = {0}", item.ID);

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

            if (foundItem.Sites.Contains(siteItem))
                throw new BusinessException("The application form already has the Site related");

            foundItem.Sites.Add(siteItem);
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
                throw new BusinessException("The application form is not active");
            var siteItem = await _siteRepository.FindAsync(siteID)
                ?? throw new BusinessException("The site related was not found");

            if (!foundItem.Sites.Contains(siteItem))
                throw new BusinessException("The site is not related to the application form");

            foundItem.Sites.Remove(siteItem);
        } // DelSiteAsync

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

                _model.Remove(item);
            }
        } // DeleteTmpByUserAsync
    }
}