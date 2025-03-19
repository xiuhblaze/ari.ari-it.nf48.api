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
        // NACECODES

        public async Task AddNaceCodeAsync(Guid id, Guid naceCodeID)
        {
            var _naceCodeRepository = _context.Set<NaceCode>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The application form to add a NACE code was not found");
            var naceCodeItem = await _naceCodeRepository.FindAsync(naceCodeID)
                ?? throw new BusinessException("The NACE code you're trying to relate to the application form was not found");

            if (foundItem.NaceCodes.Contains(naceCodeItem))
                throw new BusinessException("The application form already has the NACE code related");

            foundItem.NaceCodes.Add(naceCodeItem);
        } // AddNaceCodeAsync

        public async Task DelNaceCodeAsync(Guid id, Guid naceCodeID)
        {
            var _naceCodeRepository = _context.Set<NaceCode>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The application form to add a NACE code was not found");
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
            var contactItem = await _contactRepository.FindAsync(contactID)
                ?? throw new BusinessException("The Contact you're trying to relate to the application form was not found");

            if (foundItem.Contacts.Contains(contactItem))
                throw new BusinessException("The application form already has the Contact related");

            foundItem.Contacts.Add(contactItem);
        } // AddContactAsync

        // GENERAL

        public new async Task DeleteTmpByUserAsync(string username)
        {
            foreach (var item in await _model
                .Where(m => m.UpdatedUser.ToLower() == username.ToLower()
                    && m.Status == AppFormStatusType.Nothing
                ).ToListAsync())
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUserAsync
    }
}