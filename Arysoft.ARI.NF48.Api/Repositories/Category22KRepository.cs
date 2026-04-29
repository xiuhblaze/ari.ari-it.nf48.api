using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class Category22KRepository : BaseRepository<Category22K>
    {
        public async Task<bool> ExistByCategorySubCategoryAsync(string category, string subCategory)
        {
            return await _model
                .Where(m => 
                    m.Category.ToUpper() == category.ToUpper()
                    && m.SubCategory.ToUpper() == subCategory.ToUpper())
                .AnyAsync();
        } // ExistByCategorySubCategory

        public async Task<bool> ExistAssociatedAppFormsAsync(Guid category22KId)
        {
            return await _context.Set<AppForm>()
                .Where(m => m.Category22KID == category22KId)
                .AnyAsync();
        } // ExistAssociatedAppForms
    }
}