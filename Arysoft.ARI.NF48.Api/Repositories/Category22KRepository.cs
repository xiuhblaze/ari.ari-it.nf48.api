using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
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

    //    public async Task DeleteTmpByUser(string username)
    //    { 
    //        var items = await _model
    //            .Where(m => m.UpdatedUser.ToLower() == username.ToLower().Trim()
    //                && m.Status == StatusType.Nothing)
    //            .ToListAsync();

    //        foreach (var item in items) { 
    //            _model.Remove(item);
    //        }
    //    } // DeleteTmpByUser
    }
}