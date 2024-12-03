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

        public virtual async Task<T> GetAsync(Guid id)
        {
            return await _model                
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

        public void Delete(T item)
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

    }
}