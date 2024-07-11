using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Repositories
{
    public class ApplicationRepository : BaseRepository<Application>
    {
        public override IEnumerable<Application> Gets()
        {
            return _model
                .Include(m => m.Organization) // Parece que asi va a jala Sites...
                .Include(m => m.Standard)
                .Include(m => m.NaceCode)
                .AsEnumerable();
        } // Gets

        public override async Task<Application> GetAsync(Guid id)
        {
            return await _model
                .Include(m => m.Organization)
                .Include(m => m.Standard)
                .Include(m => m.NaceCode)
                // .Include(m => m.RiskLevel)
                .Where(m => m.ID == id)
                .FirstOrDefaultAsync();
        } // GetAsync

        //public async Task<Application> UpdateAsync(Application item)
        //{
        //    var foundItem = await _model.FindAsync(item.ID)
        //        ?? throw new BusinessException("The record to update was not found");

        //    foundItem.OrganizationID = item.OrganizationID;
        //    foundItem.StandardID = item.StandardID;
        //    foundItem.NaceCodeID = item.NaceCodeID;
        //    foundItem.RiskLevelID = item.RiskLevelID;
        //    foundItem.ProcessScope = item.ProcessScope;
        //    foundItem.NumProcess = item.NumProcess;
        //    foundItem.Services = item.Services;
        //    foundItem.LegalRequirements = item.LegalRequirements;
        //    foundItem.AnyCriticalComplaint = item.AnyCriticalComplaint;
        //    foundItem.CriticalComplaintComments = item.CriticalComplaintComments;
	       // foundItem.AutomationLevel = item.AutomationLevel;
	       // foundItem.IsDesignResponsibility = item.IsDesignResponsibility;
	       // foundItem.DesignResponsibilityJustify = item.DesignResponsibilityJustify;
	       // foundItem.AuditLanguage = item.AuditLanguage;
	       // foundItem.CurrentCertificationExpirationDate = item.CurrentCertificationExpirationDate;
	       // foundItem.CurrentCertificationBy = item.CurrentCertificationBy;
	       // foundItem.CurrentStandards = item.CurrentStandards;
	       // foundItem.TotalEmployes = item.TotalEmployes;
	       // foundItem.OutsourcedProcess = item.OutsourcedProcess;
	       // foundItem.AnyConsultancy = item.AnyConsultancy;
	       // foundItem.AnyConsultancyBy = item.AnyConsultancyBy;
	       // foundItem.Status = item.Status;
            
        //    foundItem.Updated = item.Updated;
        //    foundItem.UpdatedUser = item.UpdatedUser;

        //    Update(foundItem);

        //    return foundItem;
        //} // UpdateAsync

        public async Task DeleteTmpByUser(string username)
        {
            var items = await _model
                .Where(m => m.UpdatedUser.ToLower() == username.ToLower()
                    && m.Status == ApplicationStatusType.Nothing
                ).ToListAsync();

            foreach (var item in items)
            {
                _model.Remove(item);
            }
        } // DeleteTmpByUser
    }
}