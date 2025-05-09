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

        public async Task<bool> ValidatePasswordAsync(Guid id, string passwordHash)
        {
            return await _model
                .Where(m => m.ID == id && m.PasswordHash == passwordHash)
                .AnyAsync();
        } // ValidatePasswordAsync

        public new void Delete(User item)
        {
            _context.Database.ExecuteSqlCommand(
                "DELETE FROM UserRoles WHERE UserID = {0}", item.ID
            );

            _context.Entry(item).State = EntityState.Deleted;
        } // Delete

        // ROLES

        public async Task AddRoleAsync(Guid id, Guid roleID)
        {
            var _roleRepository = _context.Set<Role>(); // new RoleRepository(); // xBlaze 20250227: Cambios realizados porque en AuditAuditorRepository, no funcionó de la forma que aqui estaba

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The user to add role was not found");
            var itemRole = await _roleRepository.FindAsync(roleID) // _context.Roles.FindAsync(roleID)
                ?? throw new BusinessException("The role you are trying to add to the user was not found");

            if (foundItem.Roles.Contains(itemRole))
                throw new BusinessException("The role already was assigned to the user");
            
            foundItem.Roles.Add(itemRole);
        } // AddRoleAsync

        public async Task DelRoleAsync(Guid id, Guid roleID)
        {
            var _roleRepository = _context.Set<Role>();

            var foundItem = await _model.FindAsync(id)
                ?? throw new BusinessException("The user to delete role was not found");
            var itemRole = await _roleRepository.FindAsync(roleID)
                ?? throw new BusinessException("The role you are trying to delete from the user was not found");

            if (!foundItem.Roles.Contains(itemRole))
                throw new BusinessException("The role was not assigned to the user");
             
            foundItem.Roles.Remove(itemRole);
        } // DelRoleAsync
    }
}