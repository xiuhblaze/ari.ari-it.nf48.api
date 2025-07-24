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

            if (filters.OrganizationID != null && filters.OrganizationID != Guid.Empty)
            { 
                items = items.Where(e => e.AppForm.AuditCycle.OrganizationID == filters.OrganizationID);
            }

            if (filters.AuditCycleID != null && filters.AuditCycleID != Guid.Empty)
            { 
                items = items.Where(e => e.AppForm.AuditCycleID == filters.AuditCycleID);
            }

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

            //TODO: Revisar que los datos no hayan cambiado respecto a:
            // - Los sites del application form
            // - el numero de empleados por site

            return await _repository.GetAsync(id);
        } // GetAsync

        public async Task<ADC> AddAsync(ADC item)
        {
            // Validations

            if (item.AppFormID == null || item.AppFormID == Guid.Empty)            
                throw new BusinessException("The AppForm ID is required.");

            // Validar que el AppForm no tenga un ADC

            if (await _repository.AppFormHasValidADCAsync(item.AppFormID))
                throw new BusinessException("The Application Form already has a valid ADC");
            
            // Set default values
            
            item.ID = Guid.NewGuid();
            item.UserCreates = item.UpdatedUser;
            item.Status = ADCStatusType.New;
            item.Created = DateTime.UtcNow;
            item.Updated = DateTime.UtcNow;

            try
            { 
            // Procesar ADC
                await _repository.DeleteTmpByUserAsync(item.UpdatedUser);
                _repository.Add(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCService.AddAsync: {ex.Message}");
            }

            await ProcesarADCAsync(item);

            try 
            { 
                _repository.UpdateValues(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)

            {
                throw new BusinessException($"ADCService.AddAsync.Update.ProcesarADCAsync: {ex.Message}");
            }

            item = await _repository.GetAsync(item.ID, asNoTracking: true)
                ?? throw new BusinessException("The ADC was not found after creation.");

            RecalcularTotales(item);

            try
            {
                _repository.UpdateValues(item);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)

            {
                throw new BusinessException($"ADCService.AddAsync.Update.RecalcularTotales: {ex.Message}");
            }

            item = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The ADC was not found after and recalculation.");

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
                await ProcesarADCAsync(item);
                //RecalcularTotales(item);
            }

            // - Dependiendo del status, realizar diferentes acciones
            if (foundItem.Status != item.Status)
            {
                // - 'orita no me acuerdo...
                switch (item.Status) // Si el nuevo status es...
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
                        foundItem.UserReview = item.UpdatedUser;
                        break;

                    case ADCStatusType.Active:
                        if (foundItem.Status != ADCStatusType.Review)
                            throw new BusinessException("Only items in Review can be set to Active.");
                        foundItem.UserReview = item.UpdatedUser;
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

            if (item.Status < ADCStatusType.Inactive)
                RecalcularTotales(foundItem);

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(ADC item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations

            if (item.Status == ADCStatusType.Deleted) // Eliminación física
            {
                _repository.Delete(item);
            }
            else // Eliminación lógica
            {
                item.Status = foundItem.Status < ADCStatusType.Cancel
                    ? ADCStatusType.Cancel
                    : ADCStatusType.Deleted;
                item.Updated = DateTime.UtcNow;
                item.UpdatedUser = item.UpdatedUser;

                _repository.Update(item);
            }

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"ADCService.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync

        // PRIVATE

        private async Task ProcesarADCAsync(ADC item)
        {
            var appFormRepository = new AppFormRepository();
            var adcSiteRepository = new ADCSiteRepository();
            var md5Repository = new MD5Repository();

            var appForm = await appFormRepository.GetAsync(item.AppFormID)
                ?? throw new BusinessException("The AppForm was not found.");

            if (appForm.Sites == null || !appForm.Sites.Any())
                throw new BusinessException("The AppForm does not have any Sites.");

            // - Obtener los Sites del AppForm y agregarlos al ADC
            //HACK: Que pasa si se quita algun site del AppForm?
            foreach (var site in appForm.Sites
                .Where(s => s.Status == StatusType.Active))
            {
                // - Obtener los Empleados de cada turno y sumarlos
                var noEmployees = site.Shifts
                    .Where(s => s.Status == StatusType.Active)
                    .Sum(s => s.NoEmployees) ?? 0;
                
                // - Obtener el MD5
                var initialMd5 = await md5Repository.GetDaysAsync(noEmployees);
                var adcSite = item.ADCSites != null
                    ? item.ADCSites.FirstOrDefault(s => s.SiteID == site.ID) ?? new ADCSite()
                    : new ADCSite();

                adcSite.InitialMD5 = initialMd5;
                adcSite.Employees = noEmployees;
                adcSite.Updated = DateTime.UtcNow;
                adcSite.UpdatedUser = item.UpdatedUser;

                if (adcSite.ID == Guid.Empty)
                {
                    adcSite.ID = Guid.NewGuid();
                    adcSite.ADCID = item.ID;
                    adcSite.SiteID = site.ID;
                    adcSite.Created = DateTime.UtcNow;
                    adcSite.Status = StatusType.Active;

                    adcSiteRepository.Add(adcSite);
                }
                else 
                { 
                    adcSiteRepository.Update(adcSite);
                }

                // Agregar los ADCConceptValues si no existen
                await RegisterADCConceptsAsync(adcSite, appForm.StandardID ?? Guid.Empty);
            } // foreach site

            if (item.ADCSites != null && appForm.Sites.Count < item.ADCSites.Count)
            {
                // - Eliminar los Sites que no están en el AppForm
                var sitesToRemove = item.ADCSites
                    .Where(s => !appForm.Sites.Any(a => a.ID == s.SiteID))
                    .ToList();
                foreach (var siteToRemove in sitesToRemove)
                {
                    adcSiteRepository.Delete(siteToRemove);
                }
            }

            await adcSiteRepository.SaveChangesAsync();
        } // ProcesarADC

        private async Task RegisterADCConceptsAsync(ADCSite adcSite, Guid standardID)
        { 
            var adcConceptRepository = new ADCConceptRepository();
            var adcConceptValueRepository = new ADCConceptValueRepository();

            var concepts = adcConceptRepository
                .Gets()
                .Where(c => c.StandardID == standardID
                    && c.Status == StatusType.Active)
                .ToList() ?? throw new BusinessException("No ADC Concepts found for the Standard.");
            var hasChanges = false;

            foreach (var concept in concepts)
            { 
                if (adcSite.ADCConceptValues == null
                    || !adcSite.ADCConceptValues.Any(acv => acv.ADCConceptID == concept.ID))
                {
                    var adcConceptValue = new ADCConceptValue
                    {
                        ID = Guid.NewGuid(),
                        ADCConceptID = concept.ID,
                        ADCSiteID = adcSite.ID,
                        CheckValue = false,
                        Value = 0, // Inicializar en 0 o el valor que corresponda
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        Status = StatusType.Active,
                        UpdatedUser = adcSite.UpdatedUser
                    };
                    
                    adcConceptValueRepository.Add(adcConceptValue);
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                try
                {
                    await adcConceptValueRepository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"ADCService.RegisterADCConceptsAsync: {ex.Message}");
                }
            }
        } // RegisterADCConceptsAsync

        private void RecalcularTotales(ADC item) //! Este va a ser una vez este toda la info guardadad ADC
        {
            // HACK: Buscar los ADCSites de forma manual primero

            if (item.ADCSites != null && item.ADCSites.Any())
            {
                var totalEmployees = 0;
                decimal totalMD11 = 0;
                decimal allSitesTotalInitial = 0;

                foreach (var adcSite in item.ADCSites
                    .Where(adcsite => adcsite.Status == StatusType.Active))
                {
                    var totalInitial = adcSite.InitialMD5 ?? 0;

                    if (adcSite.ADCConceptValues.Any())
                    {
                        foreach (var conceptValue in adcSite.ADCConceptValues
                            .Where(acv => acv.Status == StatusType.Active))
                        {
                            //TODO: Evaluar cada Concept para sacar el Value o algo asi jojojo...
                            // devolver operaciones que afectan a totalInitial
                        }
                    }

                    adcSite.TotalInitial = totalInitial;
                    adcSite.Surveillance = totalInitial / 3; // Por lo pronto, una tercera parte del TotalInitial
                    adcSite.RR = (totalInitial * 2) / 3; // Por lo pronto, dos terceras partes del TotalInitial


                    allSitesTotalInitial += totalInitial;
                    totalEmployees += adcSite.Site.Shifts
                        .Where(s => s.Status == StatusType.Active)
                        .Sum(s => s.NoEmployees) ?? 0;

                    totalMD11 += adcSite.MD11 ?? 0;
                }

                item.TotalEmployees = totalEmployees;
                item.TotalInitial = allSitesTotalInitial;
                item.TotalMD11 = totalMD11;
            }
            else 
            {
                item.TotalEmployees = 0;
                item.TotalInitial = 0;
                item.TotalMD11 = 0;
            }
        } // RecalcularTotales
    }
}