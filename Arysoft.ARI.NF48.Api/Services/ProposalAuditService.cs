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
    public class ProposalAuditService
    {
        public readonly ProposalAuditRepository _repository;

        // CONSTRUCTOR

        public ProposalAuditService()
        {
            _repository = new ProposalAuditRepository();
        }

        // METHODS

        public PagedList<ProposalAudit> Gets(ProposalAuditQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.ProposalID != null && filters.ProposalID != Guid.Empty)
            { 
                items = items.Where(x => x.ProposalID == filters.ProposalID);
            }

            if (filters.Status.HasValue && filters.Status != StatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != StatusType.Nothing)
                    : items.Where(e => e.Status != StatusType.Nothing
                        && e.Status != StatusType.Deleted);
            }

            // Order

            switch(filters.Order)
            {
                case ProposalAuditOrderType.AuditStep:
                    items = items.OrderBy(e => e.AuditStep);
                    break;
                case ProposalAuditOrderType.TotalCost:
                    items = items.OrderBy(e => e.TotalCost);
                    break;
                case ProposalAuditOrderType.AuditStepDesc:
                    items = items.OrderByDescending(e => e.AuditStep);
                    break;
                case ProposalAuditOrderType.TotalCostDesc:
                    items = items.OrderByDescending(e => e.TotalCost);
                    break;
            }

            var pagedItems = PagedList<ProposalAudit>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<ProposalAudit> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        /// <summary>
        /// Creo que se van a crear desde la propuesta (Proposal)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<ProposalAudit> CreateAsync(ProposalAudit item)
        {
            // Validate 

            if (item.ProposalID == null || item.ProposalID == Guid.Empty)
                throw new BusinessException("ProposalID is required");

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
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ProposalAuditService.CreateAsync: {ex.Message}");
            }

            return item;
        } // CreateAsync

        public async Task<ProposalAudit> UpdateAsync(ProposalAudit item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - Validar que no esté duplicado el AuditStep dentro de la misma propuesta
            // - Validar que solo estén los AuditStep validos para el tipo de ciclo de auditoría

            // Assigning values
            foundItem = SetValuesUpdateItem(item, foundItem);

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ProposalAudit.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task<List<ProposalAudit>> UpdatedListAsync(List<ProposalAudit> items)
        {
            var areUpdatedItems = false;
            var updatedItems = new List<ProposalAudit>();

            foreach (var item in items)
            {
                var foundItem = await _repository.GetAsync(item.ID)
                    ?? throw new BusinessException($"One of the records (Proposl Audit) to update was not found: {item.ID}");

                foundItem = SetValuesUpdateItem(item, foundItem);

                _repository.Update(foundItem);
                areUpdatedItems = true;
                updatedItems.Add(foundItem);
            }

            if (areUpdatedItems)
            {
                try
                {
                    await _repository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"ProposalAuditService.UpdatedListAsync: {ex.Message}");
                }
            }

            return updatedItems;
        } // UpdatedListAsync

        public async Task DeleteAsync(ProposalAudit item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");
                        
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
                throw new BusinessException($"ProposalAuditService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync



        // PRIVATE

        private ProposalAudit SetValuesUpdateItem(ProposalAudit item, ProposalAudit foundItem)
        {   
            foundItem.TotalAuditDays = item.TotalAuditDays;
            foundItem.CertificateIssue = item.CertificateIssue;
            foundItem.TotalCost = item.TotalCost;
            foundItem.Status = foundItem.Status == StatusType.Nothing && item.Status == StatusType.Nothing
                ? StatusType.Active
                : item.Status != StatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            return foundItem;
        } // SetValuesUpdateItem
    }
}