using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class ProposalService
    {
        private readonly ProposalRepository _repository;

        // CONSTRUCTOR

        public ProposalService()
        {
            _repository = new ProposalRepository();
        }

        // FUNCTIONS

        public PagedList<Proposal> Gets(ProposalQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.AuditCycleID != null && filters.AuditCycleID != Guid.Empty)
                items = items.Where(m => m.AuditCycleID == filters.AuditCycleID);

            if (filters.AppFormID != null && filters.AppFormID != Guid.Empty)
                items = items.Where(m => m.AppFormID == filters.AppFormID);

            if (filters.ADCID != null && filters.ADCID != Guid.Empty)
                items = items.Where(m => m.ADCID == filters.ADCID);

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(m =>
                    (m.ActivitiesScope != null && m.ActivitiesScope.ToLower().Contains(filters.Text))
                    || (m.Justification != null && m.Justification.ToLower().Contains(filters.Text))
                    || (m.SignerName != null && m.SignerName.ToLower().Contains(filters.Text))
                    || (m.SignerPosition != null && m.SignerPosition.ToLower().Contains(filters.Text))
                    || (m.HistoricalDataJSON != null && m.HistoricalDataJSON.ToLower().Contains(filters.Text))
                    || (m.UpdatedUser != null && m.UpdatedUser.ToLower().Contains(filters.Text))
                );
            }

            if (filters.Status != null && filters.Status != ProposalStatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != ProposalStatusType.Nothing)
                    : items.Where(e => e.Status != ProposalStatusType.Nothing
                        && e.Status != ProposalStatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            {
                case ProposalOrderType.Created:
                    items = items.OrderBy(m => m.Created);
                    break;
                case ProposalOrderType.Status:
                    items = items.OrderBy(m => m.Status);
                    break;
                case ProposalOrderType.CreatedDesc:
                    items = items.OrderByDescending(m => m.Created);
                    break;
                case ProposalOrderType.StatusDesc:
                    items = items.OrderByDescending(m => m.Status);
                    break;
                default:
                    items = items.OrderByDescending(m => m.Created);
                    break;
            }

            // - Analizar alertas de cambio de alcance o empleados

            // Pagination

            var pagedItems = PagedList<Proposal>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Proposal> GetAsync(Guid id, bool asNoTracking = false)
        {
            // - Analizar alertas de cambio de alcance o empleados
            return await _repository.GetAsync(id, asNoTracking);
        } // GetAsync

        public async Task<Proposal> CreateAsync(Proposal item)
        {
            await ValidateNewItemAsync(item);
            item = await SetValuesForCreateAsync(item);

            // Excecute queries

            try
            { 
                await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
                _repository.Add(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ProposalService.CreateAsync: {ex.Message}");
            }

            // 1. Agregar los sitios del proposal
            // 2. Agregar los datos del appform y adc necesarios

            return item;
        } // CreateAsync

        public async Task<Proposal> UpdateAsync(Proposal item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record does not exist.");

            // Validations

            ValidateUpdatedItem(item, foundItem);
            foundItem = SetValuesForUpdate(item, foundItem);

            try 
            { 
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ProposalService.UpdateAsync: {ex.Message}");
            }

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Proposal item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record does not exist.");

            if (foundItem.Status == ProposalStatusType.Deleted)
            { 
                // HACK: Realizar validaciones
                _repository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status < ProposalStatusType.Cancel
                    ? ProposalStatusType.Cancel
                    : ProposalStatusType.Deleted;
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
                throw new BusinessException($"ProposalService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync

        // PRIVATE FUNCTIONS

        // Create 

        private async Task ValidateNewItemAsync(Proposal item)
        {
            // Validations
            
            // - Validar que el ADC no tenga otra propuesta valida
            if (await _repository.ADCHasValidProposalAsync(item.ADCID))
                throw new BusinessException("The ADC already has a valid proposal.");
            
            // - Validar que el appform, auditcycle y la organization esten activos, o algo así
            if (await _repository.HasValidParentsAsync(item))
                throw new BusinessException("The Organization, Audit cycle, App form or ADC records are not valid.");

        } // ValidateNewItem

        private async Task<Proposal> SetValuesForCreateAsync(Proposal item)
        {
            var _appformRepository = new AppFormRepository();
            var _adcRepository = new ADCRepository();
            var _md5Repository = new MD5Repository();

            var appForm = await _appformRepository.GetAsync(item.AppFormID)
                ?? throw new BusinessException("App Form record not found");
            var adc = await _adcRepository.GetAsync(item.ADCID)
                ?? throw new BusinessException("ADC record not found");
            var md5 = await _md5Repository.GetByEmployeesAsync(adc.TotalEmployees ?? 0);

            item.ID = Guid.NewGuid();

            item.ActivitiesScope = appForm.ActivitiesScope;
            item.TotalEmployees = adc.TotalEmployees;
            item.MD5ID = md5.ID;

            item.UserCreates = item.UpdatedUser;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;
            item.Status = ProposalStatusType.Nothing;

            return item;
        } // SetValuesForCreateAsync

        // Update

        private void ValidateUpdatedItem(Proposal item, Proposal foundItem)
        {

            // Si cambia el status, según el cambio validar...
            if (foundItem.Status != item.Status)
            {
                switch (item.Status)
                {
                    case ProposalStatusType.Review:
                        if (foundItem.Status != ProposalStatusType.New
                            && foundItem.Status != ProposalStatusType.Rejected
                            && foundItem.Status != ProposalStatusType.Cancel
                            )
                            throw new BusinessException("The proposal not can't be send to review.");
                        break;
                    case ProposalStatusType.Approved:
                        if (foundItem.Status != ProposalStatusType.Review)
                            throw new BusinessException("The record status can only be changed to Approved from Review.");
                        break;
                    case ProposalStatusType.Sended:
                        if (foundItem.Status != ProposalStatusType.Approved)
                            throw new BusinessException("The proposal can only be sent if it has been approved.");
                        break;
                    case ProposalStatusType.Active:
                        if (foundItem.Status != ProposalStatusType.Sended)
                            throw new BusinessException("The proposal can only be active if the client signed it.");
                        break;
                    case ProposalStatusType.Deleted:
                        throw new BusinessException("The proposal can't be deleted. Use the Delete function");
                }
            }

        } // ValidateUpdatedItem

        private Proposal SetValuesForUpdate(Proposal item, Proposal foundItem)
        {
            // Si cambia el status, según el cambio asignar...
            if (foundItem.Status != item.Status)
            {
                switch (item.Status)
                {
                    case ProposalStatusType.Review:
                        foundItem.ReviewDate = DateTime.UtcNow;
                        foundItem.UserReview = item.UpdatedUser;
                        break;
                    case ProposalStatusType.Sended:
                        if(string.IsNullOrEmpty(item.SignerName)
                            || string.IsNullOrEmpty(item.SignerPosition))
                            throw new BusinessException("The signatory's name and position are required before submitting the proposal.");
                        foundItem.SendToSignDate = DateTime.UtcNow;
                        break;
                    case ProposalStatusType.Active:
                        foundItem.ActiveDate = DateTime.UtcNow;
                        break;
                    case ProposalStatusType.Inactive:
                        foundItem.HistoricalDataJSON = GetHistoricalDataJSON(foundItem);
                        break;
                }
            }

            foundItem.MD5ID = item.MD5ID;
            foundItem.ActivitiesScope = item.ActivitiesScope;
            foundItem.TotalEmployees = item.TotalEmployees; //HACK: Considerar si es calculado o se recibe
            foundItem.Justification = item.Justification;   //HACK: Considerar si es generadp se recibe
            foundItem.SignerName = item.SignerName;
            foundItem.SignerPosition = item.SignerPosition;
            foundItem.SigendFilename = item.SigendFilename;
            foundItem.CurrencyCode = item.CurrencyCode;
            foundItem.Status = foundItem.Status == ProposalStatusType.Nothing && item.Status == ProposalStatusType.Nothing
                ? ProposalStatusType.New
                : item.Status != ProposalStatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            return foundItem;
        } // SetValuesForUpdate

        private string GetHistoricalDataJSON(Proposal item)
        {
            // HACK: Falta completar los datos -xBlaze: 20251007

            var historicalData = new
            {
                OrganizationName = item.AuditCycle?.Organization?.Name ?? string.Empty,
                AuditCycleName = item.AuditCycle?.Name ?? string.Empty,
                AppForm = new {
                    MainSiteAddress = "",
                    LegalEntities = "", // array
                    Website = "",
                    Phone = "",
                    ContactName = "",
                    EMail = ""
                },
                ADC = new { 
                    Description = "",
                    TotalEmployees = 0,
                    TotalInitial = 0,
                    TotalMD11 = 0,
                    TotalSurveillance = 0,                    
                },
                //Sites = item.ProposalSites?
                //    .Where(ps => ps.Status == StatusType.Active)
                //    .Select(ps => new { 
                //        Description = "",
                //        IsMainSite = false,
                //        Address = "",
                //        Country = ""
                //    })
            };

            return JsonSerializer.Serialize(historicalData);
        } // GetHistoricalDataJSON

    }
}