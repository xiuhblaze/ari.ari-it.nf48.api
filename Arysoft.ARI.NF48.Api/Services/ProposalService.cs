using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

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

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty)
                items = items.Where(m => 
                    m.AuditCycle != null 
                    && m.AuditCycle.OrganizationID == filters.OrganizationID
                );

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(m =>
                    (m.Justification != null && m.Justification.ToLower().Contains(filters.Text))
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

            // TODO: Analizar alertas

            // Pagination

            var pagedItems = PagedList<Proposal>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Proposal> GetAsync(Guid id, bool asNoTracking = false)
        {
            // TODO: Analizar alertas 

            return await _repository.GetAsync(id, asNoTracking);
        } // GetAsync

        public async Task<Proposal> CreateAsync(Proposal item)
        {
            var adcRepository = new ADCRepository();
            
            await ValidateNewItemAsync(item);
            item = SetValuesForCreate(item);

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

            // Agregar los ProposalAudits si solo hay un (1) ADC disponible en el auditcycle
            if (await adcRepository
                .CountADCsAvailableByAuditCycleAsync(item.AuditCycleID) == 1)
            {
                // Agregar los ProposalAudits y asociar en el ADC la propuesta (ProposalID)
                var adcID = await adcRepository.GetADCIDAvailableByAuditCycleAsync(item.AuditCycleID);
                await AddADCAsync(item, adcID);

                // Reload item
                item = await _repository.GetAsync(item.ID)
                    ?? throw new BusinessException("The Proposal was not found after add audit steps totals");
            }

            return item;
        } // CreateAsync

        public async Task<Proposal> UpdateAsync(Proposal item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record does not exist.");

            // Validations

            await ValidateUpdatedItemAsync(item, foundItem);
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

        // ADCs

        public async Task AddADCAsync(Proposal item, Guid adcID)
        { 
            var adcService = new ADCService();
            var adcRepository = new ADCRepository();

            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("Proposal record not found");

            var adc = await adcRepository.GetAsync(adcID)
                ?? throw new BusinessException("ADC record not found");

            // Validar que no este el ADC ya asociado
            if (adc.ProposalID != null && adc.ProposalID != Guid.Empty)
                throw new BusinessException("The ADC is already associated with a proposal.");

            // Validar que sea del mismo AuditCycle
            if (adc.AuditCycleID != foundItem.AuditCycleID)
                throw new BusinessException("The ADC does not belong to the same Audit Cycle as the proposal.");

            await AddStepsFromADCAsync(foundItem, adc);
            await adcService.UpdateProposalIDAsync(adc.ID, item.ID, item.UpdatedUser);
            
            await CalculateStepsTotalsAsync(foundItem);

        } // AddADCAsync

        public async Task RemoveADCAsync(Proposal item, Guid adcID)
        {
            var adcService = new ADCService();
            var adcRepository = new ADCRepository();

            var adc = await adcRepository.GetAsync(adcID)
                ?? throw new BusinessException("ADC record not found");

            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("Proposal record not found");

            // Validar que no sea un ADC inactivo
            if (adc.Status > ADCStatusType.Active)
                throw new BusinessException("The ADC is not active to remove.");

            await adcService.RemoveProposalIDAsync(adcID, item.UpdatedUser);            
            await RemoveAllStepsAsync(foundItem.ID);
            await AddAllStepsAsync(foundItem);

            await CalculateStepsTotalsAsync(foundItem);
        } // RemoveADCAsync

        // PRIVATE FUNCTIONS

        // Create 

        private async Task ValidateNewItemAsync(Proposal item)
        {
            var adcRepository = new ADCRepository();

            // - Validar que exista al menos un ADC disponible en el auditcycle para asociar
            var countADCs = await adcRepository
                .CountADCsAvailableByAuditCycleAsync(item.AuditCycleID);

            if (countADCs == 0)
                throw new BusinessException("There are no ADCs available to be associated with the proposal.");

            if (countADCs == 1)
            { 
                // - Validar que no exista otra propuesta activa para el mismo ciclo de auditoría, pues solo hay un ADC
                if (await _repository.ExistsActiveProposalForAuditCycleAsync(item.AuditCycleID))
                    throw new BusinessException("There is already an active proposal for the selected audit cycle.");
            }

            // - Validar que el auditcycle y la organization esten activos
            if (!await _repository.HasValidParentsForCreateAsync(item))
                throw new BusinessException("The Organization or Audit cycle records are not valid.");

        } // ValidateNewItem

        private Proposal SetValuesForCreate(Proposal item)
        {   
            item.ID = Guid.NewGuid();
            item.CreatedBy = item.UpdatedUser;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;
            item.Status = ProposalStatusType.Nothing;

            return item;
        } // SetValuesForCreate

        // Update

        private async Task ValidateUpdatedItemAsync(Proposal item, Proposal foundItem)
        {
            // Si cambia el status, según el cambio validar...
            if (foundItem.Status != item.Status)
            {
                switch (item.Status) // Si el nuevo estatus es...
                {
                    case ProposalStatusType.New:
                        if (foundItem.Status != ProposalStatusType.Nothing)
                            throw new BusinessException("The record status can only be changed to New when is the first time.");
                        break;

                    case ProposalStatusType.Review:
                        if (foundItem.Status != ProposalStatusType.Nothing
                            && foundItem.Status != ProposalStatusType.New
                            && foundItem.Status != ProposalStatusType.Rejected
                            && foundItem.Status != ProposalStatusType.Cancel
                            )
                            throw new BusinessException("The proposal not can't be send to review.");
                        // Validar que tenga ADC asociados y activos
                        break;

                    case ProposalStatusType.Approved:
                        if (foundItem.Status != ProposalStatusType.Review)
                            throw new BusinessException("The record status can only be changed to Approved from Review.");
                        break;

                    case ProposalStatusType.Sended:
                        if (foundItem.Status != ProposalStatusType.Approved)
                            throw new BusinessException("The proposal can only be sent if it has been approved.");

                        if (string.IsNullOrEmpty(item.SignerName)
                            || string.IsNullOrEmpty(item.SignerPosition))
                            throw new BusinessException("The signatory's name and position are required before submitting the proposal.");

                        break;

                    case ProposalStatusType.Active:
                        // Validar que otra propuesa del mismo ciclo no esté activa
                        if (foundItem.Status != ProposalStatusType.Sended)
                            throw new BusinessException("The proposal can only be active if the client signed it.");
                        break;

                    case ProposalStatusType.Inactive:
                        if (foundItem.Status != ProposalStatusType.Active)
                            throw new BusinessException("Only active proposals can be set to inactive.");
                        break;

                    case ProposalStatusType.Deleted:
                        throw new BusinessException("The proposal can't be deleted. Use the Delete function");
                }
            }

            if (item.Status >= ProposalStatusType.New && string.IsNullOrEmpty(item.Justification))
                throw new BusinessException("The Justification is required.");

            // Solo si está activa la propuesta, validar ...
            if (foundItem.Status <= ProposalStatusType.Active)
            {
                // - Valida que la Organizacion siga siendo válida
                // - Valida que el AuditCycle siga siendo válido
                // - Valida los ADCs asociados sigan siendo válidos
                // - Valida que los AppForms de los ADCs asociados sigan siendo válidos
                if (!await _repository.HasValidParentsForUpdateAsync(foundItem))
                    throw new BusinessException("The Organization, Audit Cycle, App Form or ADC records are not valid.");
            }

        } // ValidateUpdatedItemAsync

        private Proposal SetValuesForUpdate(Proposal item, Proposal foundItem)
        {
            // Si cambia el status, según el cambio asignar...
            if (foundItem.Status != item.Status)
            {
                switch (item.Status)
                {
                    case ProposalStatusType.Review:
                        foundItem.ReviewDate = DateTime.UtcNow;                        
                        break;

                    case ProposalStatusType.Rejected:
                        foundItem.ReviewDate = DateTime.UtcNow;
                        break;

                    case ProposalStatusType.Sended:                        
                        foundItem.SignRequestDate = DateTime.UtcNow;
                        break;

                    //case ProposalStatusType.Active:
                    //    //foundItem.ActiveDate = DateTime.UtcNow;                        
                    //    break;

                    case ProposalStatusType.Inactive:
                        foundItem.HistoricalDataJSON = GetHistoricalDataJSON(foundItem);
                        break;
                }
            }

            // Si ya esta activo, esta información no se modifica
            if (foundItem.Status == ProposalStatusType.Nothing
                || foundItem.Status == ProposalStatusType.New
                || foundItem.Status == ProposalStatusType.Review
                || foundItem.Status == ProposalStatusType.Rejected
                )
            { 
                foundItem.Justification = item.Justification;   //HACK: Considerar si es generado o se recibe, creo que lo va a generar el front end para que se acepte visualmente
                foundItem.SignerName = item.SignerName;
                foundItem.SignerPosition = item.SignerPosition;
                foundItem.SignedFilename = item.SignedFilename;
                foundItem.CurrencyCode = item.CurrencyCode;
            }

            foundItem.Status = foundItem.Status == ProposalStatusType.Nothing && item.Status == ProposalStatusType.Nothing
                ? ProposalStatusType.New
                : item.Status != ProposalStatusType.Nothing
                    ? item.Status
                    : foundItem.Status;
            foundItem.Updated = DateTime.UtcNow;
            foundItem.UpdatedUser = item.UpdatedUser;

            return foundItem;
        } // SetValuesForUpdate

        // STEPS - ProposalAudits

        private async Task AddStepsFromADCAsync(Proposal proposal, ADC adc)
        {
            var adcRepository = new ADCRepository();
            var proposalAuditRepository = new ProposalAuditRepository();
            var mainADCSite = adc.ADCSites
                .Where(adcs => adcs.Status == StatusType.Active
                    && adcs.Site.IsMainSite)
                .FirstOrDefault()
                ?? throw new BusinessException("The ADC does not have the main site");
            bool hasChanges = false;

            foreach (var adcSiteAudit in mainADCSite.ADCSiteAudits
                .Where(asa => asa.Status == StatusType.Active))
            {
                if (adcSiteAudit.AuditStep == AuditStepType.Nothing) continue;

                // Validar si existe ya el ProposalAudit
                var proposalAudit = await proposalAuditRepository
                    .GetByProposalAndStepAsync(proposal.ID, adcSiteAudit.AuditStep ?? AuditStepType.Nothing);

                if (proposalAudit == null) // Si no existe, crearlo
                {
                    proposalAudit = new ProposalAudit
                    {
                        ID = Guid.NewGuid(),
                        ProposalID = proposal.ID,
                        AuditStep = adcSiteAudit.AuditStep,
                        Status = StatusType.Active,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        UpdatedUser = proposal.UpdatedUser
                    };
                    proposalAuditRepository.Add(proposalAudit);
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                try
                { 
                    await proposalAuditRepository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"ProposalService.AddStepsFromADCAsync: {ex.Message}");
                }
            }
        } // AddStepsFromADCAsync

        private async Task AddAllStepsAsync(Proposal proposal)
        {
            var adcRepository = new ADCRepository();
            var adcs = await adcRepository
                .GetsByProposalAsync(proposal.ID);

            foreach (var adc in adcs.Where(a => a.Status == ADCStatusType.Active))
            { 
                await AddStepsFromADCAsync(proposal, adc);
            }
        } // AddAllStepsAsync

        private async Task RemoveAllStepsAsync(Guid proposalID)
        {
            var proposalAuditRepository = new ProposalAuditRepository();

            try
            { 
                await proposalAuditRepository
                    .RemoveItemsByProposalID(proposalID);
                await proposalAuditRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ProposalService.RemoveAllStepsAsync: {ex.Message}");
            }
        } // RemoveAllSteps

        private async Task CalculateStepsTotalsAsync(Proposal proposal)
        {
            var proposalAuditRepository = new ProposalAuditRepository();
            var adcRepository = new ADCRepository();

            bool hasChanges = false;

            var proposalAuditSteps = await proposalAuditRepository
                .GetsByProposalAsync(proposal.ID);

            var adcs = await adcRepository
                .GetsByProposalAsync(proposal.ID);

            foreach (var proposalAudit in proposalAuditSteps.OrderBy(pas => pas.AuditStep))
            {
                var adcSiteList = new List<ADCSite>();

                foreach (var adc in adcs.Where(a => a.Status == ADCStatusType.Active))
                {
                    var adcSite = adc.ADCSites
                        .Where(asite => asite.Status == StatusType.Active
                            && asite.ADCSiteAudits
                                .Where(asa => asa.Status == StatusType.Active
                                    && (asa.Value.HasValue && asa.Value.Value)
                                    && asa.AuditStep == proposalAudit.AuditStep)
                                .Any()
                        ).FirstOrDefault();

                    if (adcSite != null) adcSiteList.Add(adcSite);
                } // Obteniendo los sites que tienen el step
                
                var hasStage1 = false;

                foreach (var adcSite in adcSiteList)
                {
                    proposalAudit.TotalAuditDays = proposalAudit.TotalAuditDays ?? 0;
                    switch (proposalAudit.AuditStep)
                    { 
                        case AuditStepType.Stage1:
                            proposalAudit.TotalAuditDays = 1;
                            hasStage1 = true;
                            break;
                        case AuditStepType.Stage2:                            
                            if (hasStage1)
                                proposalAudit.TotalAuditDays += adcSite.Total - 1 ?? 0;
                            else
                                proposalAudit.TotalAuditDays += adcSite.Total ?? 0;
                            break;
                        case AuditStepType.Surveillance1:
                        case AuditStepType.Surveillance2:
                        case AuditStepType.Surveillance3:
                        case AuditStepType.Surveillance4:
                        case AuditStepType.Surveillance5:
                            proposalAudit.TotalAuditDays += adcSite.Surveillance ?? 0;
                            break;
                        case AuditStepType.Recertification:
                            proposalAudit.TotalAuditDays += adcSite.Recertification ?? 0;
                            break;
                    }
                } // Por cada step, sumar el total de días de ese step

                proposalAudit.Updated = DateTime.UtcNow;
                proposalAudit.UpdatedUser = proposal.UpdatedUser;
                proposalAuditRepository.Update(proposalAudit);
                hasChanges = true;

                // TODO: Falta guardar los datos nuevos y ver si falta algun step para agregarlo
            }

            if (hasChanges)
            {
                try
                {
                    await proposalAuditRepository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"ProposalService.CalculateStepsTotalsAsync: {ex.Message}");
                }
            }   
        } // CalculateStepsTotalsAsync

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