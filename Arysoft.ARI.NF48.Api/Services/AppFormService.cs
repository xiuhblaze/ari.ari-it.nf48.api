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
        public async Task<AppForm> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        /// <summary>
        /// Genera un registro con la información minima necesaria y en base a ella
        /// Obtiene el siguiente ciclo disponible para el standard seleccionado
        /// </summary>
        /// <param name="item">Elemento con los datos minimos para crear un registro</param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<AppForm> AddAsync(AppForm item)
        {
            await ValidateCreateAppFormAsync(item);

            // Set values

            item.ID = Guid.NewGuid();
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

            await ValidateAppFormAsync(item, foundItem);

            // - Asignaciones por status

            if (foundItem.Status == AppFormStatusType.Nothing) // Si es la primera vez...
            {
                foundItem.StandardID = item.StandardID; // Se asigna el standard
                // Omitido porque la asignación del CycleYear se hace en el front al crear el appform - xBlaze 20251203
                //foundItem.CycleYear = await _repository.GetNextCycleYearAwait( 
                //    foundItem.AuditCycleID, 
                //    item.StandardID ?? Guid.Empty,
                //    foundItem.AuditCycle.Periodicity ?? AuditCyclePeriodicityType.Nothing
                //);
            }

            // Validar si el CycleYear es valido y no está duplicado
            if (await _repository.ExistsValidCycleYearAppForm(
                foundItem.AuditCycleID, 
                foundItem.StandardID ?? Guid.Empty, 
                item.CycleYear ?? CycleYearType.Nothing,
                item.ID
                ))
                throw new BusinessException("The selected Cycle Year is already assigned to another Application Form for the same standard in the current audit cycle");

            // TODO: Considerar el validar por fechas de aplicación, que no este el año 2 un año fisico antes que año 1, etc.

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
                            //if (string.IsNullOrEmpty(item.SalesComments))
                            //    throw new BusinessException("Comments is required");
                            item.SalesDate = DateTime.UtcNow;
                            foundItem.UserSales = item.UpdatedUser;
                        }

                        if (foundItem.Status == AppFormStatusType.ApplicantRejected)
                        {
                            //if (string.IsNullOrEmpty(item.ReviewComments))
                            //    throw new BusinessException("Review comments is required");
                            item.ReviewDate = DateTime.UtcNow;
                            foundItem.UserReviewer = item.UpdatedUser;
                        }
                        break;
                    
                    case AppFormStatusType.ApplicantRejected:
                        item.ReviewDate = DateTime.UtcNow;
                        foundItem.UserReviewer = item.UpdatedUser;
                        //if (string.IsNullOrEmpty(item.ReviewComments))
                        //    throw new BusinessException("Review comments is required");
                        break;

                    case AppFormStatusType.Active:
                        item.ReviewDate = DateTime.UtcNow;
                        foundItem.UserReviewer = item.UpdatedUser;
                        //if (string.IsNullOrEmpty(item.ReviewComments))
                        //    throw new BusinessException("Review comments is required");
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
                // ISO 9K
                foundItem.ActivitiesScope = item.ActivitiesScope;
                foundItem.ProcessServicesCount = item.ProcessServicesCount;
                foundItem.ProcessServicesDescription = item.ProcessServicesDescription;
                foundItem.LegalRequirements = item.LegalRequirements;
                foundItem.AnyCriticalComplaint = item.AnyCriticalComplaint;
                foundItem.CriticalComplaintComments = item.CriticalComplaintComments;
                foundItem.AutomationLevelPercent = item.AutomationLevelPercent;
                foundItem.AutomationLevelJustification = item.AutomationLevelJustification;
                foundItem.IsDesignResponsibility = item.IsDesignResponsibility;
                foundItem.DesignResponsibilityJustify = item.DesignResponsibilityJustify;

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
                //foundItem.SalesComments = item.SalesComments;
                foundItem.ReviewDate = item.ReviewDate ?? foundItem.ReviewDate;
                foundItem.ReviewJustification = item.ReviewJustification;
                //foundItem.ReviewComments = item.ReviewComments;                
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

            // - Validar que no haya otro appform activo del mismo standard y ciclo
            if (await _repository.ExistsValidAppFormAsync(originalItem.AuditCycleID, originalItem.StandardID.Value))
            {
                throw new BusinessException("There is already an active application form for the same standard in the selected audit cycle");
            }

            var cycleYear = await _repository.GetNextCycleYearAsync(
                originalItem.AuditCycleID,
                originalItem.StandardID ?? Guid.Empty,
                originalItem.AuditCycle.Periodicity ?? AuditCyclePeriodicityType.Nothing
            );
            if (cycleYear == CycleYearType.Nothing)
                throw new BusinessException($"The audit cycle has already completed its three years for the {originalItem.Standard.Name} standard");

            var newItem = new AppForm
            {
                ID = Guid.NewGuid(),
                OrganizationID = originalItem.OrganizationID,
                AuditCycleID = originalItem.AuditCycleID,
                StandardID = originalItem.StandardID,
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
                Sites = new List<Site>()
            };

            switch (originalItem.Standard.StandardBase)
            { 
                case StandardBaseType.ISO9k:
                    // ISO 9000
                    newItem.ActivitiesScope = originalItem.ActivitiesScope;
                    newItem.ProcessServicesCount = originalItem.ProcessServicesCount;
                    newItem.ProcessServicesDescription = originalItem.ProcessServicesDescription;
                    newItem.LegalRequirements = originalItem.LegalRequirements;
                    newItem.AnyCriticalComplaint = originalItem.AnyCriticalComplaint;
                    newItem.CriticalComplaintComments = originalItem.CriticalComplaintComments;
                    newItem.AutomationLevelPercent = originalItem.AutomationLevelPercent;
                    newItem.AutomationLevelJustification = originalItem.AutomationLevelJustification;
                    newItem.IsDesignResponsibility = originalItem.IsDesignResponsibility;
                    newItem.DesignResponsibilityJustify = originalItem.DesignResponsibilityJustify;
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
                throw new BusinessException($"AppFormService.DuplicateAsync.AddNaceCode: {ex.Message}");
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
                                    )
            };

            return JsonConvert.SerializeObject(historicalData);
        } // GetHistoricalDataJSON

        private async Task ValidateCreateAppFormAsync(AppForm newItem)
        {
            var organizationRepository = new OrganizationRepository();
            var auditCycleRepository = new AuditCycleRepository();

            // - Validar que la organizacion exista y esté activo
            var organization = await organizationRepository.GetAsync(newItem.OrganizationID)
                ?? throw new BusinessException("The selected organization was not found");

            if (organization.Status != OrganizationStatusType.Active 
                && organization.Status != OrganizationStatusType.Applicant)
                throw new BusinessException("The selected organization is not active");

            // - Validar que el ciclo exista y esté activo o inactivo
            var auditCycle = await auditCycleRepository.GetAsync(newItem.AuditCycleID)
                ?? throw new BusinessException("The selected audit cycle was not found");

            if (auditCycle.Status != StatusType.Active 
                && auditCycle.Status != StatusType.Inactive)
                throw new BusinessException("The selected audit cycle is not valid");

            // Solo si el AuditCycle tiene un solo standard activo, lo valida para 
            // ver si aun tiene CycleYear's disponibles
            var auditCycleStandards = auditCycle.AuditCycleStandards
                .Where(acs => acs.Status == StatusType.Active)
                .ToList();
            if (auditCycleStandards.Count() == 1)
            { 
                var auditCycleStandard = auditCycleStandards.First();
                var nextCycleYear = await _repository
                    .GetNextCycleYearAsync(
                        newItem.AuditCycleID, 
                        auditCycleStandard.StandardID ?? Guid.Empty, 
                        auditCycle.Periodicity ?? AuditCyclePeriodicityType.Nothing
                    );

                if (nextCycleYear == CycleYearType.Nothing)
                    throw new BusinessException($"The audit cycle has already completed its three years for the { auditCycleStandard.Standard.Name } standard");
            }

            // TODO: Validar para todos los auditCycleStandards activos que tenga al menos
            // uno de los standards, un AppForm sin que esté activo para permitir crear uno nuevo

        } // ValidateCreateAppFormAsync

        private async Task ValidateAppFormAsync(AppForm newItem, AppForm currentItem)
        {
            // - Validarque el CycleYear no exista
            // - Solo puede haber un appform activo por ciclo y standard
            // - Validar que el ciclo esté activo - Omitir por ahora -UPDATE xBlaze(20250826): este no, es necesario subir auditorias o documentación estando inactivo
            //   por lo pronto, validar que el ciclo no sea del pasado
            // - Validar que el standard esté activo y que pertenesca al ciclo,
            //   solo la primera vez - YA
            // - Validar que el appform no esté en un status que no se pueda editar
            // - AuditLanguage - Validar que sea un idioma aceptado 'es', 'en' - YA

            var standardRepository = new StandardRepository();

            if (currentItem.Status == AppFormStatusType.Inactive
                || currentItem.Status == AppFormStatusType.Deleted)
                throw new BusinessException("The record is not editable");

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
            } // El status cambió

            if (await _repository
                .ExistsValidAppFormAsync(
                    newItem.AuditCycleID, 
                    newItem.StandardID ?? Guid.Empty,
                    newItem.ID
                ))
                throw new BusinessException("There is already an active Application Form for this cycle and standard");

            if (currentItem.AuditCycle != null 
                && currentItem.AuditCycle.EndDate != null 
                && currentItem.AuditCycle.EndDate < DateTime.Now)
                throw new BusinessException("The audits cycle is old");

            // Considerar que solo la primera vez se registra el standard, despues si
            // ya se validó, sin importar el status del standard, se queda
            if (currentItem.Status == AppFormStatusType.Nothing) // Si es nuevo...
            { 
                if (newItem.StandardID != null && newItem.StandardID != Guid.Empty)
                {
                    var standardItem = standardRepository.Gets()
                        .Where(s => s.ID == newItem.StandardID).FirstOrDefault()
                        ?? throw new BusinessException("The selected standard was not found");

                    if (standardItem.Status != StatusType.Active)
                        throw new BusinessException("The selected standard is not active");

                    if (await _repository.ExistsValidAppFormAsync(currentItem.AuditCycleID, newItem.StandardID.Value))
                        throw new BusinessException("There is already an active application form for the same standard in the current audit cycle");
                    
                    if (!currentItem.AuditCycle.AuditCycleStandards.Where(acs => acs.StandardID == newItem.StandardID).Any())
                        throw new BusinessException("The selected standard is not associated with the current audit cycle");

                    if (await _repository
                        .GetNextCycleYearAsync(
                            currentItem.AuditCycleID, 
                            newItem.StandardID.Value,
                            currentItem.AuditCycle.Periodicity ?? AuditCyclePeriodicityType.Nothing
                        ) == CycleYearType.Nothing)
                        throw new BusinessException($"The audit cycle has already completed its three years for the { standardItem.Name } standard");
                }
                else // Probablemente aquí no entre nunca
                {
                    throw new BusinessException("The standard is required");
                }
            }

            if (!string.IsNullOrEmpty(newItem.AuditLanguage))
            {   
                var languages = LanguagesList.GetLanguages();
                string languagesCodes = string.Join(", ", languages.Select(l => l.Code));

                if (!languages.Where(l => l.Code.Equals(newItem.AuditLanguage.ToLower())).Any())
                    throw new BusinessException("The audit language is not valid, must be: " + languagesCodes);
            } else throw new BusinessException("The audit language is required");

        } // ValidateAppFormAsync
    }
}