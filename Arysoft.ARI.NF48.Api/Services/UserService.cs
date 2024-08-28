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
        
        // CONSTRUCTORS

        public UserService()
        {
            _userRepository = new UserRepository();;
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

            if (filters.Type != null && filters.Type != UserType.Nothing)
            {
                items = items.Where(e => e.Type == filters.Type);
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
                case UserOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case UserOrderType.UsernameDesc:
                    items = items.OrderByDescending(e => e.Username);
                    break;
                case UserOrderType.EmailDesc:
                    items = items.OrderByDescending(e => e.Email);
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

        public async Task<User> LoginAsync(string username, string password)
        { 
            string passwordHash = Tools.Encrypt.GetSHA256(password);

            var foundItem = await _userRepository.GetUserByLoginAsync(username, passwordHash)
                ?? throw new BusinessException("The username or password is not valid");

            return foundItem;
        } // LoginAsync

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
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries

            await _userRepository.DeleteTmpByUser(item.UpdatedUser);
            _userRepository.Add(item);
            await _userRepository.SaveChangesAsync();

            return item;
        } // AddAsync

        /// <summary>
        /// Actualiza la información de un registro en la base de datos
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<User> UpdateAsync(User item)
        {
            var foundItem = await _userRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations 

            if (string.IsNullOrEmpty(item.FirstName))
                throw new BusinessException("Must first name specify");

            if (string.IsNullOrEmpty(item.Email))
                throw new BusinessException("Must email an specify");

            if (item.Type == UserType.Nothing)
                throw new BusinessException("Must user type specify");

            var sameNameUser = await _userRepository.GetByUsername(item.Username, item.ID);
            if (sameNameUser != null)
            {
                if (sameNameUser.Status == StatusType.Deleted)
                    throw new BusinessException("The username was used but deleted.");
                else
                    throw new BusinessException("The username already exist.");
            }

            // Assigning values

            foundItem.OwnerID = item.OwnerID;            
            foundItem.Username = item.Username;
            foundItem.PasswordHash = string.IsNullOrEmpty(item.PasswordHash)
                ? foundItem.PasswordHash
                : Tools.Encrypt.GetSHA256(item.PasswordHash);
            foundItem.Email = item.Email;
            foundItem.FirstName = item.FirstName;
            foundItem.LastName = item.LastName;
            foundItem.Type = item.Type;
            foundItem.Status = item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Execute queries
            
            _userRepository.Update(foundItem);
            await _userRepository.SaveChangesAsync();

            return foundItem;
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
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _userRepository.Update(foundItem);
            }

            await _userRepository.SaveChangesAsync();
        } // DeleteAsync
    }
}