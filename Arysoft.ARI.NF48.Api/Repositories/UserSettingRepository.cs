using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class UserSettingRepository : BaseRepository<UserSetting>
    {
        public async Task<bool> IsUserSettingExistsAsync(Guid userID, Guid? exceptionUserSettingID = null)
        {
            var items = _model
                .Where(m => m.UserID == userID);

            if (exceptionUserSettingID != null && exceptionUserSettingID != Guid.Empty)
                items = items.Where(m => m.ID != exceptionUserSettingID);

            return await items.AnyAsync();
        } // IsUserSettingExists
    }
}