using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class ApplicationService
    {
        private readonly ApplicationRepository _applicationRepository;

        // CONSTRUCTOR

        public ApplicationService()
        {
            _applicationRepository = new ApplicationRepository();
        }

        // METHODS

        public PagedList<Application> Gets(ApplicationQueryFilters filters)
        {
            var items = _applicationRepository.Gets();

            // filters

            if (filters.StandardID != null)
            {
                items = items.Where(i => i.StandardID == filters.StandardID);
            }

            if (filters.NaceCodeID != null)
            {
                items = items.Where(i => i.NaceCodeID == filters.NaceCodeID);
            }

            if (filters.RiskLevelID != null)
            {
                items = items.Where(i => i.RiskLevelID == filters.RiskLevelID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            { 
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(i => 
                    i.ProcessScope.ToLower().Contains(filters.Text)
                    || i.Services.ToLower().Contains(filters.Text)
                    || i.LegalRequirements.ToLower().Contains(filters.Text)
                    || i.CriticalComplaintComments.ToLower().Contains(filters.Text)
                    || i.AutomationLevel.ToLower().Contains(filters.Text)
                    || i.DesignResponsibilityJustify.ToLower().Contains(filters.Text)
                    || i.CurrentCertificationBy.ToLower().Contains(filters.Text)
                    || i.CurrentStandards.ToLower().Contains(filters.Text)
                    || i.OutsourcedProcess.ToLower().Contains(filters.Text)
                    || i.AnyConsultancyBy.ToLower().Contains(filters.Text)
                    || (i.Organization != null && i.Organization.Name.ToLower().Contains(filters.Text))
                    || (i.Standard != null && i.Standard.Name.ToLower().Contains(filters.Text))
                );
            }

            if (filters.AuditLanguage.HasValue)
            {
                items = items.Where(i => i.AuditLanguage == filters.AuditLanguage);
            }

            if (filters.Status.HasValue && filters.Status != ApplicationStatusType.Nothing)
            {
                items = items.Where(i => i.Status == filters.Status);
            }
            else
            { 
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(i => i.Status != ApplicationStatusType.Nothing)
                    : items.Where(i => i.Status != ApplicationStatusType.Nothing 
                        && i.Status != ApplicationStatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            {
                case ApplicationOrderType.Organization:
                    items = items.OrderBy(i => i.Organization.Name); 
                    break;
                case ApplicationOrderType.Created:
                    items = items.OrderBy(i => i.Created);
                    break;
                case ApplicationOrderType.OrganizationDesc:
                    items = items.OrderByDescending(i => i.Organization.Name);
                    break;
                case ApplicationOrderType.CreatedDesc:
                    items = items.OrderByDescending(i => i.Created);
                    break;
                default:
                    items = items.OrderByDescending(i => i.Created);
                    break;
            }

            // Pagination

            var pagedItems = PagedList<Application>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets 

        /// <summary>
        /// Gets a application register with the id gived
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Application> GetAsync(Guid id)
        { 
            return await _applicationRepository.GetAsync(id);
        } // GetAsync

        /// <summary>
        /// Add a application register with the updated user gived
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<Application> AddAsync(Application item)
        {
            if (string.IsNullOrEmpty(item.UpdatedUser))
                throw new BusinessException("User was not specified");

            // Assign values

            item.ID = Guid.NewGuid();
            item.Status = ApplicationStatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Excecute queries

            await _applicationRepository.DeleteTmpByUser(item.UpdatedUser);
            _applicationRepository.Add(item);
            await _applicationRepository.SaveChangesAsync();

            return item;
        } // AddAsync

        /// <summary>
        /// Update the values of a register with the 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<Application> UpdateAsync(Application item)
        {
            var foundItem = await _applicationRepository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // TODO: Validations

            // - Validar de acuerdo al tipo de Standard
            // - Validar de acuerdo al Status en el que se encuentra el Application Form

            // Assigning values

            foundItem.OrganizationID = item.OrganizationID;
            foundItem.StandardID = item.StandardID;
            foundItem.NaceCodeID = item.NaceCodeID;
            foundItem.RiskLevelID = item.RiskLevelID;
            foundItem.ProcessScope = item.ProcessScope;
            foundItem.NumProcess = item.NumProcess;
            foundItem.Services = item.Services;
            foundItem.LegalRequirements = item.LegalRequirements;
            foundItem.AnyCriticalComplaint = item.AnyCriticalComplaint;
            foundItem.CriticalComplaintComments = item.CriticalComplaintComments;
            foundItem.AutomationLevel = item.AutomationLevel;
            foundItem.IsDesignResponsibility = item.IsDesignResponsibility;
            foundItem.DesignResponsibilityJustify = item.DesignResponsibilityJustify;
            foundItem.AuditLanguage = item.AuditLanguage;
            foundItem.CurrentCertificationExpirationDate = item.CurrentCertificationExpirationDate;
            foundItem.CurrentCertificationBy = item.CurrentCertificationBy;
            foundItem.CurrentStandards = item.CurrentStandards;
            foundItem.TotalEmployes = item.TotalEmployes;
            foundItem.OutsourcedProcess = item.OutsourcedProcess;
            foundItem.AnyConsultancy = item.AnyConsultancy;
            foundItem.AnyConsultancyBy = item.AnyConsultancyBy;
            foundItem.Status = item.Status == ApplicationStatusType.Nothing
                ? ApplicationStatusType.New
                : item.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Excecute queries

            _applicationRepository.Update(foundItem);
            await _applicationRepository.SaveChangesAsync();

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Application item)
        {
            var foundItem = await _applicationRepository.GetAsync(item.ID)
                ?? throw new BusinessException("Item to delete was not found");

            if (foundItem.Status == ApplicationStatusType.Deleted)
            {
                _applicationRepository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status >= ApplicationStatusType.New && foundItem.Status <= ApplicationStatusType.Active
                    ? ApplicationStatusType.Cancel
                    : ApplicationStatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _applicationRepository.Update(foundItem);
            }

            await _applicationRepository.SaveChangesAsync();
        } // DeleteAsync

    }
}