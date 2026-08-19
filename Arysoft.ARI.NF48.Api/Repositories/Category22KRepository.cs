using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class Category22KRepository : BaseRepository<Category22K>
    {
        /// <summary>
        /// Check if a Category22K with the same Category, SubCategory and Version exists 
        /// in the database, excluding the current item if withException is true.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="withException"></param>
        /// <returns></returns>
        public async Task<bool> ExistByCategorySubCategoryAsync(Category22K item, bool withException = true)
        { 
            Guid? exceptionID = withException ? (Guid?)item.ID : null;

            return await ExistByCategorySubCategoryAsync(
                item.Category, 
                item.SubCategory, 
                item.Version ?? Categories22KVersionType.Nothing,
                exceptionID);
        } // ExistByCategorySubCategory

        /// <summary>
        /// Check if a Category22K with the same Category, SubCategory and Version exists
        /// </summary>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <param name="version"></param>
        /// <param name="exceptionID"></param>
        /// <returns></returns>
        public async Task<bool> ExistByCategorySubCategoryAsync(string category, string subCategory, Categories22KVersionType version, Guid? exceptionID = null)
        {
            var query = _model
                .Where(m => 
                    m.Category.ToUpper() == category.ToUpper()
                    && m.SubCategory.ToUpper() == subCategory.ToUpper()
                    && m.Version == version);

            if (exceptionID != null && exceptionID != Guid.Empty)
            {
                query = query.Where(m => m.ID != exceptionID);
            }

            return await query.AnyAsync();
        } // ExistByCategorySubCategory

        public async Task<bool> ExistAssociatedAppFormsAsync(Guid category22KId)
        {
            return await _context.Set<AppForm>()
                .Where(m => m.Category22KID == category22KId)
                .AnyAsync();
        } // ExistAssociatedAppForms
    }
}