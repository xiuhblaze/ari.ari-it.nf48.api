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
using System.Web.Http.Results;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class ADCService
    {
        public readonly ADCRepository _repository;

        // CONSTRUCTOR

        public ADCService()
        {
            _repository = new ADCRepository();
        }

        // METHODS

        public PagedList<ADC> Gets(ADCQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.AppFormID != null && filters.AppFormID != Guid.Empty)
            {
                items = items.Where(e => e.AppFormID == filters.AppFormID);
            }

            if (filters.Status.HasValue && filters.Status != ADCStatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != ADCStatusType.Nothing)
                    : items.Where(e => e.Status != ADCStatusType.Nothing
                        && e.Status != ADCStatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            {
                case ADCOrderType.Description:
                    items = items.OrderBy(e => e.Description);
                    break;
                case ADCOrderType.Created:
                    items = items.OrderBy(e => e.Created);
                    break;
                case ADCOrderType.DescriptionDesc:
                    items = items.OrderBy(e => e.Description);
                    break;
                case ADCOrderType.CreatedDesc:
                    items = items.OrderByDescending(e => e.Created);
                    break;
                default:
                    items = items.OrderByDescending(e => e.Created);
                    break;
            }
            
            var pagedItems = PagedList<ADC>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<ADC> GetAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<ADC> AddAsync(ADC item)
        {
            // Validations

            if (item.AppFormID == null || item.AppFormID == Guid.Empty)            
                throw new ArgumentException("The AppForm ID is required.");
            
            
            // Set default values
            
            item.ID = Guid.NewGuid();
            item.UserCreate = item.UpdatedUser;
            item.Status = ADCStatusType.New;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            try
            { 
                await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
                _repository.Add(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<ADC> UpdateAsync(ADC item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            // - no se me ocurre que ahorita

            if (item.Status < ADCStatusType.Inactive)
            { 
                RecalcularTotales(item);
            }

            // - Dependiendo del status, realizar diferentes acciones
            if (foundItem.Status != item.Status)
            {
                // - 'orita no me acuerdo...
                switch (item.Status)
                {
                    case ADCStatusType.Review:
                        if (string.IsNullOrEmpty(item.ReviewComments))
                            throw new BusinessException("Comments are required when send to Review.");

                        foundItem.ReviewDate = DateTime.UtcNow;
                        foundItem.ReviewComments = item.ReviewComments;
                        break;

                    case ADCStatusType.Rejected:
                        if (string.IsNullOrEmpty(item.ReviewComments))
                            throw new BusinessException("Comments are required when rejected.");

                        foundItem.ReviewDate = DateTime.UtcNow;
                        foundItem.ReviewComments = item.ReviewComments;
                        foundItem.UserReviewer = item.UpdatedUser;
                        break;

                    case ADCStatusType.Active:
                        if (foundItem.Status != ADCStatusType.Review)
                            throw new BusinessException("Only items in Review can be set to Active.");
                        foundItem.UserReviewer = item.UpdatedUser;
                        foundItem.ActiveDate = DateTime.UtcNow;
                        break;

                    case ADCStatusType.Inactive:

                        break;
                }
            }

            // Assigning values

            foundItem.Description = item.Description;
            foundItem.ExtraInfo = item.ExtraInfo;
            foundItem.Status = foundItem.Status == ADCStatusType.Nothing && item.Status == ADCStatusType.Nothing
                ? ADCStatusType.New
                : item.Status != ADCStatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        // PRIVATE

        public void RecalcularTotales(ADC item)
        {
            if (item.ADCSites != null && item.ADCSites.Any())
            {
                var totalEmployees = 0;
                decimal totalMD11 = 0;

                foreach (var site in item.ADCSites
                    .Where(adcsite => adcsite.Status == StatusType.Active))
                {   
                    if (site.ADCConceptValues.Any())
                    { 
                        foreach (var conceptValue in site.ADCConceptValues
                            .Where(acv => acv.Status == StatusType.Active))
                        {
                            //TODO: Evaluar cada Concept para sacar el Value o algo asi jojojo...
                        }
                    }

                    totalEmployees += site.Site.Shifts
                        .Where(s => s.Status == StatusType.Active)
                        .Sum(s => s.NoEmployees) ?? 0;

                    totalMD11 += site.MD11 ?? 0;
                }

                item.TotalEmployees = totalEmployees;
                item.TotalMD11 = totalMD11;
            }
        }
    }
}