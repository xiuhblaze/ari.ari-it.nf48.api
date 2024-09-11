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
    public class DayCalculationConceptApplicationService
    {
        private readonly DayCalculationConceptApplicationRepository _repository;

        // CONSTRUCTOR

        public DayCalculationConceptApplicationService()
        {
            _repository = new DayCalculationConceptApplicationRepository();
        }

        // METHODS

        public PagedList<DayCalculationConceptApplication> Gets(DayCalculationConceptApplicationQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.ApplicationID.HasValue && filters.ApplicationID != Guid.Empty)
            {
                items = items.Where(e => e.ApplicationID == filters.ApplicationID);
            }

            if (filters.DayCalculationConceptID.HasValue && filters.DayCalculationConceptID != Guid.Empty)
            {
                items = items.Where(e => e.DayCalculationConceptID == filters.DayCalculationConceptID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => e.Justification.ToLower().Contains(filters.Text)
                    || e.JustificationApproved.ToLower().Contains(filters.Text)
                );
            }

            if (filters.Unit.HasValue && filters.Unit != DayCalculationConceptUnitType.Nothing)
            {
                items = items.Where(e => e.Unit == filters.Unit);
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
                    : items.Where(e => e.Status != StatusType.Nothing && e.Status != StatusType.Deleted);
            }

            // OrderBy

            switch (filters.Order)
            {
                case DayCalculationConceptApplicationOrderType.DayCalculationConcept:
                    items = items.OrderBy(e => e.DayCalculationConcept.Description);
                    break;
                case DayCalculationConceptApplicationOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case DayCalculationConceptApplicationOrderType.DayCalculationConceptDesc:
                    items = items.OrderByDescending(e => e.DayCalculationConcept.Description);
                    break;
                case DayCalculationConceptApplicationOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
            }

            // Paging

            var pagedItems = PagedList<DayCalculationConceptApplication>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<DayCalculationConceptApplication> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<DayCalculationConceptApplication> AddAsync(DayCalculationConceptApplication item)
        {
            if (item.ApplicationID == Guid.Empty)
                throw new BusinessException("The ApplicationID must not be empty");

            if (item.DayCalculationConceptID == Guid.Empty)
                throw new BusinessException("The Day Calculation Concept Association must not be empty");

            item.ID = Guid.NewGuid();
            item.Unit = DayCalculationConceptUnitType.Nothing;
            item.Status = StatusType.Nothing;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            await _repository.DeleteTmpByUser(item.UpdatedUser);
            _repository.Add(item);
            _repository.SaveChanges();

            return item;
        } // AddAsync

        public async Task<DayCalculationConceptApplication> UpdateAsync(DayCalculationConceptApplication item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found"); 

            // Validations

            // - No duplicar el concepto en una misma application

            // Assigning values

            foundItem.Value = item.Value;
            foundItem.Justification = item.Justification;
            foundItem.ValueApproved = item.ValueApproved;
            foundItem.JustificationApproved = item.JustificationApproved;
            foundItem.Unit = item.Unit;
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
                throw new BusinessException(ex.Message);
            }

            return foundItem;
        } // UpdateAsync

        //TODO: Aqui voy!
    }
}