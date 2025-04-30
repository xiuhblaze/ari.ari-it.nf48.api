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
    public class AuditAuditorService
    {
        public readonly AuditAuditorRepository _repository;

        // CONSTRUCTOR

        public AuditAuditorService()
        {
            _repository = new AuditAuditorRepository();
        }

        // METHODS

        public PagedList<AuditAuditor> Gets(AuditAuditorQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.AuditID != null && filters.AuditID != Guid.Empty)
            {
                items = items.Where(e => e.AuditID == filters.AuditID);
            }

            if (filters.IsLeader != null && filters.IsLeader != BoolType.Nothing)
            {
                items = items.Where(e => e.IsLeader == (filters.IsLeader == BoolType.True));
            }

            if (filters.IsWitness != null && filters.IsWitness != BoolType.Nothing)
            {
                items = items.Where(e => e.IsWitness == (filters.IsWitness == BoolType.True));
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
                case AuditAuditorOrderType.AuditorName:
                    items = items.OrderBy(e => e.Auditor.FirstName)
                        .ThenBy(e => e.Auditor.MiddleName)
                        .ThenBy(e => e.Auditor.LastName);
                    break;
                case AuditAuditorOrderType.IsLeader:
                    items = items.OrderBy(e => e.IsLeader)
                        .ThenBy(e => e.Auditor.FirstName)
                        .ThenBy(e => e.Auditor.MiddleName)
                        .ThenBy(e => e.Auditor.LastName);
                    break;
                case AuditAuditorOrderType.Status:
                    items = items.OrderBy(e => e.Status);
                    break;
                case AuditAuditorOrderType.AuditorNameDesc:
                    items = items.OrderByDescending(e => e.Auditor.FirstName)
                        .ThenByDescending(e => e.Auditor.MiddleName)
                        .ThenByDescending(e => e.Auditor.LastName);
                    break;
                case AuditAuditorOrderType.IsLeaderDesc:
                    items = items.OrderByDescending(e => e.IsLeader)
                        .ThenByDescending(e => e.Auditor.FirstName)
                        .ThenByDescending(e => e.Auditor.MiddleName)
                        .ThenByDescending(e => e.Auditor.LastName);
                    break;
                case AuditAuditorOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status);
                    break;
                default:
                    items = items.OrderBy(e => e.IsLeader)
                        .ThenBy(e => e.Auditor.FirstName)
                        .ThenBy(e => e.Auditor.MiddleName)
                        .ThenBy(e => e.Auditor.LastName);
                    break;
            }

            // Paging

            var pagedItems = PagedList<AuditAuditor>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<AuditAuditor> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<AuditAuditor> AddAsync(AuditAuditor item)
        {
            if (item.AuditID == null || item.AuditID == Guid.Empty)
                throw new BusinessException("Must first assign an audit");

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            // Execute queries

            try { 
                await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
                _repository.Add(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditAuditorService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<AuditAuditor> UpdateAsync(AuditAuditor item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations
            // - ISO9001: Que tenga los NACE codes relacionados a la auditoria (ver appForm)

            // - La primera vez debe de traer el ID del auditor
            if (foundItem.Status == StatusType.Nothing)
            { 
                if (item.AuditorID == null || item.AuditorID == Guid.Empty)
                    throw new BusinessException("Must first assign an auditor");

                foundItem.AuditorID = item.AuditorID;
            }

            // - El auditor no puede estar en dos auditorias al mismo tiempo,
            //   si la asiganción del auditor esta activa o se va a activar
            if (item.Status == StatusType.Active && foundItem.Audit != null
                && foundItem.Audit.StartDate.HasValue
                && foundItem.Audit.EndDate.HasValue) 
            {
                var auditsRepository = new AuditRepository();
                var hasAudit = await auditsRepository.HasAuditorAnAudit(
                    foundItem.AuditorID.Value,
                    foundItem.Audit.StartDate.Value,
                    foundItem.Audit.EndDate.Value,
                    foundItem.Audit.ID);
                if (hasAudit)
                    throw new BusinessException("The auditor is already assigned to another audit");
            }

            // - Que el auditor no sea líder y observador al mismo tiempo
            bool isLeader = item.IsLeader ?? false;
            bool isWitness = item.IsWitness ?? false;

            if (isWitness && isLeader)
                throw new BusinessException("An auditor cannot be both leader and witness");

            // Assigning values

            if (item.Status == StatusType.Nothing)
                item.Status = StatusType.Active;

            foundItem.IsLeader = isLeader;
            foundItem.IsWitness = isWitness;
            foundItem.Comments = item.Comments;
            foundItem.Status = foundItem.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status;
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
                throw new BusinessException($"AuditAuditorService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(AuditAuditor item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            if (foundItem.Status == StatusType.Deleted)
            {
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
                throw new BusinessException($"AuditAuditorService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync

        // AUDIT STANDARDS

        public async Task AddAuditStandardAsync(Guid id, Guid auditStandardID)
        {
            await _repository.AddAuditStandardAsync(id, auditStandardID);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditAuditorService.AddAuditStandardAsync: {ex.Message}");
            }
        } // AddAuditStandardAsync

        public async Task DelAuditStandardAsync(Guid id, Guid auditStandardID)
        {
            await _repository.DelAuditStandardAsync(id, auditStandardID);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"AuditAuditorService.DelAuditStandardAsync: {ex.Message}");
            }
        } // DelAuditStandardAsync
    }
}