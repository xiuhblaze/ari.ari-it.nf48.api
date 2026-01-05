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

namespace Arysoft.ARI.NF48.Api.Services
{
    public class AuditCycleService
    {
        private readonly AuditCycleRepository _repository;

        // CONSTRUCTOR

        public AuditCycleService()
        {
            _repository = new AuditCycleRepository();
        }

        // METHODS

        public PagedList<AuditCycle> Gets(AuditCycleQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty)
            {
                items = items.Where(e => e.OrganizationID == filters.OrganizationID);
            }

            if (filters.CycleType != null && filters.CycleType != AuditCycleType.Nothing)
            {
                items = items.Where(e => e.CycleType == filters.CycleType);
            }

            if (filters.StartDate != null)
            {
                items = items.Where(e => e.StartDate >= filters.StartDate);
            }

            if (filters.EndDate != null)
            {
                items = items.Where(e => e.EndDate <= filters.EndDate);
            }

            if (filters.Status != null && filters.Status != StatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != StatusType.Nothing)
                    : items.Where(e => e.Status != StatusType.Nothing && e.Status != StatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            {
                case AuditCycleOrderType.Date:
                    items = items.OrderBy(e => e.StartDate);
                    break;
                case AuditCycleOrderType.DateDesc:
                    items = items.OrderByDescending(e => e.StartDate);
                    break;
                case AuditCycleOrderType.Status:
                    items = items.OrderBy(e => e.Status);
                    break;
                case AuditCycleOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status);
                    break;
                default:
                    items = items.OrderByDescending(e => e.StartDate);
                    break;
            }

            // Paging

            var pagedItems = PagedList<AuditCycle>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<AuditCycle> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<AuditCycle> AddAsync(AuditCycle item)
        { 
            var _organizationRepository = new OrganizationRepository();

            // Validations

            if (item.OrganizationID == Guid.Empty)
                throw new BusinessException("Organization is required");

            // - Validar que la organizacion exista
            // - Validar que la organización tenga un status de activo
            // - TODO: Analizar que otro requisito necesita la organización para
            //         poder crear un ciclo

            var organizationFound = await _organizationRepository.GetAsync(item.OrganizationID)
                ?? throw new BusinessException("The organization does not exist");

            if (organizationFound.Status != OrganizationStatusType.Applicant &&
                organizationFound.Status != OrganizationStatusType.Active)
                throw new BusinessException("The organization is not active");

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries
            try
            {
                await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
                _repository.Add(item);
                await _repository.SaveChangesAsync(); // Async para esperar aquí a ver si sucede un error
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditCycle.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<AuditCycle> UpdateAsync(AuditCycle item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            await ValidateUpdatedItemAsync(item, foundItem);

            // Assigning values

            if (foundItem.Status == StatusType.Nothing) // Solo si es nuevo...
            { 
                foundItem.StandardID = item.StandardID; // ...asignar el standard ID
            }

            foundItem.Name = item.Name;
            foundItem.CycleType = item.CycleType;
            foundItem.InitialStep = item.InitialStep;
            foundItem.StartDate = item.StartDate;
            foundItem.EndDate = item.EndDate;
            foundItem.Periodicity = item.Periodicity;
            foundItem.ExtraInfo = item.ExtraInfo;
            foundItem.Status = foundItem.Status == StatusType.Nothing && item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status != StatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            // Execute queries

            try
            {   
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync(); // Async para esperar aquí a ver si sucede un error
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditCycle.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(AuditCycle item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations


            // Excecute queries

            if (foundItem.Status == StatusType.Deleted)
            {
                // - Validar que no existan auditorías asociadas al ciclo
                _repository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status == StatusType.Active
                    ? StatusType.Inactive
                    : StatusType.Deleted;
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
                throw new BusinessException($"AuditCycle.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync 

        public async Task CreateMissingAuditCyclesAsync()
        {
            var _auditStandardsRepository = new AuditStandardRepository();
            var _auditCycleStandardRepository = new AuditCycleStandardRepository();

            var auditStandards = _auditStandardsRepository.Gets()
                .Where(e => e.AuditCycleID == null)
                .ToList();

            var errorList = new List<string>();
            var newCycles = new List<AuditCycle>();
            var standardsToUpdate = new List<AuditStandard>();

            foreach (var auditStandard in auditStandards)
            {
                // Validaciones básicas de existencia
                if (auditStandard.Audit == null || auditStandard.Audit.OrganizationID == null)
                {
                    errorList.Add($"AuditStandard ID {auditStandard.ID} skipped: Audit or OrganizationID is null");
                    continue;
                }

                if (auditStandard.Audit.AuditCycle == null || auditStandard.Audit.AuditCycleID == null)
                {
                    errorList.Add($"AuditStandard ID {auditStandard.ID} skipped: AuditCycle or AuditCycleID is null");
                    continue;
                }

                var auditCycleStandard = _auditCycleStandardRepository.Gets()
                    .FirstOrDefault(e =>
                        e.StandardID == auditStandard.StandardID
                        && e.AuditCycleID == auditStandard.Audit.AuditCycleID);

                if (auditCycleStandard == null)
                {
                    errorList.Add($"AuditStandard ID {auditStandard.ID} skipped: No matching AuditCycleStandard found");
                    continue;
                }

                var existingCycle = _repository.Gets()
                    .Where(e =>
                        e.OrganizationID == (auditStandard.Audit.OrganizationID ?? Guid.Empty)
                        && e.StandardID == auditStandard.StandardID
                        && e.StartDate == auditStandard.Audit.AuditCycle.StartDate
                        && e.EndDate == auditStandard.Audit.AuditCycle.EndDate)
                    .ToList();

                var preCreatedCycle = newCycles
                    .Where(e =>
                        e.OrganizationID == (auditStandard.Audit.OrganizationID ?? Guid.Empty)
                        && e.StandardID == auditStandard.StandardID
                        && e.StartDate == auditStandard.Audit.AuditCycle.StartDate
                        && e.EndDate == auditStandard.Audit.AuditCycle.EndDate)
                    .ToList();

                if (existingCycle.Count > 0)
                {
                    auditStandard.AuditCycleID = existingCycle.First().ID;
                }
                else if (preCreatedCycle.Count > 0)
                {
                    auditStandard.AuditCycleID = preCreatedCycle.First().ID;
                }
                else
                {
                    // Crear nuevo ciclo de auditoría
                    var newAuditCycle = new AuditCycle
                        {
                            ID = Guid.NewGuid(),
                            OrganizationID = auditStandard.Audit.OrganizationID ?? Guid.Empty,
                            StandardID = auditStandard.StandardID,
                            Name = auditStandard.Audit.AuditCycle.Name ?? string.Empty,
                            CycleType = auditCycleStandard.CycleType,
                            InitialStep = auditCycleStandard.InitialStep,
                            StartDate = auditStandard.Audit.AuditCycle.StartDate,
                            EndDate = auditStandard.Audit.AuditCycle.EndDate,
                            Periodicity = auditStandard.Audit.AuditCycle.Periodicity,
                            ExtraInfo = auditStandard.Audit.AuditCycle.ExtraInfo,
                            Status = StatusType.Active,
                            Created = DateTime.UtcNow,
                            Updated = DateTime.UtcNow,
                            UpdatedUser = "system"
                        };

                    // Validación mínima: si se requiere Start/End para crear el ciclo...
                    if (newAuditCycle.StartDate == null || newAuditCycle.EndDate == null)
                    {
                        errorList.Add($"AuditStandard ID {auditStandard.ID} skipped: StartDate or EndDate is null");
                        continue;
                    }

                    newCycles.Add(newAuditCycle);
                    auditStandard.AuditCycleID = newAuditCycle.ID;
                }

                // Preparar actualización del AuditStandard para que apunte al nuevo ciclo o al existente                
                standardsToUpdate.Add(auditStandard);
            }

            // Guardar todos los nuevos ciclos en un solo SaveChanges (mejor rendimiento)
            if (newCycles.Count > 0)
            {
                try
                {
                    foreach (var c in newCycles) _repository.Add(c);
                    await _repository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Si falla aquí, no intentamos actualizar standards (evita inconsistencias)
                    throw new BusinessException($"CreateMissingAuditCyclesAsync (saving cycles): {ex.Message}");
                }
            }

            // Actualizar todos los AuditStandard en un solo SaveChanges
            if (standardsToUpdate.Count > 0)
            {
                try
                {
                    foreach (var s in standardsToUpdate) _auditStandardsRepository.Update(s);
                    await _auditStandardsRepository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"CreateMissingAuditCyclesAsync (updating auditStandards): {ex.Message}");
                }
            }

            if (errorList.Count > 0)
            {
                throw new BusinessException("Errors found creating missing audit cycles: "
                    + String.Join("; ", errorList));
            }
        } // CreateMissingAuditCycles

        // PRIVATE

        /// <summary>
        /// Realiza las validaciones necesarias al actualizar un ciclo de auditoría
        /// </summary>
        /// <param name="item">Objeto que contiene los datos que se van a actualizar</param>
        /// <param name="foundItem">Objeto que contiene los datos registrados en la base de datos</param>
        /// <returns></returns>
        private async Task ValidateUpdatedItemAsync(AuditCycle item, AuditCycle foundItem)
        {
            var _standardRepository = new StandardRepository();
            var _organizationStandardRepository = new OrganizationStandardRepository();

            // Validations

            // - Validar que el standard exista
            // - Validar que el standard pertenezca a la organizacion
            // - Validar que el standard tenga un status de activo
            // - Solo puede haber un ciclo activo por standard y organización
            // - Que el tipo de ciclo sea en el orden correcto (Initial -> Recertification, o Transfer -> Recertification)
            // - Si ya cuenta con una Auditoria Inicial o de Recertificacion, validar que tenga las fechas del certificado

            // - Validar que el standard exista
            var standard = await _standardRepository.GetAsync(item.StandardID ?? Guid.Empty)
                ?? throw new BusinessException("The standard does not exist");

            var organizationStandard = _organizationStandardRepository.Gets()
                .FirstOrDefault(e =>
                    e.OrganizationID == foundItem.OrganizationID
                    && e.StandardID == item.StandardID)
                ?? throw new BusinessException("The standard does not belong to the organization");

            if (foundItem.Status == StatusType.Nothing) // Solo si es nuevo...
            { 
                if (standard.Status != StatusType.Active)
                    throw new BusinessException("The standard is not active");

                // - Validar que el standard pertenezca a la organización
                if (organizationStandard.Status != StatusType.Active)
                    throw new BusinessException("The standard is not active for the organization");
            }

            // Solo si es Initial no validar hasta que haya una auditoria de ST2 terminada
            bool validateDates = false;

            if (item.CycleType == AuditCycleType.Recertification)
            {
                validateDates = true;
            }
            else if (item.CycleType == AuditCycleType.Transfer)
            {
                validateDates = true;
            } 
            else if (item.CycleType == AuditCycleType.Initial)
            {
                var _auditRepository = new AuditRepository();
                // Validar si ya existe una auditoría ST2 terminada para este ciclo
                var st2Audit = await _auditRepository
                    .GetAuditStepInAuditCycleAsync(AuditStepType.Stage2, foundItem.ID);
                if (st2Audit != null && st2Audit.Status >= AuditStatusType.Finished)
                {
                    validateDates = true;
                }
            }

            if (validateDates)
            {
                // Validar que existan las fechas de inicio y fin
                if (!item.StartDate.HasValue)
                    throw new BusinessException("The start date is required for the certificate cycle");

                if (!item.EndDate.HasValue)
                    throw new BusinessException("The end date is required for the certificate cycle");

                // - Que la fecha de inicio sea menor a la fecha de fin
                if (item.StartDate > item.EndDate)
                    throw new BusinessException("The start date must be less than the end date");
            }

            if (item.Status != foundItem.Status) // Si cambia el status...
            {
                switch (item.Status)
                {
                    case StatusType.Active:
                        // Validar que no exista otro ciclo activo con el mismo standard y organización
                        if (await _repository.IsAnyCycleActiveByOrganizationAndStandardAsync(
                            foundItem.OrganizationID,
                            foundItem.StandardID ?? Guid.Empty,
                            foundItem.ID))
                        {
                            throw new BusinessException("There is a other active cycle with the same standard");
                        }
                        break;
                }
            }

        } // ValidateUpdatedItemAsync
    }
}