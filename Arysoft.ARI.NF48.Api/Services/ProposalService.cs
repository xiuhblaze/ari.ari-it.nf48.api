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
                items = items.Where(i => 
                    i.ADCs.Where(adc => adc.AuditCycleID == filters.AuditCycleID).Any()
                );

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty)
                items = items.Where(i => i.OrganizationID == filters.OrganizationID);

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(m =>
                    (m.Justification != null && m.Justification.ToLower().Contains(filters.Text))
                    || (m.SignerName != null && m.SignerName.ToLower().Contains(filters.Text))
                    || (m.SignerPosition != null && m.SignerPosition.ToLower().Contains(filters.Text))
                    //|| (m.HistoricalDataJSON != null && m.HistoricalDataJSON.ToLower().Contains(filters.Text))
                    || (m.UpdatedUser != null && m.UpdatedUser.ToLower().Contains(filters.Text))
                    || (m.Organization != null & m.Organization.Name != null && m.Organization.Name.ToLower().Contains(filters.Text))
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
            var item = await _repository.GetAsync(id, asNoTracking)
                ?? throw new BusinessException("The record does not exist.");

            if (item.Status <= ProposalStatusType.Rejected)
            {
                await AddOrUpdateStepsAsync(item);
                await CalculateStepsTotalsAsync(item);

                item = await _repository.GetAsync(id, asNoTracking)
                    ?? throw new BusinessException("The record does not exist after updating steps.");
            }

            return item; //await _repository.GetAsync(id, asNoTracking);
        } // GetAsync

        public async Task<Proposal> CreateAsync(Proposal item)
        {
            //var adcRepository = new ADCRepository();
            
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

        public async Task<Proposal> UpdateCompleteAsync(Proposal item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record does not exist.");
            var _proposalAuditsService = new ProposalAuditService();

            // Validations

            await ValidateUpdatedItemAsync(item, foundItem);
            foundItem = SetValuesForUpdate(item, foundItem);

            var listProposalAudits = new List<ProposalAudit>();

            if (item.ProposalAudits?.Any() ?? false)
            { 
                listProposalAudits = await _proposalAuditsService
                    .UpdatedListAsync(item.ProposalAudits.ToList());
            }

            try
            {
                _repository.Update(foundItem);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ProposalService.UpdateCompleteAsync: {ex.Message}");
            }

            foundItem.ProposalAudits = listProposalAudits;

            // Calcular los totales de los steps
            await CalculateStepsTotalsAsync(foundItem);

            // Reload item
            foundItem = await _repository.GetAsync(foundItem.ID)
                ?? throw new BusinessException("The Proposal was not found after update complete");
            return foundItem;

        } // UpdateCompleteAsync

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
            if (adc.ProposalID != null && adc.ProposalID != Guid.Empty
                && adc.Proposal.Status != ProposalStatusType.Nothing)
                throw new BusinessException("The ADC is already associated with a proposal.");

            await adcService.UpdateProposalIDAsync(adc.ID, item.ID, item.UpdatedUser);
            
            //await AddStepsFromADCAsync(foundItem, adc);
            await AddOrUpdateStepsAsync(foundItem);
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

            //await AddAllStepsAsync(foundItem);
            await AddOrUpdateStepsAsync(foundItem);
            await CalculateStepsTotalsAsync(foundItem);
        } // RemoveADCAsync

        // PRIVATE FUNCTIONS

        // Create 

        private async Task ValidateNewItemAsync(Proposal item)
        {
            var adcRepository = new ADCRepository();

            // - Validar que exista al menos un ADC disponible en la Organización para asociar
            var countADCs = await adcRepository
                .CountADCsAvailableByOrganizationAsync(item.OrganizationID);

            if (countADCs == 0)
                throw new BusinessException("There are no ADCs available to be associated with the proposal.");

            // - Validar que el auditcycle y la organization esten activos
            // TODO: Esta validación también se va a transladar al momento de asignar un ADC a la Propuesta
            //if (!await _repository.HasValidParentsForCreateAsync(item))
            //    throw new BusinessException("The Organization or Audit cycle records are not valid.");

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

                    case ProposalStatusType.Sent:
                        if (foundItem.Status != ProposalStatusType.Approved)
                            throw new BusinessException("The proposal can only be sent if it has been approved.");

                        if (string.IsNullOrEmpty(item.SignerName)
                            || string.IsNullOrEmpty(item.SignerPosition))
                            throw new BusinessException("The signatory's name and position are required before submitting the proposal.");

                        break;

                    case ProposalStatusType.Active:
                        // Validar que otra propuesa del mismo ciclo no esté activa
                        if (foundItem.Status != ProposalStatusType.Sent)
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

                    case ProposalStatusType.Sent:
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
                foundItem.Justification = item.Justification;   // Va a ser generado en el frontend
                foundItem.SignerName = item.SignerName;
                foundItem.SignerPosition = item.SignerPosition;
                foundItem.CurrencyCode = item.CurrencyCode;
                foundItem.ExchangeRate = item.ExchangeRate;
                foundItem.TaxRate = item.TaxRate;
                foundItem.IncludeTravelExpenses = item.IncludeTravelExpenses;
            }

            foundItem.SignedFilename = item.SignedFilename; // Considerar si es obligatorio al momenta de pasar a Active
            foundItem.ExtraInfo = item.ExtraInfo;
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

            // Lista temporal con par (AuditStep, Days)
            var auditStepDays = new List<(AuditStepType AuditStep, decimal Days)>();

            // Recolectar los días de todos los ADCSite del ADC
            foreach (var adcSite in adc.ADCSites
                .Where(adcs => adcs.Status == StatusType.Active))
            {
                foreach (var adcSiteAudit in adcSite.ADCSiteAudits
                    .Where(asa => asa.Status == StatusType.Active))
                {
                    if (adcSiteAudit.AuditStep == AuditStepType.Nothing) continue;
                    var step = adcSiteAudit.AuditStep ?? AuditStepType.Nothing;
                    var days = (decimal)(adcSiteAudit.Days ?? 0);
                    var idx = auditStepDays.FindIndex(x => x.AuditStep == step);
                    if (idx >= 0)
                    {
                        var (AuditStep, Days) = auditStepDays[idx];
                        auditStepDays[idx] = (AuditStep, Days + days);
                    }
                    else
                    {
                        auditStepDays.Add((step, days));
                    }
                }
            }

            //// Recolectar desde el main site del ADC actual
            //foreach (var adcSiteAudit in mainADCSite.ADCSiteAudits
            //    .Where(asa => asa.Status == StatusType.Active))
            //{
            //    if (adcSiteAudit.AuditStep == AuditStepType.Nothing) continue;

            //    var step = adcSiteAudit.AuditStep ?? AuditStepType.Nothing;
            //    var days = (decimal)(adcSiteAudit.Days ?? 0);

            //    var idx = auditStepDays.FindIndex(x => x.AuditStep == step);
            //    if (idx >= 0)
            //    {
            //        var (AuditStep, Days) = auditStepDays[idx];
            //        auditStepDays[idx] = (AuditStep, Days + days);
            //    }
            //    else
            //    {
            //        auditStepDays.Add((step, days));
            //    }
            //}

            // Incluir ADCS ya asociados a la propuesta (si existen) y acumular días
            var adcsByProposal = adcRepository.Gets()
                .Where(a => a.ProposalID == proposal.ID
                    && a.Status == ADCStatusType.Active);

            foreach (var adcByProposal in adcsByProposal)
            {
                var mainSiteByProposal = adcByProposal.ADCSites
                    .Where(adcs => adcs.Status == StatusType.Active
                        && adcs.Site.IsMainSite)
                    .FirstOrDefault()
                    ?? throw new BusinessException("The ADC associated with the proposal does not have the main site");

                foreach (var adcSite in adcByProposal.ADCSites
                    .Where(adcs => adcs.Status == StatusType.Active))
                    {
                        foreach (var adcSiteAudit in adcSite.ADCSiteAudits
                            .Where(asa => asa.Status == StatusType.Active))
                        {
                            if (adcSiteAudit.AuditStep == AuditStepType.Nothing) continue;
                            var step = adcSiteAudit.AuditStep ?? AuditStepType.Nothing;
                            var days = (decimal)(adcSiteAudit.Days ?? 0);
                            var idx = auditStepDays.FindIndex(x => x.AuditStep == step);
                            if (idx >= 0)
                            {
                                var (AuditStep, Days) = auditStepDays[idx];
                                auditStepDays[idx] = (AuditStep, Days + days);
                            }
                            else
                            {
                                auditStepDays.Add((step, days));
                            }
                        }
                    }

                //foreach (var adcSiteAudit in mainSiteByProposal.ADCSiteAudits
                //    .Where(asa => asa.Status == StatusType.Active))
                //{
                //    if (adcSiteAudit.AuditStep == AuditStepType.Nothing) continue;

                //    var step = adcSiteAudit.AuditStep ?? AuditStepType.Nothing;
                //    var days = (decimal)(adcSiteAudit.Days ?? 0);

                //    var idx = auditStepDays.FindIndex(x => x.AuditStep == step);
                //    if (idx >= 0)
                //    {
                //        var (AuditStep, Days) = auditStepDays[idx];
                //        auditStepDays[idx] = (AuditStep, Days + days);
                //    }
                //    else
                //    {
                //        auditStepDays.Add((step, days));
                //    }
                //}
            }

            // Ahora usa auditStepDays para crear/actualizar ProposalAudits
            foreach (var (AuditStep, Days) in auditStepDays)
            {
                // ejemplo: obtener el proposalAudit por step y asignar TotalAuditDays = entry.Days
                var proposalAudit = await proposalAuditRepository
                    .GetByProposalAndStepAsync(proposal.ID, AuditStep);

                if (proposalAudit == null)
                {
                    proposalAudit = new ProposalAudit
                    {
                        ID = Guid.NewGuid(),
                        ProposalID = proposal.ID,
                        AuditStep = AuditStep,
                        TotalAuditDays = Days,
                        Status = StatusType.Active,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        UpdatedUser = proposal.UpdatedUser
                    };
                    proposalAuditRepository.Add(proposalAudit);
                    hasChanges = true;
                }
                else
                {
                    // si ya existe actualizar días si es necesario
                    if (proposalAudit.TotalAuditDays != Days)
                    {
                        proposalAudit.TotalAuditDays = Days;
                        proposalAudit.Updated = DateTime.UtcNow;
                        proposalAudit.UpdatedUser = proposal.UpdatedUser;
                        proposalAuditRepository.Update(proposalAudit);
                        hasChanges = true;
                    }
                }
            }

            //// Esta parte es cuando haya multiples ADCs asociados a la Propuesta
            //var adcsByProposal = adcRepository.Gets()
            //    .Where(a => a.ProposalID == proposal.ID
            //        && a.Status == ADCStatusType.Active);

            //foreach (var adcByProposal in adcsByProposal)
            //{ 
            //    var mainSiteByProposal = adcByProposal.ADCSites
            //        .Where(adcs => adcs.Status == StatusType.Active
            //            && adcs.Site.IsMainSite)
            //        .FirstOrDefault()
            //        ?? throw new BusinessException("The ADC associated with the proposal does not have the main site");
            //    foreach (var adcSiteAudit in mainSiteByProposal.ADCSiteAudits
            //        .Where(asa => asa.Status == StatusType.Active))
            //    {
            //        if (adcSiteAudit.AuditStep == AuditStepType.Nothing) continue;
            //        if (!auditSteps.Contains(adcSiteAudit.AuditStep ?? AuditStepType.Nothing))
            //            auditSteps.Add(adcSiteAudit.AuditStep ?? AuditStepType.Nothing);
            //    }
            //}


            //foreach (var adcSiteAudit in mainADCSite.ADCSiteAudits
            //    .Where(asa => asa.Status == StatusType.Active))
            //{
            //    if (adcSiteAudit.AuditStep == AuditStepType.Nothing) continue;

            //    // Si es Preaudit

            //    // Validar si existe ya el ProposalAudit
            //    var proposalAudit = await proposalAuditRepository
            //        .GetByProposalAndStepAsync(proposal.ID, adcSiteAudit.AuditStep ?? AuditStepType.Nothing);

            //    if (proposalAudit == null) // Si no existe, crearlo
            //    {
            //        proposalAudit = new ProposalAudit // TODO: Verificar esto para que lleven los días de Auditoria indicados
            //        {
            //            ID = Guid.NewGuid(),
            //            ProposalID = proposal.ID,
            //            AuditStep = adcSiteAudit.AuditStep,
            //            TotalAuditDays = adcSiteAudit.Days ?? 0, // Porque existe preaudit y stage 1 con posibles valores
            //            Status = StatusType.Active,
            //            Created = DateTime.UtcNow,
            //            Updated = DateTime.UtcNow,
            //            UpdatedUser = proposal.UpdatedUser
            //        };
            //        proposalAuditRepository.Add(proposalAudit);
            //        hasChanges = true;
            //    }
            //}

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

        private async Task AddOrUpdateStepsAsync(Proposal proposal)
        {
            var adcRepository = new ADCRepository();
            var proposalAuditRepository = new ProposalAuditRepository();
            bool hasChanges = false;

            var auditStepDays = new List<(AuditStepType AuditStep, decimal Days)>();

            var adcs = await adcRepository
                .GetsByProposalAsync(proposal.ID);

            foreach (var adc in adcs.Where(a => a.Status == ADCStatusType.Active))
            { 
                foreach (var adcSite in adc.ADCSites
                    .Where(adcSite => adcSite.Status == StatusType.Active))
                {
                    foreach (var adcSiteAudit in adcSite.ADCSiteAudits
                        .Where(asa => asa.Status == StatusType.Active))
                    {
                        if (adcSiteAudit.AuditStep == AuditStepType.Nothing) continue;
                        var step = adcSiteAudit.AuditStep ?? AuditStepType.Nothing;
                        var days = (decimal)(adcSiteAudit.Days ?? 0);
                        var idx = auditStepDays.FindIndex(x => x.AuditStep == step);
                        if (idx >= 0)
                        {
                            var (AuditStep, Days) = auditStepDays[idx];
                            auditStepDays[idx] = (AuditStep, Days + days);
                        }
                        else
                        {
                            auditStepDays.Add((step, days));
                        }
                    }
                }
            }

            // Ahora usa auditStepDays para crear/actualizar ProposalAudits
            foreach (var (AuditStep, Days) in auditStepDays)
            {
                var proposalAudit = await proposalAuditRepository
                    .GetByProposalAndStepAsync(proposal.ID, AuditStep);
                if (proposalAudit == null)
                {
                    proposalAudit = new ProposalAudit
                    {
                        ID = Guid.NewGuid(),
                        ProposalID = proposal.ID,
                        AuditStep = AuditStep,
                        TotalAuditDays = Days,
                        Status = StatusType.Active,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        UpdatedUser = proposal.UpdatedUser
                    };

                    proposalAuditRepository.Add(proposalAudit);
                    hasChanges = true;
                }
                else
                {
                    // si ya existe actualizar días si es necesario
                    if (proposalAudit.TotalAuditDays != Days)
                    {
                        proposalAudit.TotalAuditDays = Days;
                        proposalAudit.Updated = DateTime.UtcNow;
                        proposalAudit.UpdatedUser = proposal.UpdatedUser;

                        proposalAuditRepository.Update(proposalAudit);
                        hasChanges = true;
                    }
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
                    throw new BusinessException($"ProposalService.AddOrUpdateStepsAsync: {ex.Message}");
                }
            }
        } // AddOrUpdateStepsAsync

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
            //var proposalAuditStage1 = proposalAuditSteps
            //    .Where(pas => pas.AuditStep == AuditStepType.Stage1);

            foreach (var proposalAudit in proposalAuditSteps)
            {                
                var adcSiteList = new List<ADCSite>();

                // Obteniendo los sites que tienen el step de todos los ADCs
                // asociados a la propuesta
                foreach (var adc in adcs.Where(a => a.Status == ADCStatusType.Active))
                {
                    var adcSites = adc.ADCSites
                        .Where(asite => asite.Status == StatusType.Active
                            && asite.ADCSiteAudits
                                .Where(asa => asa.Status == StatusType.Active
                                    && (asa.Value.HasValue && asa.Value.Value)
                                    && asa.AuditStep == proposalAudit.AuditStep)
                                .Any()
                        );

                    if (adcSites != null) {
                        foreach (var adcSite in adcSites) {
                            adcSiteList.Add(adcSite);
                        }
                    }
                }

                decimal totalAuditDays = 0;
                foreach (var adcSite in adcSiteList)
                {
                    switch (proposalAudit.AuditStep)
                    {
                        // case AuditStepType.PreAudit: // La suma ya viene desde AddOrUpdateStepsAsync
                        // case AuditStepType.Stage1: // La suma ya viene desde AddOrUpdateStepsAsync
                        case AuditStepType.Stage2:
                            
                            // Revisar si stage1 existe y se va auditar para ese sitio,
                            // si es así, restar lo que contenga en adcSiteAudit.Days
                            var stage1 = adcSite.ADCSiteAudits
                                .Where(asa => asa.Status == StatusType.Active
                                    && (asa.Value.HasValue && asa.Value.Value)
                                    && asa.AuditStep == AuditStepType.Stage1)
                                .FirstOrDefault();
                            if (stage1 != null)
                            {
                                totalAuditDays += (adcSite.Total ?? 0) - (stage1.Days ?? 0);
                            }
                            else
                            { 
                                totalAuditDays += adcSite.Total ?? 0;
                            }

                            break;
                        case AuditStepType.Recertification:
                            totalAuditDays += adcSite.Recertification ?? 0;
                            break;
                        case AuditStepType.Surveillance1:
                        case AuditStepType.Surveillance2:
                        case AuditStepType.Surveillance3:
                        case AuditStepType.Surveillance4:
                        case AuditStepType.Surveillance5:
                            totalAuditDays += adcSite.Surveillance ?? 0;
                            break;
                    }
                } // Por cada step, sumar el total de días de ese step

                // TODO: Ahora la suma de stage 1 y stage 2 debe ser al menos 2 días, ver
                // como validar eso o desde el frontend.
                //if (proposalAudit.AuditStep == AuditStepType.Stage2 && totalAuditDays < 2)
                //    totalAuditDays = 2; // Mínimo 2 días para Stage 2

                if (proposalAudit.AuditStep != AuditStepType.PreAudit &&
                    proposalAudit.AuditStep != AuditStepType.Stage1) 
                { 
                    proposalAudit.TotalAuditDays = totalAuditDays;
                }
                proposalAudit.Updated = DateTime.UtcNow;
                proposalAudit.UpdatedUser = proposal.UpdatedUser;

                proposalAuditRepository.Update(proposalAudit);
                hasChanges = true;
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
                OrganizationName = item.Organization?.Name ?? string.Empty,
                //AuditCycleName = item.AuditCycle?.Name ?? string.Empty,
                AppForm = new {
                    MainSiteAddress = "",
                    LegalEntities = "", // array
                    Website = "",
                    Phone = "",
                    ContactName = "",
                    EMail = ""
                },
                ADCs = item.ADCs?
                    .Where(adc => adc.Status == ADCStatusType.Active)
                    .Select(adc => new
                    {
                        //adc.Description,
                        adc.TotalWorkers,
                        TotalInitial = 0,
                        TotalMD11 = 0,
                        TotalSurveillance = 0
                    })
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