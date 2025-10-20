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

            // item = await AddProposalAuditsToNewProposalAsync(item);

            // 1. Agregar los sitios del proposal
            // 2. Agregar los datos del appform y adc necesarios -YA

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

        // PRIVATE FUNCTIONS

        // Create 

        private async Task ValidateNewItemAsync(Proposal item)
        {
            // Validations
                        
            // - Validar que el auditcycle y la organization esten activos
            if (await _repository.HasValidParentsForCreateAsync(item))
                throw new BusinessException("The Organization, Audit cycle or App form records are not valid.");

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
                    case ProposalStatusType.Review:
                        if (foundItem.Status != ProposalStatusType.New
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
                        
                        break;

                    case ProposalStatusType.Active:
                        // Validar que otra propuesa del mismo ciclo no esté activa
                        if (foundItem.Status != ProposalStatusType.Sended)
                            throw new BusinessException("The proposal can only be active if the client signed it.");
                        break;

                    case ProposalStatusType.Deleted:
                        throw new BusinessException("The proposal can't be deleted. Use the Delete function");
                }
            }

            if (!await _repository.HasValidParentsForUpdateAsync(item))
                throw new BusinessException("The Organization, Audit cycle, ADC or App form records are not valid.");

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
                        if(string.IsNullOrEmpty(item.SignerName)
                            || string.IsNullOrEmpty(item.SignerPosition))
                            throw new BusinessException("The signatory's name and position are required before submitting the proposal.");
                        foundItem.SignRequestDate = DateTime.UtcNow;
                        break;
                    case ProposalStatusType.Active:
                        //foundItem.ActiveDate = DateTime.UtcNow;                        
                        break;
                    case ProposalStatusType.Inactive:
                        foundItem.HistoricalDataJSON = GetHistoricalDataJSON(foundItem);
                        break;
                }
            }

            // foundItem.MD5ID = item.MD5ID;
            //foundItem.ActivitiesScope = item.ActivitiesScope;
            //foundItem.TotalEmployees = item.TotalEmployees; //HACK: Considerar si es calculado o se recibe
            foundItem.Justification = item.Justification;   //HACK: Considerar si es generado o se recibe, creo que lo va a generar el front end para que se acepte visualmente
            foundItem.SignerName = item.SignerName;
            foundItem.SignerPosition = item.SignerPosition;
            foundItem.SignedFilename = item.SignedFilename;
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

        //private async Task<Proposal> AddProposalAuditsToNewProposalAsync(Proposal proposal)
        //{
        //    var proposalAuditRepository = new ProposalAuditRepository();
        //    var adcRepository = new ADCRepository();

        //    var adc = await adcRepository.GetAsync(proposal.ADCID)
        //        ?? throw new BusinessException("ADC record not found");

        //    var oneADCSite = adc.ADCSites
        //        .Where(adcs => adcs.Status == StatusType.Active)
        //        .FirstOrDefault()
        //        ?? throw new BusinessException("The ADC has no active sites.");

        //    foreach (var adcSiteAudit in oneADCSite.ADCSiteAudits
        //        .Where(asa => asa.Status == StatusType.Active))
        //    {
        //        decimal totalDays = 0;

        //        foreach (var adcSite in adc.ADCSites
        //            .Where(adcs => adcs.Status == StatusType.Active))
        //        {
        //            var currAsa = adcSite.ADCSiteAudits
        //                .Where(asa => asa.Status == StatusType.Active
        //                    && (asa.Value.HasValue && asa.Value.Value)
        //                    && asa.AuditStep == adcSiteAudit.AuditStep)
        //                .FirstOrDefault();

        //            // Si es multisitio, tomar el valor de MD11

        //            switch (currAsa.AuditStep)
        //            {
        //                case AuditStepType.Stage1:
        //                case AuditStepType.Stage2:
        //                    totalDays += adcSite.Total ?? 0;
        //                    break;
        //                case AuditStepType.Surveillance1:
        //                case AuditStepType.Surveillance2:
        //                case AuditStepType.Surveillance3:
        //                case AuditStepType.Surveillance4:
        //                case AuditStepType.Surveillance5:
        //                    totalDays += adcSite.Surveillance ?? 0;
        //                    break;
        //                case AuditStepType.Recertification:
        //                    //TODO: Calcular RR - esto se va a mover al frontend
        //                    totalDays += adcSite.Total.HasValue 
        //                        ? adcSite.Total.Value * 0.67m
        //                        : 0;
        //                    break;
        //            }
        //        }

        //        var proposalAudit = new ProposalAudit
        //        {
        //            ID = Guid.NewGuid(),
        //            ProposalID = proposal.ID,
        //            AuditStep = adcSiteAudit.AuditStep,
        //            TotalAuditDays = totalDays,
        //            Status = StatusType.Active,
        //            Created = DateTime.UtcNow,
        //            Updated = DateTime.UtcNow,
        //            UpdatedUser = proposal.UpdatedUser,
        //        };
        //    }








        //} // AddProposalAuditsToNewProposalAsync

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