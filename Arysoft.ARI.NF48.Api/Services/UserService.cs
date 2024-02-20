using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly RoleRepository roleRepository;
        private AriContext db = new AriContext();

        // CONSTRUCTORS

        public UserService()
        {
            _userRepository = new UserRepository();
            roleRepository = new RoleRepository();
        }

        // METHODS

        public PagedList<User> Gets(UserQueryFilters filters)
        {
            var items = _userRepository.Gets();

            if (!string.IsNullOrEmpty(filters.Text))
            { 
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(e =>
                    e.Username.ToLower().Contains(filters.Text)
                    || e.Email.ToLower().Contains(filters.Text)
                    || e.FirstName.ToLower().Contains(filters.Text)
                    || e.LastName.ToLower().Contains(filters.Text)
                    || e.UpdatedUser.ToLower().Contains(filters.Text)
                );

            }

            if (filters.Status != null && filters.Status != StatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else { 
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != StatusType.Nothing)
                    : items.Where(e => e.Status != StatusType.Nothing 
                        && e.Status != StatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            {
                case UserOrderType.Username:
                    items = items.OrderBy(e => e.Username);
                    break;
                case UserOrderType.Email:
                    items = items.OrderBy(e => e.Email);
                    break;
                case UserOrderType.FirstName:
                    items = items.OrderBy(e => e.FirstName);
                    break;
                case UserOrderType.LastName:
                    items = items.OrderBy(e => e.LastName);
                    break;
                case UserOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case UserOrderType.UsernameDesc:
                    items = items.OrderByDescending(e => e.Username);
                    break;
                case UserOrderType.EmailDesc:
                    items = items.OrderByDescending(e => e.Email);
                    break;
                case UserOrderType.FirstNameDesc:
                    items = items.OrderByDescending(e => e.FirstName);
                    break;
                case UserOrderType.LastNameDesc:
                    items = items.OrderByDescending(e => e.LastName);
                    break;
                case UserOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.Username);
                    break;
            }

            // Pagination

            var pagedItems = PagedList<User>
                .Create(items, filters.PageNumber, filters.PageSize);
            
            return pagedItems;
        } // Gets

        public async Task<User> GetAsync(Guid id)
        {
            return await _userRepository.GetAsync(id);
        } // GetAsync

        public async Task<User> AddAsync(User item)
        {
            // Validations 

            if (string.IsNullOrEmpty(item.UpdatedUser))
            {
                throw new BusinessException("User was not specified");
            }

            // Assign values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.Now;
            item.Updated = DateTime.Now;

            // Execute queries

            await _userRepository.DeleteTmpByUser(item.UpdatedUser);
            _userRepository.Add(item);
            await _userRepository.SaveChangesAsync();

            return item;
        } // AddAsync

        public async Task<User> UpdateAsync(User item)
        {
            // Validations 

            //HACK: Validar que el usuario no este eliminado para validarlo
            if (await _userRepository.GetByUsername(item.Username, item.ID) != null)
            {
                throw new BusinessException("The username already exist.");
            }

            if (!string.IsNullOrEmpty(item.PasswordHash))
            {
                item.PasswordHash = Tools.Encrypt.GetSHA256(item.PasswordHash);
            }

            // Execute queries

            if (item.Status == StatusType.Nothing) item.Status = StatusType.Active;
            item.Updated = DateTime.Now;

            var updatedItem = await _userRepository.UpdateAsync(item);
            await _userRepository.SaveChangesAsync();

            return updatedItem;
        } // UpdateAsync

        /// <summary>
        /// Add a role to a user according to IDs
        /// </summary>
        /// <param name="id">User Id to add role</param>
        /// <param name="roleID">Role to add</param>
        /// <returns></returns>
        public async Task AddRoleAsync(Guid id, Guid roleID)
        {
            await _userRepository.AddRoleAsync(id, roleID);
            await _userRepository.SaveChangesAsync();            
        } // AddRoleAsync

        public async Task DeleteAsync(User item)
        {   
            var foundItem = await _userRepository.GetAsync(item.ID) 
                ?? throw new BusinessException("Item was not found");

            if (foundItem.Status == StatusType.Deleted)
            {
                _userRepository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status == StatusType.Active 
                    ? StatusType.Inactive 
                    : StatusType.Deleted;
                foundItem.Updated = DateTime.Now;
                foundItem.UpdatedUser = item.UpdatedUser;

                await _userRepository.UpdateAsync(foundItem);
            }

            await _userRepository.SaveChangesAsync();
        } // DeleteAsync
    }
}