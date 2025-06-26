using Arysoft.ARI.NF48.Api.Data;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class BaseRepository<T> where T : BaseModel
    {
        protected readonly AriContext _context;
        protected readonly DbSet<T> _model;

        // CONSTRUCTOR

        public BaseRepository()
        {
            _context = new AriContext();
            _model = _context.Set<T>();
        }

        // METHODS

        public virtual IEnumerable<T> Gets()
        {
            return _model
                .AsEnumerable();
                // .AsNoTracking()
        }

        /// <summary>
        /// Obtiene un registro por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="asNoTracking">Para que en una llamada en un mismo
        ///   metodo se haga o no seguimiento de los cambios
        /// </param>
        /// <returns></returns>
        public virtual async Task<T> GetAsync(Guid id, bool asNoTracking = false)
        {
            var query = _model.AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query
                .FirstOrDefaultAsync(m => m.ID == id);
        }

        public void Add(T item)
        { 
            _model.Add(item);
        }

        public void Update(T item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }

        public virtual void Delete(T item)
        {
            _context.Entry(item).State = EntityState.Deleted;
            //_model.Remove(item);
        }

        public void SaveChanges()
        { 
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        { 
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina todos los registros temporales creados por el usuario
        /// </summary>
        /// <param name="username">Nombre del usuario</param>
        /// <returns></returns>
        public virtual async Task DeleteTmpByUserAsync(string username)
        {
            var items = await _model
                .Where(m =>
                    m.UpdatedUser.ToUpper() == username.ToUpper().Trim()
                    && m.Status == StatusType.Nothing
                ).ToListAsync();

            foreach (var item in items)
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUser
    }
}