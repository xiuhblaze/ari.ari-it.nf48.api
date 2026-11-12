using Arysoft.ARI.NF48.Api.Data;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class BaseRepository<T> : IDisposable where T : BaseModel
    {
        protected readonly AriContext _context;
        protected readonly DbSet<T> _model;
        private bool _disposed;

        // CONSTRUCTOR

        public BaseRepository()
        {
            _context = new AriContext();
            _model = _context.Set<T>();
        }

        // METHODS

        /// <summary>
        /// Query base SIN ejecutar. Los repositorios hijos pueden sobrescribirlo
        /// para agregar .Include() de las relaciones que siempre se necesitan.
        /// Los filtros, orden y paginación se traducen a SQL.
        /// </summary>
        public virtual IQueryable<T> Query()
        {
            return _model.AsQueryable();
        } // Query

        /// <summary>
        /// Se mantiene por compatibilidad con los servicios que aún no migran a Query().
        /// OJO: ejecuta el query completo y el resto de filtros corre en memoria.
        /// </summary>
        public virtual IEnumerable<T> Gets()
        {
            return Query().AsEnumerable();
        } // Gets

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
        } // GetAsync

        /// <summary>
        /// Carga explícitamente una navigation property de una entidad ya
        /// rastreada por el contexto (por ejemplo, recién insertada con new,
        /// que no es un proxy y por tanto no soporta lazy loading).
        /// </summary>
        public async Task LoadReferenceAsync<TProperty>(
            T item,
            Expression<Func<T, TProperty>> navigation) where TProperty : class
        {
            await _context.Entry(item).Reference(navigation).LoadAsync();
        } // LoadReferenceAsync

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

        // DISPOSE

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        } // Dispose
    }
}