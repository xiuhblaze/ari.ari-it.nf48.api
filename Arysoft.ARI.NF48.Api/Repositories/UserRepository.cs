using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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

        public async Task UpdateAsync(User item)
        {
            var currentItem = await _model.FindAsync(item.ID) ?? throw new Exception("The record to update was not found");

            currentItem.OrganizationID = item.OrganizationID;
            currentItem.ContactID = item.ContactID;
            currentItem.Username = item.Username;
            currentItem.PasswordHash = item.PasswordHash;
            currentItem.Email = item.Email;
            currentItem.FirstName = item.FirstName;
            currentItem.LastName = item.LastName;
            currentItem.Status = item.Status;
            currentItem.Updated = item.Updated;
            currentItem.UpdatedUser = item.UpdatedUser;

            Update(currentItem);
        } // UpdateAsync

        public async Task DeleteTmpByUser(string username)
        { 
            var items = await _model
                .Where(m => m.UpdatedUser.ToUpper() == username.ToUpper() && m.Status == Enumerations.StatusType.Nothing)
                .ToListAsync();

            foreach(var item in items)
            {
                _model.Remove(item);
            }
        }
    }
}