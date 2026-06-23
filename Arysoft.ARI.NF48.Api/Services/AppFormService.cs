using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using Arysoft.ARI.NF48.Api.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                    || (m.AutomationLevelJustification != null && m.AutomationLevelJustification.ToLower().Contains(filters.Text))
                    || (m.DesignResponsibilityJustify != null && m.DesignResponsibilityJustify.ToLower().Contains(filters.Text))
                    || (m.OperationalControls != null && m.OperationalControls.ToLower().Contains(filters.Text))
                    || (m.CurrentCertificationsExpiration != null && m.CurrentCertificationsExpiration.ToLower().Contains(filters.Text))
                    || (m.CurrentStandards != null && m.CurrentStandards.ToLower().Contains(filters.Text))
                    || (m.CurrentCertificationsBy != null && m.CurrentCertificationsBy.ToLower().Contains(filters.Text) )
                    || (m.OutsourcedProcess != null && m.OutsourcedProcess.ToLower().Contains(filters.Text))
                    || (m.AnyConsultancyBy != null && m.AnyConsultancyBy.ToLower().Contains(filters.Text))
                    || (m.ReviewJustification != null && m.ReviewJustification.ToLower().Contains(filters.Text))
                    || (m.Organization != null && m.Organization.Name.ToLower().Contains(filters.Text))
                    || (m.Standard != null && m.Standard.Name.ToLower().Contains(filters.Text))
                    || (m.UserSales != null && m.UserSales.ToLower().Contains(filters.Text))
                    || (m.UserReviewer != null && m.UserReviewer.ToLower().Contains(filters.Text))
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

            foreach (var item in items)
            {
                if (item.Status < AppFormStatusType.Inactive)
                {
                    item.Alerts = GetAlertsAsync(item)
                        .GetAwaiter()
                        .GetResult();
                }
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
                    case AppFormOrderType.CycleYear:
                        items = items.OrderBy(m => m.CycleYear);
                        break;
                    case AppFormOrderType.CreatedDesc:
                        items = items.OrderByDescending(m => m.Created);
                        break;
                    case AppFormOrderType.OrganizationDesc:
                        items = items.OrderByDescending(m => m.Organization.Name);
                        break;
                    case AppFormOrderType.CycleYearDesc:
                        items = items.OrderByDescending(m => m.CycleYear);
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
        public async Task<AppForm> GetAsync(Guid id, bool asNoTracking = false)
        {
            var item = await _repository.GetAsync(id, asNoTracking)
                ?? throw new BusinessException("The record was not found");

            if (item.Status < AppFormStatusType.Inactive)
            {
                item.Alerts = await GetAlertsAsync(item);
            }

            return item; //await _repository.GetAsync(id);
        } // GetAsync

        /// <summary>
        /// Obtiene un listado de todos los Scopes de los AppForm asociadas las ADCs 
        /// asociadas a una propuesta, dada por el ID
        /// </summary>
        /// <param name="id">Identificador de la Propuesta</param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<List<string>> GetsScopesByProposalID(Guid id)
        {
            var proposalRepository = new ProposalRepository();
            var proposalItem = await proposalRepository.GetAsync(id)
                ?? throw new BusinessException("The proposal record was not found");

            var scopes = proposalItem.ADCs
                .Where(adc => adc.AppForm != null)
                .Select(adc => adc.AppForm.ActivitiesScope)
                .ToList();

            return scopes;
        } // GetsScopesByProposalID

        /// <summary>
        /// Genera un registro con la información minima necesaria y en base a ella
        /// Obtiene el siguiente ciclo disponible para el standard seleccionado
        /// </summary>
        /// <param name="item">Elemento con los datos minimos para crear un registro</param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<AppForm> AddAsync(AppForm item)
        {
            var _auditCycleRepository = new AuditCycleRepository();

            var auditCycle = await _auditCycleRepository.GetAsync(item.AuditCycleID)
                ?? throw new BusinessException("The selected Audit Cycle was not found");

            await ValidateCreateAppFormAsync(item, auditCycle);

            // Set values

            var nextCycleYear = await _repository.GetNextCycleYearAsync(
                item.AuditCycleID, 
                auditCycle.Periodicity ?? AuditCyclePeriodicityType.Nothing
            );

            item.ID = Guid.NewGuid();
            item.StandardID = auditCycle.StandardID;
            item.CycleYear = nextCycleYear;
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

            item = await _repository.GetAsync(item.ID, true); // Obtener el item con las relaciones cargadas
            item = await AddMainSiteAsync(item); // Agregar el sitio principal al appform

            return item;
        } // AddAsync

        public async Task<AppForm> UpdateAsync(AppForm item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validate

            await ValidateAppFormAsync(item, foundItem);

            // - Asignaciones por status

            // Validar si el CycleYear es valido y no está duplicado - Movido a ValidateAppFormAsync 
            //if (await _repository.ExistsValidCycleYearAppForm(
            //        foundItem.AuditCycleID, 
            //        item.CycleYear ?? CycleYearType.Nothing,
            //        item.ID))
            //    throw new BusinessException("The selected Cycle Year is already assigned to another Application Form in the current certificate cycle");

            if (item.Status == AppFormStatusType.Nothing 
                || item.Status == AppFormStatusType.SalesReview // xBlaze 20250424: Estos dos últimos para evitar que se utilicen - en el futuro se podrian necesitar
                || item.Status == AppFormStatusType.SalesRejected)
                item.Status = AppFormStatusType.New;

            if (item.Status != foundItem.Status) // El status cambió
            {
                switch (item.Status)
                {
                    //case AppFormStatusType.SalesReview:
                    //    item.SalesDate = DateTime.UtcNow;
                    //    foundItem.UserSales = item.UpdatedUser;
                    //    if (string.IsNullOrEmpty(item.SalesComments))
                    //        throw new BusinessException("Sales comments is required");
                    //    break;

                    //case AppFormStatusType.SalesRejected:
                    //    item.SalesDate = DateTime.UtcNow;
                    //    foundItem.UserSales = item.UpdatedUser;
                    //    if (string.IsNullOrEmpty(item.SalesComments))
                    //        throw new BusinessException("Sales comments is required");
                    //    break;

                    case AppFormStatusType.ApplicantReview:
                        //if (foundItem.Status == AppFormStatusType.SalesReview)
                        //{
                        //    if (string.IsNullOrEmpty(item.SalesComments))
                        //        throw new BusinessException("Sales comments is required");
                        //    item.SalesDate = DateTime.UtcNow;
                        //    foundItem.UserSales = item.UpdatedUser;
                        //}
                        if (foundItem.Status == AppFormStatusType.New)
                        {   
                            item.SalesDate = DateTime.UtcNow;   // Guarda cuando se envió a revisión siendo nuevo
                            foundItem.UserSales = item.UpdatedUser;
                        }

                        if (foundItem.Status == AppFormStatusType.ApplicantRejected)
                        {
                            item.ReviewDate = DateTime.UtcNow;  // Guarda cuando se envió a revision despues de rechazado
                            foundItem.UserReviewer = item.UpdatedUser;
                        }
                        break;
                    
                    case AppFormStatusType.ApplicantRejected:
                        item.ReviewDate = DateTime.UtcNow;      // Guarda cuando se rechazó
                        foundItem.UserReviewer = item.UpdatedUser;
                        break;

                    case AppFormStatusType.Active:
                        item.ReviewDate = DateTime.UtcNow;      // Guarda cuando se aprobó
                        foundItem.UserReviewer = item.UpdatedUser;
                        break;

                    case AppFormStatusType.Inactive:
                        // Guardar todos los datos de contacts y sites en formato
                        // JSON solo si viene de estar activo
                        if (foundItem.Status == AppFormStatusType.Active)
                        {
                            foundItem.HistoricalDataJSON = GetHistoricalDataJSON(foundItem);
                        }
                        break;

                    case AppFormStatusType.Cancel:
                        // Guardar todos los datos de contacts y sites en formato
                        // JSON solo si viene de cualquier status que sea menor a inactivo
                        if (foundItem.Status <= AppFormStatusType.Active)
                        {
                            foundItem.HistoricalDataJSON = GetHistoricalDataJSON(foundItem);
                        }
                        break;

                } // switch
            } // Cambio de status

            // Asignar valores

            // Si es inactivo, cancelado solo guardar ciertos valores y no todo lo demas
            if (item.Status < AppFormStatusType.Inactive)
            {
                // ISO Varios
                foundItem.ActivitiesScope = item.ActivitiesScope;                       // 9K, 14K, 22K, HACCP
                foundItem.ProcessServicesCount = item.ProcessServicesCount;             // 9K, 14K, 22K, HACCP
                foundItem.ProcessServicesDescription = item.ProcessServicesDescription; // 9K, 14K, 22K, HACCP
                foundItem.LegalRequirements = item.LegalRequirements;                   // 9K, 14K, 22K, HACCP
                foundItem.AnyCriticalComplaint = item.AnyCriticalComplaint;             // 9K, 14K, 37K
                foundItem.CriticalComplaintComments = item.CriticalComplaintComments;   // 9K, 14K, 37K
                foundItem.AutomationLevelPercent = item.AutomationLevelPercent;
                foundItem.AutomationLevelJustification = item.AutomationLevelJustification;
                foundItem.ReviewJustification = item.ReviewJustification;
                // ISO 9K
                if (foundItem.Standard.StandardBase == StandardBaseType.ISO9K)
                {
                    foundItem.IsDesignResponsibility = item.IsDesignResponsibility;
                    foundItem.DesignResponsibilityJustify = item.DesignResponsibilityJustify;
                }
                // ISO 14K
                foundItem.OperationalControls = foundItem.Standard.StandardBase == StandardBaseType.ISO14K
                    ? item.OperationalControls
                    : null;
                // ISO 22K & HACCP
                if (foundItem.Standard.StandardBase == StandardBaseType.ISO22K 
                    || foundItem.Standard.StandardBase == StandardBaseType.HACCP)
                {
                    foundItem.Category22KID = item.Category22KID;
                    foundItem.HACCPCount = item.HACCPCount;
                    foundItem.SeasonalityJSON = item.SeasonalityJSON;
                }
                // - internal 22K
                // ISO 27K
                foundItem.AssetsISO27KJSON = foundItem.Standard.StandardBase == StandardBaseType.ISO27K
                    ? item.AssetsISO27KJSON
                    : null;
                // ISO 45K
                if (foundItem.Standard.StandardBase == StandardBaseType.ISO45K)
                {
                    foundItem.OHSHazardRisk45KJSON = item.OHSHazardRisk45KJSON;
                    foundItem.HazardousMaterials45KJSON = item.HazardousMaterials45KJSON;
                    foundItem.AccidentRate45KJSON = item.AccidentRate45KJSON;
                    foundItem.IndirectHSRisk45KJSON = item.IndirectHSRisk45KJSON;
                    foundItem.HighLevelRisks45K = item.HighLevelRisks45K;
                }

                // General
                foundItem.Description = item.Description;
                foundItem.AuditLanguage = item.AuditLanguage;
                foundItem.CycleYear = item.CycleYear;
                foundItem.CurrentCertificationsExpiration = item.CurrentCertificationsExpiration;
                foundItem.CurrentStandards = item.CurrentStandards;
                foundItem.CurrentCertificationsBy = item.CurrentCertificationsBy;
                foundItem.OutsourcedProcess = item.OutsourcedProcess;
                foundItem.AnyConsultancy = item.AnyConsultancy;
                foundItem.AnyConsultancyBy = item.AnyConsultancyBy;
                // Internal
                foundItem.SalesDate = item.SalesDate ?? foundItem.SalesDate;
                foundItem.ReviewDate = item.ReviewDate ?? foundItem.ReviewDate;
            }

            foundItem.Status = item.Status;
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

        public async Task<AppForm> DuplicateAsync(Guid id, string updatedUser)
        {
            var originalItem = await _repository.GetAsync(id)
                ?? throw new BusinessException("The record to duplicate was not found");

            // Validations 

            // - Validar que no haya otro appform activo del mismo standard y año de ciclo
            if (await _repository.ExistsValidAppFormAsync(originalItem.AuditCycleID))
            {
                throw new BusinessException("There is already an active application form for the same standard in the selected audit cycle");
            }

            var cycleYear = await _repository.GetNextCycleYearAsync(
                originalItem.AuditCycleID,
                originalItem.AuditCycle.Periodicity ?? AuditCyclePeriodicityType.Nothing
            );
            if (cycleYear == CycleYearType.Nothing)
                throw new BusinessException($"The certificate cycle has already completed all its years.");

            var newItem = new AppForm
            {
                ID = Guid.NewGuid(),
                OrganizationID = originalItem.OrganizationID,
                AuditCycleID = originalItem.AuditCycleID,
                StandardID = originalItem.StandardID,
                // ISO Varios
                ActivitiesScope = originalItem.ActivitiesScope,
                ProcessServicesCount = originalItem.ProcessServicesCount,
                ProcessServicesDescription = originalItem.ProcessServicesDescription,
                LegalRequirements = originalItem.LegalRequirements,
                AnyCriticalComplaint = originalItem.AnyCriticalComplaint,
                CriticalComplaintComments = originalItem.CriticalComplaintComments,
                // General
                AuditLanguage = originalItem.AuditLanguage,
                CycleYear = cycleYear,
                CurrentCertificationsExpiration = originalItem.CurrentCertificationsExpiration,
                CurrentStandards = originalItem.CurrentStandards,
                CurrentCertificationsBy = originalItem.CurrentCertificationsBy,
                AnyConsultancy = originalItem.AnyConsultancy,
                AnyConsultancyBy = originalItem.AnyConsultancyBy,
                // Internal
                Status = AppFormStatusType.New,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                UpdatedUser = updatedUser,
                NaceCodes = new List<NaceCode>(),
                Contacts = new List<Contact>(),
                Sites = new List<Site>(),
                RiskLevels = new List<RiskLevel>()
            };

            switch (originalItem.Standard.StandardBase)
            { 
                case StandardBaseType.ISO9K:
                    // ISO 9000
                    newItem.AutomationLevelPercent = originalItem.AutomationLevelPercent;
                    newItem.AutomationLevelJustification = originalItem.AutomationLevelJustification;
                    newItem.IsDesignResponsibility = originalItem.IsDesignResponsibility;
                    newItem.DesignResponsibilityJustify = originalItem.DesignResponsibilityJustify;
                    break;
                case StandardBaseType.ISO14K:
                    // ISO 14000
                    newItem.OperationalControls = originalItem.OperationalControls;
                    break;
                case StandardBaseType.ISO22K:
                case StandardBaseType.HACCP:
                    // ISO 22000 & HACCP
                    newItem.Category22KID = originalItem.Category22KID;
                    newItem.HACCPCount= originalItem.HACCPCount;
                    newItem.SeasonalityJSON = originalItem.SeasonalityJSON;
                    break;
                case StandardBaseType.ISO27K:
                    newItem.AssetsISO27KJSON = originalItem.AssetsISO27KJSON;
                    break;
                case StandardBaseType.ISO45K:
                    newItem.OHSHazardRisk45KJSON = originalItem.OHSHazardRisk45KJSON;
                    newItem.HazardousMaterials45KJSON = originalItem.HazardousMaterials45KJSON;
                    newItem.AccidentRate45KJSON = originalItem.AccidentRate45KJSON;
                    newItem.IndirectHSRisk45KJSON = originalItem.IndirectHSRisk45KJSON;
                    newItem.HighLevelRisks45K = originalItem.HighLevelRisks45K;
                    break;
            }

            // Agregar los NaceCodes, Contacts y Sites

            foreach (var nace in originalItem.NaceCodes
                .Where(nc => nc.Status == StatusType.Active))
            {
                await _repository.AddNaceCodeAsync(newItem, nace.ID);
            }

            // - Contacts

            foreach (var contact in originalItem.Contacts
                .Where(c => c.Status == StatusType.Active))
            {
                await _repository.AddContactAsync(newItem, contact.ID);
            }

            // - Risk Levels
            foreach (var riskLevel in originalItem.RiskLevels
                .Where(rl => rl.Status == StatusType.Active))
            {
                await _repository.AddRiskLevelAsync(newItem, riskLevel.ID);
            }

            // - Sites

            foreach (var site in originalItem.Sites
                .Where(s => s.Status == StatusType.Active))
            {
                await _repository.AddSiteAsync(newItem, site.ID);
            }

            try
            {
                _repository.Add(newItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AppFormService.DuplicateAsync: {ex.Message}");
            }

            return newItem;
        } // DuplicateAsync

        public async Task DeleteAsync(AppForm item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

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

        // RiskLevels

        public async Task AddRiskLevelAsync(Guid id, Guid riskLevelID)
        {
            // Validar
            // - Que el nivel de riesgo esté activo
            // - Que el nivel de riesgo sea del standard del app form
            await _repository.AddRiskLevelAsync(id, riskLevelID);
            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AppFormService.AddRiskLevelAsync: {ex.Message}");
            }
        } // AddRiskLevelAsync

        public async Task DelRiskLevelAsync(Guid id, Guid riskLevelID)
        {
            await _repository.DelRiskLevelAsync(id, riskLevelID);
            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AppFormService.DelRiskLevelAsync: {ex.Message}");
            }
        } // DelRiskLevelAsync

        // Sites

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

        // PRIVATE 

        private string GetHistoricalDataJSON(AppForm item)
        {
            var historicalData = new
            {
                OrganizationName = item.Organization?.Name,
                AuditCycleName = item.AuditCycle?.Name,
                StandardName = item.Standard?.Name,
                item.Organization?.Website,
                item.Organization?.Phone,
                Category22K = item.Category22KID.HasValue ? new { 
                    item.Category22K.Cluster,
                    item.Category22K.Category,
                    item.Category22K.CategoryDescription,
                    item.Category22K.SubCategory,
                    item.Category22K.SubCategoryDescription,
                    item.Category22K.Examples,
                    item.Category22K.AccreditedStatus
                } : null,
                Companies = item.Organization.Companies
                    .Where(c => c.Status == StatusType.Active)
                    .Select(c => new { c.ID, c.Name, c.LegalEntity, c.COID }),
                Contacts = item.Contacts
                    .Where(c => c.Status == StatusType.Active)
                    .Select(c => new 
                    { 
                        c.ID, 
                        FullName = Strings.FullName(c.FirstName, c.MiddleName, c.LastName), 
                        c.Email, 
                        c.Phone, 
                        c.Position 
                    }),
                Sites = item.Sites
                    .Where(s => s.Status == StatusType.Active)
                    .Select(s => new {
                        s.ID,
                        s.Description,
                        s.IsMainSite,
                        s.Address,
                        s.Country,
                        Shifts = s.Shifts
                            .Where(sh => sh.Status == StatusType.Active)
                            .Select(sh => new
                            {
                                sh.ID,
                                sh.Type,
                                sh.NoEmployees,
                                sh.ActivitiesDescription,
                                sh.ShiftStart,
                                sh.ShiftEnd,
                                sh.ShiftStart2,
                                sh.ShiftEnd2,
                            }),
                        EmployeesCount = s.Shifts
                            .Where(sh => sh.Status == StatusType.Active)
                            .Sum(sh => sh.NoEmployees)
                    }),
                SitesEmployeesCount = item.Sites != null
                    ? item.Sites
                        .Where(s => s.Status == StatusType.Active)
                        .Sum(s => s.Shifts
                            .Where(sh => sh.Status == StatusType.Active)
                            .Sum(sh => sh.NoEmployees)) ?? 0
                    : 0,
                NaceCodes = item.NaceCodes
                    .Where(nc => nc.Status == StatusType.Active)
                    .Select(nc => new 
                        { 
                            nc.ID, 
                            nc.Sector,
                            nc.Division,
                            nc.Group,
                            nc.Class,
                            nc.Description,
                            nc.AccreditedStatus
                        }
                    ),
                RiskLevels = item.RiskLevels
                    .Where(rl => rl.Status == StatusType.Active)
                    .Select(rl => new
                    {
                        rl.ID,
                        rl.Category,
                        rl.BusinessSector
                    })
            };

            return JsonConvert.SerializeObject(historicalData);
        } // GetHistoricalDataJSON

        // CREATE

        /// <summary>
        /// Valida los datos recibidos para crear un AppForm, con los datos minimos necesarios
        /// requeridos y que las asociaciones iniciales a otros objetos sean validas
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="auditCycle"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        private async Task ValidateCreateAppFormAsync(AppForm newItem, AuditCycle auditCycle)
        {
            var organizationRepository = new OrganizationRepository();
            //var auditCycleRepository = new AuditCycleRepository();

            // - Validar que sea un Standard valido para generar un AppForm, por el momento solo:
            //   [ ISO 9001, ISO 14001, ISO 22000, ISO 27001, ISO 37001, ISO 45001, HACCP ]
            //   se pueden agregar más en el futuro

            if (auditCycle.Standard.StandardBase != StandardBaseType.ISO9K
                && auditCycle.Standard.StandardBase != StandardBaseType.ISO14K
                && auditCycle.Standard.StandardBase != StandardBaseType.ISO22K
                && auditCycle.Standard.StandardBase != StandardBaseType.ISO27K
                && auditCycle.Standard.StandardBase != StandardBaseType.ISO37K
                && auditCycle.Standard.StandardBase != StandardBaseType.ISO45K
                && auditCycle.Standard.StandardBase != StandardBaseType.HACCP)
                throw new BusinessException("The selected standard is not valid for generating an Application Form");

            // - Validar que la organizacion exista y esté activo
            var organization = await organizationRepository.GetAsync(newItem.OrganizationID)
                ?? throw new BusinessException("The selected organization was not found");

            if (organization.Status != OrganizationStatusType.Active 
                && organization.Status != OrganizationStatusType.Applicant)
                throw new BusinessException("The selected organization is not active");

            // - Validar que el ciclo exista y esté activo o inactivo
            if (auditCycle.Status != StatusType.Active 
                && auditCycle.Status != StatusType.Inactive)
                throw new BusinessException("The selected audit cycle is not valid");

            // - Validar que el ciclo no sea del pasado
            if (auditCycle.EndDate < DateTime.Today)
                throw new BusinessException("Can't create Application Forms for an Audit Cycle that is outdated.");

            if (await _repository.ExistsValidAppFormAsync(auditCycle.ID))
                throw new BusinessException("There is already an active Application Form for this standard cycle");

            // - Validar que exista al menos un Site en la Organización y sea el sitio principal
            if (!organization.Sites.Any(s => s.Status == StatusType.Active))
                throw new BusinessException("The organization must have at least one active site");

            if (!organization.Sites.Any(s => s.Status == StatusType.Active && s.IsMainSite))
                throw new BusinessException("The organization must have an active main site");

            // - Validar que el Standard asociado al AuditCycle esté activo tanto en la
            //   organización como en el sistema

            if (auditCycle.StandardID.HasValue && auditCycle.StandardID.Value != Guid.Empty)
            { 
                var standardRepository = new StandardRepository();
                var standardItem = standardRepository.Gets()
                    .Where(s => s.ID == auditCycle.StandardID.Value).FirstOrDefault()
                    ?? throw new BusinessException("The standard was not found");

                if (standardItem.Status != StatusType.Active)
                    throw new BusinessException("The standard is not active");

                if (!organization.OrganizationStandards
                    .Any(os => os.StandardID == auditCycle.StandardID.Value && os.Status == StatusType.Active))
                    throw new BusinessException("The organization does not have the standard assigned or is not active");
            }
            else
            {
                throw new BusinessException("The audit cycle does not have a standard assigned");
            }

            // Considerar que solo la primera vez se registra el standard, despues si
            // ya se validó, sin importar el status del standard, se queda
            if (newItem.Status == AppFormStatusType.Nothing) // Si es nuevo...
            {
                if (await _repository.GetNextCycleYearAsync(
                    newItem.AuditCycleID,
                    auditCycle.Periodicity ?? AuditCyclePeriodicityType.Nothing
                    ) == CycleYearType.Nothing
                )
                    throw new BusinessException($"The certificate cycle has already completed all its years");
            }

        } // ValidateCreateAppFormAsync

        /// <summary>
        /// Busca y agrega el sitio principal de la organización al appform creado, 
        /// esto para asegurar que siempre vaya el sitio principal.
        /// </summary>
        /// <param name="appForm"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        private async Task<AppForm> AddMainSiteAsync(AppForm appForm)
        {
            var organizationRepository = new OrganizationRepository();
            var organization = await organizationRepository.GetAsync(appForm.OrganizationID)
                ?? throw new BusinessException("AddMainSiteAsync: The organization was not found");
            var mainSite = organization.Sites
                .Where(s => s.Status == StatusType.Active && s.IsMainSite)
                .FirstOrDefault();

            if (mainSite != null)
            {
                await _repository.AddSiteAsync(appForm.ID, mainSite.ID);
                await _repository.SaveChangesAsync();
                appForm = await _repository.GetAsync(appForm.ID, true); // Obtener el item con las relaciones cargadas
            }

            return appForm;
        } // AddMainSiteAsync

        // UPDATE

        private async Task ValidateAppFormAsync(AppForm newItem, AppForm currentItem)
        {
            // - Validar que el CycleYear exista
            // - Solo puede haber un appform activo por ciclo
            // - Validar que el ciclo esté activo - Omitir por ahora -UPDATE xBlaze(20250826): este no, es necesario subir auditorias o documentación estando inactivo
            //   por lo pronto, validar que el ciclo no sea del pasado
            // - Validar que el standard esté activo y que pertenezca al ciclo,
            //   solo la primera vez - Este se cambió a ValidateCreateAppFormAsync
            // - Validar que el appform no esté en un status que no se pueda editar
            // - AuditLanguage - Validar que sea un idioma aceptado 'es', 'en' - YA
            // - Al cambiar a Activo, validar de acuerdo con su Standard Base:
            //   - Tener al menos un sitio asignado y que sea el principal
            //   - Tener al menos un contacto asignado
            //   - 9K y 14K: Tener al menos un NACE code asignado
            //   - 14K: Tener al menos un Risk Level asignado
            //   - 22K y HACCP: Validar que tenga una categoría asignada y que si tiene HACCP
            //   - 37K: Sin campos extra, validacion como 14K

            // TODO: Considerar el validar por fechas de aplicación, que no este el año 2 un año fisico antes que año 1, etc.

            var standardRepository = new StandardRepository();

            if (currentItem.Status == AppFormStatusType.Inactive
                || currentItem.Status == AppFormStatusType.Deleted)
                throw new BusinessException("The record is not editable");

            // Validar si el CycleYear es valido y no está duplicado
            if (await _repository.ExistsValidCycleYearAppForm(
                    currentItem.AuditCycleID,
                    newItem.CycleYear ?? CycleYearType.Nothing,
                    newItem.ID))
                throw new BusinessException("The selected Cycle Year is already assigned to another Application Form in the current certificate cycle");

            if (newItem.Status != currentItem.Status) // El status cambió
            {
                // - De que status a que status no se puede ir
                if ((currentItem.Status == AppFormStatusType.Nothing
                        || currentItem.Status == AppFormStatusType.New
                        || currentItem.Status == AppFormStatusType.SalesReview
                        || currentItem.Status == AppFormStatusType.SalesRejected)
                    && newItem.Status != AppFormStatusType.New
                    && newItem.Status != AppFormStatusType.ApplicantReview
                    && newItem.Status != AppFormStatusType.Cancel)
                    throw new BusinessException("You can't change to this status from New");

                if (currentItem.Status == AppFormStatusType.ApplicantReview
                    && newItem.Status != AppFormStatusType.ApplicantRejected
                    && newItem.Status != AppFormStatusType.Active
                    && newItem.Status != AppFormStatusType.Cancel)
                    throw new BusinessException("You can't change to this status from Review");

                if (currentItem.Status == AppFormStatusType.ApplicantRejected
                    && newItem.Status != AppFormStatusType.ApplicantReview
                    && newItem.Status != AppFormStatusType.Cancel)
                    throw new BusinessException("You can't change to this status from Review rejected");

                if (currentItem.Status == AppFormStatusType.Active
                    && newItem.Status != AppFormStatusType.Inactive
                    && newItem.Status != AppFormStatusType.Cancel)
                    throw new BusinessException("You can't change to this status from Active");

                //if (currentItem.Status == AppFormStatusType.Inactive
                //    && newItem.Status != AppFormStatusType.Active
                //    && newItem.Status != AppFormStatusType.Cancel)
                //    throw new BusinessException("You can't change to this status from Inactive");

                if (currentItem.Status == AppFormStatusType.Cancel
                    && newItem.Status != AppFormStatusType.New)
                    throw new BusinessException("You can't change to this status from Cancel");

                if (await _repository.ExistsValidAppFormAsync(newItem.AuditCycleID, newItem.ID))
                    throw new BusinessException("There is already an active Application Form for this standard cycle");

                // NOTE: Deshabilitado por el momento porque se necesita modificar información
                // aunque el ciclo esté inactivo, pero se puede considerar que no se
                // debe de subir más información - xBlaze: 20260220
                //if (currentItem.AuditCycle != null 
                //    && currentItem.AuditCycle.EndDate.HasValue
                //    && currentItem.AuditCycle.EndDate.Value < DateTime.Now)
                //    throw new BusinessException("Audit cycle is old, Application Forms cannot be generated or updated for a certificate that has expired");

            } // El status cambió

            // ISO Varios
            await ValidateAppFormForGeneralAsync(newItem, currentItem);

            // Validar por ISO Base...
            switch (currentItem.Standard.StandardBase)
            {
                case StandardBaseType.ISO9K:
                    await ValidateAppFormFor9KAsync(newItem, currentItem);
                    break;

                case StandardBaseType.ISO14K:
                    await ValidateAppFormFor14KAsync(newItem, currentItem);
                    break;

                case StandardBaseType.ISO22K:
                case StandardBaseType.HACCP:
                    await ValidateAppFormFor22KAsync(newItem, currentItem);
                    break;

                case StandardBaseType.ISO37K:
                    await ValidateAppFormFor37KAsync(newItem, currentItem);
                    break;
            }

            // General...

            if (!string.IsNullOrEmpty(newItem.AuditLanguage))
            {   
                var languages = LanguagesList.GetLanguages();
                string languagesCodes = string.Join(", ", languages.Select(l => l.Code));

                if (!languages.Where(l => l.Code.Equals(newItem.AuditLanguage.ToLower())).Any())
                    throw new BusinessException("The audit language is not valid, must be: " + languagesCodes);
            } else throw new BusinessException("The audit language is required");

        } // ValidateAppFormAsync

        // ISO General

        private async Task ValidateAppFormForGeneralAsync(AppForm newItem, AppForm currentItem)
        {
            var item = newItem.Status == currentItem.Status // El status no ha cambiado
                ? currentItem
                : newItem;

            if (item.Status >= AppFormStatusType.New
                && item.Status <= AppFormStatusType.Active)
            {
                // - Validar que tenga al menos un sitio activo y que sea el principal
                if (!currentItem.Sites.Where(s => s.Status == StatusType.Active && s.IsMainSite).Any())
                    throw new BusinessException("The Application Form must have an active main site assigned");

                // - Validar que tenga al menos un contacto asignado
                if (!currentItem.Contacts.Where(c => c.Status == StatusType.Active).Any())
                    throw new BusinessException("The Application Form must have at least one active contact assigned");
            }
        } // ValidateAppFormForGeneralAsync

        // ISO 9K
        private async Task ValidateAppFormFor9KAsync(AppForm newItem, AppForm currentItem)
        {
            var item = newItem.Status == currentItem.Status // El status no ha cambiado
                ? currentItem
                : newItem;

            if (item.Status >= AppFormStatusType.ApplicantReview
                && item.Status <= AppFormStatusType.Active)
            {
                // - Validar que tenga al menos un nace code activo
                if (!currentItem.NaceCodes.Where(nc => nc.Status == StatusType.Active).Any())
                    throw new BusinessException("The Application Form must have at least one active NACE code assigned");
            }
        } // ValidateAppFormFor9KAsync

        // ISO 14K
        private async Task ValidateAppFormFor14KAsync(AppForm newItem, AppForm currentItem)
        {
            var item = newItem.Status == currentItem.Status // El status no ha cambiado
                ? currentItem
                : newItem;

            // Si está dentro de estos status, validar...
            if (item.Status >= AppFormStatusType.ApplicantReview
                && item.Status <= AppFormStatusType.Active)
            {
                // - Validar que tenga al menos un nace code activo
                if (!currentItem.NaceCodes.Where(nc => nc.Status == StatusType.Active).Any())
                    throw new BusinessException("The Application Form must have at least one active NACE code assigned");

                // - Validar que tenga un nivel de riesgo activo
                if (!currentItem.RiskLevels.Where(rl => rl.Status == StatusType.Active).Any())
                    throw new BusinessException("The Application Form must have at least one active Risk Level assigned");
            }

        } // ValidateAppFormFor14KAsync

        // ISO 22K & HACCP
        private async Task ValidateAppFormFor22KAsync(AppForm newItem, AppForm currentItem)
        {
            var item = newItem.Status == currentItem.Status // El status no ha cambiado
                ? currentItem
                : newItem;

            if (item.Status >= AppFormStatusType.ApplicantReview
                && item.Status <= AppFormStatusType.Active)
            {
                if (item.Category22KID == null || item.Category22KID == Guid.Empty)
                    throw new BusinessException("The Application Form must have a category assigned");

                if (item.HACCPCount == null || item.HACCPCount <= 0)
                    throw new BusinessException("The Application Form must have a valid HACCP number assigned");

                if ((item.Status == AppFormStatusType.ApplicantRejected || item.Status == AppFormStatusType.Active)
                    && string.IsNullOrEmpty(item.ReviewJustification))
                    throw new BusinessException("Review justification are required");
            }
        } // ValidateAppFormFor22KAsync

        private async Task ValidateAppFormFor37KAsync(AppForm newItem, AppForm currentItem)
        {
            var item = newItem.Status == currentItem.Status // El status no ha cambiado
                ? currentItem
                : newItem;

            // Si está dentro de estos status, validar...
            if (item.Status >= AppFormStatusType.ApplicantReview
                && item.Status <= AppFormStatusType.Active)
            {
                // - Validar que tenga al menos un nace code activo
                if (!currentItem.NaceCodes.Where(nc => nc.Status == StatusType.Active).Any())
                    throw new BusinessException("The Application Form must have at least one active NACE code assigned");

                // - Validar que tenga un nivel de riesgo activo
                if (!currentItem.RiskLevels.Where(rl => rl.Status == StatusType.Active).Any())
                    throw new BusinessException("The Application Form must have at least one active Risk Level assigned");
            }
        } // ValidateAppFormFor37KAsync

        // STATIC METHODS

        public static async Task<List<AppFormAlertType>> GetAlertsAsync(AppForm item)
        {
            var alerts = new List<AppFormAlertType>();

            // - Que al menos haya un sitio activo, puede que hayan actualizado sitios
            if (item.Sites == null || !item.Sites.Any(site => site.Status == StatusType.Active))
                alerts.Add(AppFormAlertType.NoActiveSites);

            // - Que tenga al menos un sitio activo y que uno de ellos sea el sitio principal
            if (item.Sites == null || !item.Sites.Any(s => s.Status == StatusType.Active && s.IsMainSite))
                alerts.Add(AppFormAlertType.MainSiteMissing);

            return alerts;
        } // GetAlertsAsync
    }
}