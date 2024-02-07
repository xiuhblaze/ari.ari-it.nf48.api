using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public async Task<User> GetByUsername(string username, Guid? excludeID)
        { 
            return await _model
                .Where(m => 
                    m.Username.ToLower() == username.ToLower()
                    && (excludeID == null || m.ID != excludeID)
                )
                .FirstOrDefaultAsync();
        } // GetByUsername

        public async Task<User> UpdateAsync(User item)
        {
            var foundItem = await _model.FindAsync(item.ID) 
                ?? throw new BusinessException("The record to update was not found");

            foundItem.OrganizationID = item.OrganizationID;
            foundItem.ContactID = item.ContactID;
            foundItem.Username = item.Username;
            foundItem.PasswordHash = string.IsNullOrEmpty(item.PasswordHash) 
                ? foundItem.PasswordHash
                : item.PasswordHash;
            foundItem.Email = item.Email;
            foundItem.FirstName = item.FirstName;
            foundItem.LastName = item.LastName;
            foundItem.Status = item.Status;
            foundItem.Updated = item.Updated;
            foundItem.UpdatedUser = item.UpdatedUser;

            Update(foundItem);

            return foundItem;
        } // UpdateAsync

        public async Task DeleteTmpByUser(string username)
        { 
            var items = await _model
                .Where(m => 
                    m.UpdatedUser.ToUpper() == username.ToUpper() 
                    && m.Status == Enumerations.StatusType.Nothing
                ).ToListAsync();

            foreach(var item in items)
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUser
    }
}