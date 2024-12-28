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
    public class UserRepository : BaseRepository<User>
    {
        public override IEnumerable<User> Gets()
        {
            return _model
                .Include(m => m.Roles)
                .AsEnumerable();
        } // Gets

        public override async Task<User> GetAsync(Guid id)
        {
            return await _model
                .Include(m => m.Roles)
                .Where(m => m.ID == id)
                .FirstOrDefaultAsync();
        } // GetAsync

        public async Task<User> GetByUsername(string username, Guid? excludeID)
        { 
            return await _model
                .Include(m => m.Roles)
                .Where(m => 
                    m.Username.ToLower() == username.ToLower()
                    && (excludeID == null || m.ID != excludeID)
                )
                .FirstOrDefaultAsync();
        } // GetByUsername

        public async Task<User> GetUserByLoginAsync(string username, string passwordHash)
        {
            return await _model
                .Include(m => m.Roles)
                .Where(m => m.Username == username && m.PasswordHash == passwordHash)
                .FirstOrDefaultAsync();
        } // GetUserByLoginAsync

        public async Task AddRoleAsync(Guid id, Guid roleID)
        {
            var _roleRepository = new RoleRepository();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The user to add role was not found");
            var itemRole = await _roleRepository.GetAsync(roleID) // _context.Roles.FindAsync(roleID)
                ?? throw new BusinessException("The role you are trying to add to the user was not found");

            if (!foundItem.Roles.Contains(itemRole))
            {
                foundItem.Roles.Add(itemRole);
            }
            else throw new BusinessException("The role already was assigned to the user");
        } // AddRoleAsync

        //public async Task DeleteTmpByUser(string username)
        //{ 
        //    var items = await _model
        //        .Where(m => 
        //            m.UpdatedUser.ToUpper() == username.ToUpper() 
        //            && m.Status == StatusType.Nothing
        //        ).ToListAsync();

        //    foreach(var item in items)
        //    {
        //        _model.Remove(item);
        //    }
        //} // DeleteTmpByUser
    }
}