using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class CertificateService
    {
        private readonly CertificateRepository _repository;

        // CONSTRUCTOR

        public CertificateService()
        {
            _repository = new CertificateRepository();
        }

        // METHODS

        public PagedList<Certificate> Gets(CertificateQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.OrganizationID != null)
            {
                items = items.Where(e => e.OrganizationID == filters.OrganizationID);
            }

            if (filters.StandardID != null)
            {
                items = items.Where(e => e.StandardID == filters.StandardID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => 
                    (e.Comments != null && e.Comments.Contains(filters.Text))
                    || (e.PrevAuditNote != null && e.PrevAuditNote.Contains(filters.Text))
                    || (e.NextAuditNote != null && e.NextAuditNote.Contains(filters.Text))
                    );
            }

            if (filters.StartDate != null 
                && filters.DateType != null 
                && filters.DateType != CertificateFilterDateType.Nothing)
            {
                switch (filters.DateType)
                { 
                    case CertificateFilterDateType.StartDate:
                        items = items.Where(e => e.StartDate >= filters.StartDate);
                        break;
                    case CertificateFilterDateType.DueDate:
                        items = items.Where(e => e.DueDate >= filters.StartDate);
                        break;
                    case CertificateFilterDateType.PrevAuditDate:
                        items = items.Where(e => e.PrevAuditDate >= filters.StartDate);
                        break;
                    case CertificateFilterDateType.NextAuditDate:
                        items = items.Where(e => e.NextAuditDate >= filters.StartDate);
                        break;
                }
            }

            if (filters.EndDate != null
                && filters.DateType != null
                && filters.DateType != CertificateFilterDateType.Nothing)
            {
                switch (filters.DateType)
                {
                    case CertificateFilterDateType.StartDate:
                        items = items.Where(e => e.StartDate <= filters.EndDate);
                        break;
                    case CertificateFilterDateType.DueDate:
                        items = items.Where(e => e.DueDate <= filters.EndDate);
                        break;
                    case CertificateFilterDateType.PrevAuditDate:
                        items = items.Where(e => e.PrevAuditDate <= filters.EndDate);
                        break;
                    case CertificateFilterDateType.NextAuditDate:
                        items = items.Where(e => e.NextAuditDate <= filters.EndDate);
                        break;
                }
            }

            if (filters.Status != null && filters.Status != CertificateStatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(e => e.Status != CertificateStatusType.Nothing)
                    : items.Where(e => e.Status != CertificateStatusType.Nothing && e.Status != CertificateStatusType.Deleted);
            }

            // - Generando valores calculados

            foreach (var item in items)
            {
                item.ValidityStatus = GetValidityStatus(item);
                item.AuditPlanValidityStatus = GetAuditPlanValidityStatus(item);
            }

            if (filters.ValidityStatus != null && filters.ValidityStatus != CertificateValidityStatusType.Nothing)
            {
                items = items.Where(e => e.ValidityStatus == filters.ValidityStatus);
            }

            // Order

            switch (filters.Order)
            {
                case CertificateOrderType.Date:
                    items = items.OrderBy(e => e.StartDate);
                    break;
                case CertificateOrderType.Status:
                    items = items.OrderBy(e => e.Status);
                    break;
                case CertificateOrderType.ExpireStatus:
                    items = items.OrderBy(e => e.ValidityStatus);
                    break;
                case CertificateOrderType.DateDesc:
                    items = items.OrderByDescending(e => e.StartDate);
                    break;
                case CertificateOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status);
                    break;
                case CertificateOrderType.ExpireStatusDesc:
                    items = items.OrderByDescending(e => e.ValidityStatus);
                    break;
            } // Switch

            // Paging

            var pagedItems = PagedList<Certificate>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

        public async Task<Certificate> GetAsync(Guid id)
        { 
            var item = await _repository.GetAsync(id);

            item.ValidityStatus = GetValidityStatus(item);
            item.AuditPlanValidityStatus = GetAuditPlanValidityStatus(item);

            return item;
        } // GetAsync

        public async Task<Certificate> AddAsync(Certificate item)
        {
            // Validations

            if (item.OrganizationID == null)
                throw new BusinessException("Must specify a organization");

            // Assigning values

            item.ID = Guid.NewGuid();
            item.Status = CertificateStatusType.Nothing;
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
                throw new BusinessException($"CertificateService.AddAsync: {ex.Message}");
            }

            return item;
        } // AddAsync

        public async Task<Certificate> UpdateAsync(Certificate item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to update was not found");

            // Validations

            if (item.StandardID == null)
                throw new BusinessException("Must specify a standard");

            if (item.PrevAuditDate == null)
                throw new BusinessException("Must specify the previous audit date");

            // - Falta validar cambios de estado y de cual a cual puede cambiar o regresar

            // Assigning values

            // - Solo cuando es nuevo, se guarda el standard
            if (foundItem.Status == CertificateStatusType.Nothing)
            { 
                foundItem.StandardID = item.StandardID;
            }

            if (item.Status == CertificateStatusType.Nothing)
            {
                item.Status = CertificateStatusType.Active;
            }

            // - Si hay alguna NC es necesario la fecha del plan de acción
            if ((item.HasNCsCritical ?? false) 
                || (item.HasNCsMajor ?? false) 
                || (item.HasNCsMinor ?? false))
            {
                if (item.ActionPlanDate == null)
                    throw new BusinessException("Must specify an action plan date");
            }

            if (item.Status == CertificateStatusType.Active 
                && foundItem.Status != CertificateStatusType.Active)
            {
                await _repository.SetInactiveByOrganizationAndStandardAsync(
                    foundItem.OrganizationID, 
                    foundItem.StandardID ?? Guid.Empty);
            }

            foundItem.StartDate = item.StartDate;
            foundItem.DueDate = item.DueDate;
            foundItem.Comments = item.Comments;
            foundItem.Filename = item.Filename;
            foundItem.PrevAuditDate = item.PrevAuditDate;
            foundItem.PrevAuditNote = item.PrevAuditNote;
            foundItem.NextAuditDate = item.NextAuditDate;
            foundItem.NextAuditNote = item.NextAuditNote;
            foundItem.HasNCsMinor = item.HasNCsMinor;
            foundItem.HasNCsMajor = item.HasNCsMajor;
            foundItem.HasNCsCritical = item.HasNCsCritical;
            foundItem.ActionPlanDate = item.ActionPlanDate;
            foundItem.ActionPlanDelivered = item.ActionPlanDelivered;
            foundItem.Status = item.Status;
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
                throw new BusinessException($"CertificateService.UpdateAsync: {ex.Message}");
            }

            foundItem.ValidityStatus = GetValidityStatus(foundItem);
            foundItem.AuditPlanValidityStatus = GetAuditPlanValidityStatus(foundItem);

            return foundItem;
        } // UpdateAsync

        public async Task DeleteAsync(Certificate item)
        {
            var foundItem = await _repository.GetAsync(item.ID)
                ?? throw new BusinessException("The record to delete was not found");

            // Validations


            // Excecute queries

            if (foundItem.Status == CertificateStatusType.Deleted)
            {   
                _repository.Delete(foundItem);
            }
            else
            {
                foundItem.Status = foundItem.Status == CertificateStatusType.Active
                    ? CertificateStatusType.Expired
                    : CertificateStatusType.Deleted;
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
                throw new BusinessException($"Certificate.DeleteAsync: {ex.Message}");
            }
        } // DeleteAsync 

        public static CertificateValidityStatusType GetValidityStatus(Certificate item)
        {
            CertificateValidityStatusType status = CertificateValidityStatusType.Nothing;

            if (item.DueDate == null)
                return CertificateValidityStatusType.Nothing;

            DateTime currentDate = DateTime.Today;
            DateTime warningDate = item.DueDate.Value.AddMonths(-4); // Cuatro meses antes de la fecha de vencimiento

            if (currentDate >= item.DueDate)
                status = CertificateValidityStatusType.Danger;
            else if (currentDate >= warningDate)
                status = CertificateValidityStatusType.Warning;
            else
                status = CertificateValidityStatusType.Success;

            return status;
        } // GetValidityStatus

        public static DefaultValidityStatusType GetAuditPlanValidityStatus(Certificate item) 
        {
            DefaultValidityStatusType status = DefaultValidityStatusType.Nothing;

            // Si no tiene la fecha previa de auditoria, no validar nada
            if (item.PrevAuditDate is null)
                return status;

            // Si ya fué entregado el plan de acción, no validar nada
            if (item.ActionPlanDelivered ?? false) 
                return DefaultValidityStatusType.Success;

            // Si hay NCs validar si hay plan de acción
            if ((item.HasNCsCritical ?? false) 
                || (item.HasNCsMajor ?? false)
                || (item.HasNCsMinor ?? false))
            {
                int days = (item.HasNCsMajor ?? false) || (item.HasNCsCritical ?? false)
                    ? 15
                    : (item.HasNCsMinor ?? false)
                        ? 28
                        : 0;
                DateTime actionPlanDeliveredDate = item.ActionPlanDate ?? 
                    item.PrevAuditDate.Value.AddDays(-days);

                DateTime currentDate = DateTime.Today;
                DateTime warningDate = actionPlanDeliveredDate.AddDays(-10); // item.PrevAuditDate.Value.AddDays(10); // Advertir sobre la entrega del plande acción
                // DateTime dangerDate = item.PrevAuditDate.Value.AddDays(28); // Es la fecha limite para entregar el plan de acción

                if (currentDate >= actionPlanDeliveredDate)
                    status = DefaultValidityStatusType.Danger;
                else if (currentDate >= warningDate)
                    status = DefaultValidityStatusType.Warning;
                //else
                //    status = DefaultValidityStatusType.Success;
            }

            return status;
        } // GetAuditPlanValidityStatus
    }
}