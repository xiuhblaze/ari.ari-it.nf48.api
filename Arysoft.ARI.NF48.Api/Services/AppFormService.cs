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
    public class AppFormService
    {
        public readonly AppFormRepository _repository;

        // CONSTRUCTOR

        public AppFormService()
        {
            _repository = new AppFormRepository();
        } // AppFormService

        // METHODS

        public PagedList<AppForm> Gets(AppFormQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.OrganizationID.HasValue)
            {
                items = items.Where(m => m.OrganizationID == filters.OrganizationID);
            }

            if (filters.AuditCycleID.HasValue)
            {
                items = items.Where(m => m.AuditCycleID == filters.AuditCycleID);
            }

            if (filters.StandardID.HasValue)
            {
                items = items.Where(m => m.StandardID == filters.StandardID);
            }

            if (!string.IsNullOrWhiteSpace(filters.Text))
            {
                filters.Text = filters.Text.ToLower();
                items = items.Where(m => 
                    (m.ActivitiesScope != null && m.ActivitiesScope.ToLower().Contains(filters.Text))
                    || (m.ProcessServicesDescription != null && m.ProcessServicesDescription.ToLower().Contains(filters.Text))
                    || (m.LegalRequirements != null && m.LegalRequirements.ToLower().Contains(filters.Text))
                    || (m.CriticalComplaintComments != null && m.CriticalComplaintComments.ToLower().Contains(filters.Text))
                    || (m.AutomationLevel != null && m.AutomationLevel.ToLower().Contains(filters.Text))
                    || (m.DesignResponsibilityJustify != null && m.DesignResponsibilityJustify.ToLower().Contains(filters.Text))
                    || (m.CurrentCertificationsExpiration != null && m.CurrentCertificationsExpiration.ToLower().Contains(filters.Text))
                    || (m.CurrentStandards != null && m.CurrentStandards.ToLower().Contains(filters.Text))
                    || (m.CurrentCertificationsBy != null && m.CurrentCertificationsBy.ToLower().Contains(filters.Text) )
                    || (m.OutsourcedProcess != null && m.OutsourcedProcess.ToLower().Contains(filters.Text))
                    || (m.AnyConsultancyBy != null && m.AnyConsultancyBy.ToLower().Contains(filters.Text))
                    || (m.SalesComments != null && m.SalesComments.ToLower().Contains(filters.Text))
                    || (m.ReviewJustification != null && m.ReviewJustification.ToLower().Contains(filters.Text))
                    || (m.ReviewComments != null && m.ReviewComments.ToLower().Contains(filters.Text))
                    || (m.Organization != null && m.Organization.Name.ToLower().Contains(filters.Text))
                    || (m.Standard != null && m.Standard.Name.ToLower().Contains(filters.Text))
                    || (m.UserSales != null && m.UserSales.ToLower().Contains(filters.Text))
                    || (m.UserReviewer != null && m.UserReviewer.ToLower().Contains(filters.Text))
                    || (m.UserOrganization != null && m.UserOrganization.ToLower().Contains(filters.Text))
                );
            } // Text

            if (filters.Status.HasValue && filters.Status != AppFormStatusType.Nothing)
            {
                items = items.Where(m => m.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(i => i.Status != AppFormStatusType.Nothing)
                    : items.Where(i => i.Status != AppFormStatusType.Nothing
                        && i.Status != AppFormStatusType.Deleted);
            }

            // Order

            if (filters.Order.HasValue)
            {
                switch (filters.Order)
                {
                    case AppFormOrderType.Created:
                        items = items.OrderByDescending(m => m.Created);
                        break;
                    case AppFormOrderType.Organization:
                        items = items.OrderBy(m => m.Organization.Name);
                        break;
                    case AppFormOrderType.CreatedDesc:
                        items = items.OrderByDescending(m => m.Created);
                        break;
                    case AppFormOrderType.OrganizationDesc:
                        items = items.OrderByDescending(m => m.Organization.Name);
                        break;
                    default:
                        items = items.OrderByDescending(m => m.Created);
                        break;
                }
            }

            // Pagination

            var pagedItems = PagedList<AppForm>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        /// <summary>
        /// Get a single AppForm by ID
        /// </summary>
        /// <param name="id">Unique id</param>
        /// <returns></returns>
        public async Task<AppForm> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<AppForm> AddAsync(AppForm item)
        {
            // Validate

            if (string.IsNullOrEmpty(item.UpdatedUser))
                throw new BusinessException("UpdatedUser is required");

            // - Validar que la organizacion exista y esté activo
            // - Validar que el ciclo exista y esté activo

            // Set values

            item.ID = Guid.NewGuid();
            item.OrganizationID = item.OrganizationID;
            item.AuditCycleID = item.AuditCycleID;
            item.Status = AppFormStatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries

            try
            {
                await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
                _repository.Add(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AppFormService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<AppForm> UpdateAsync(AppForm item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validate

            if (string.IsNullOrEmpty(item.UpdatedUser))
                throw new BusinessException("UpdatedUser is required");

            // - Validaciones por status
            if (item.Status != foundItem.Status) { // El status cambió
                switch (item.Status)
                {
                    case AppFormStatusType.Nothing:
                        item.Status = AppFormStatusType.New;
                        break;

                    case AppFormStatusType.Send:
                        foundItem.UserOrganization = item.UpdatedUser;
                        break;

                    case AppFormStatusType.SalesReview:
                        item.SalesDate = DateTime.UtcNow;
                        foundItem.UserSales = item.UpdatedUser;
                        if (string.IsNullOrEmpty(item.SalesComments))
                            throw new BusinessException("Sales comments is required");
                        break;

                    case AppFormStatusType.SalesRejected:
                        item.SalesDate = DateTime.UtcNow;
                        foundItem.UserSales = item.UpdatedUser;
                        if (string.IsNullOrEmpty(item.SalesComments))
                            throw new BusinessException("Sales comments is required");
                        break;

                    case AppFormStatusType.ApplicantReview:
                        item.ReviewDate = DateTime.UtcNow;
                        foundItem.UserReviewer = item.UpdatedUser;
                        if (string.IsNullOrEmpty(item.ReviewJustification))
                            throw new BusinessException("Review justification is required");
                        break;
                    
                    case AppFormStatusType.ApplicantRejected:
                        item.ReviewDate = DateTime.UtcNow;
                        foundItem.UserReviewer = item.UpdatedUser;
                        if (string.IsNullOrEmpty(item.ReviewJustification))
                            throw new BusinessException("Review justification is required");
                        break;
                    //TODO: Falta validar cada caso
                }
            }

            // TODO: AuditLanguage - Validar que sea un idioma aceptado 'es', 'en'

            // Set values
            
            //foundItem.OrganizationID = item.OrganizationID;
            //foundItem.AuditCycleID = item.AuditCycleID;
            foundItem.StandardID = item.StandardID;

            // ISO 9K
            foundItem.ActivitiesScope = item.ActivitiesScope;
            foundItem.ProcessServicesCount = item.ProcessServicesCount;
            foundItem.ProcessServicesDescription = item.ProcessServicesDescription;
            foundItem.LegalRequirements = item.LegalRequirements;
            foundItem.AnyCriticalComplaint = item.AnyCriticalComplaint;
            foundItem.CriticalComplaintComments = item.CriticalComplaintComments;
            foundItem.AutomationLevel = item.AutomationLevel;
            foundItem.IsDesignResponsibility = item.IsDesignResponsibility;
            foundItem.DesignResponsibilityJustify = item.DesignResponsibilityJustify;

            // General
            foundItem.AuditLanguage = item.AuditLanguage;
            foundItem.CurrentCertificationsExpiration = item.CurrentCertificationsExpiration;
            foundItem.CurrentStandards = item.CurrentStandards;
            foundItem.CurrentCertificationsBy = item.CurrentCertificationsBy;
            foundItem.OutsourcedProcess = item.OutsourcedProcess;
            foundItem.AnyConsultancy = item.AnyConsultancy;
            foundItem.AnyConsultancyBy = item.AnyConsultancyBy;
            foundItem.SalesDate = item.SalesDate;   // TODO: Este va a cambiar de lugar cuando las validaciones
            foundItem.SalesComments = item.SalesComments;
            foundItem.ReviewDate = item.ReviewDate; // TODO: Este va a cambiar en cuanto se apliquen las validaciones
            foundItem.ReviewJustification = item.ReviewJustification;
            foundItem.ReviewComments = item.ReviewComments;

            foundItem.Status = item.Status;         // TODO: Validar los cambios de status
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Execute queries
            try
            {   
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AppFormService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(AppForm item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            if (foundItem.Status == AppFormStatusType.Deleted)
            {
                // TODO: Validar cuando es una eliminación física

                _repository.Delete(foundItem);
            }
            else
            { 
                foundItem.Status = foundItem.Status < AppFormStatusType.Cancel
                    ? AppFormStatusType.Cancel 
                    : AppFormStatusType.Deleted;
                foundItem.Updated = DateTime.UtcNow;
                foundItem.UpdatedUser = item.UpdatedUser;

                _repository.Update(foundItem);
            }

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AppFormService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync

        // NACE CODES

        public async Task AddNaceCodeAsync(Guid id, Guid naceCodeID)
        { 

            // HACK: Posiblemente aquí obtener los dos objetos y validar
            // - Que no se agregue un nace Inactivo
            // - No agregar naces a appsforms Canceladas, Eliminadas y ver que otros casos,
            //   tal vez de ciclos ya cerrados

            await _repository.AddNaceCodeAsync(id, naceCodeID); // Fuera del try-catch para enviar los errores hasta el cliente

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AppFormService.AddNaceCodeAsync: {ex.Message}");
            }
        } // AddNaceCodeAsync

        public async Task DelNaceCodeAsync(Guid id, Guid naceCodeID)
        {
            // Igual, considerar validaciones al quitar el nacecode

            await _repository.DelNaceCodeAsync(id, naceCodeID);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AppFormService.DelNaceCodeAsync: {ex.Message}");
            }
        } // DelNaceCodeAsync

        // CONTACTS

        public async Task AddContactAsync(Guid id, Guid contactID)
        {

            // Validar

            // - Que el contacto esté activo
            // - Que el contacto sea de la organización del app form
            await _repository.AddContactAsync(id, contactID);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AppFormService.AddContactAsync: {ex.Message}");
            }
        } // AddContactAsync

        public async Task DelContactAsync(Guid id, Guid contactID)
        {
            // Ver que validaciones se necesitan

            await _repository.DelContactAsync(id, contactID);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AppFormService.DelContactAsync: {ex.Message}");
            }
        } // DelContactAsync

        public async Task AddSiteAsync(Guid id, Guid siteID)
        {
            // Validar
            
            // - Que el sitio esté activo
            // - Que el sitio sea de la organización del app form

            await _repository.AddSiteAsync(id, siteID);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AppFormService.AddSiteAsync: {ex.Message}");
            }
        } // AddSiteAsync

        public async Task DelSiteAsync(Guid id, Guid siteID)
        {
            // Ver que validaciones se necesitan
            await _repository.DelSiteAsync(id, siteID);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AppFormService.DelSiteAsync: {ex.Message}");
            }
        } // DelSiteAsync
    }
}