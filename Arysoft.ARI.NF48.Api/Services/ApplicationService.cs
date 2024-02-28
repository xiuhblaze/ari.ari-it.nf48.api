using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Services
{
    public class ApplicationService
    {
        private readonly ApplicationRepository _applicationRepository;

        // CONSTRUCTOR

        public ApplicationService()
        {
            _applicationRepository = new ApplicationRepository();
        }

        // METHODS

        public PagedList<Application> Gets(ApplicationQueryFilters filters)
        {
            var items = _applicationRepository.Gets();

            // filters

            if (filters.StandardID != null)
            {
                items = items.Where(i => i.StandardID == filters.StandardID);
            }

            if (filters.NaceCodeID != null)
            {
                items = items.Where(i => i.NaceCodeID == filters.NaceCodeID);
            }

            if (filters.RiskLevelID != null)
            {
                items = items.Where(i => i.RiskLevelID == filters.RiskLevelID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            { 
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(i => 
                    i.ProcessScope.ToLower().Contains(filters.Text)
                    || i.Services.ToLower().Contains(filters.Text)
                    || i.LegalRequirements.ToLower().Contains(filters.Text)
                    || i.CriticalComplaintComments.ToLower().Contains(filters.Text)
                    || i.AutomationLevel.ToLower().Contains(filters.Text)
                    || i.DesignResponsibilityJustify.ToLower().Contains(filters.Text)
                    || i.CurrentCertificationBy.ToLower().Contains(filters.Text)
                    || i.CurrentStandards.ToLower().Contains(filters.Text)
                    || i.OutsourcedProcess.ToLower().Contains(filters.Text)
                    || i.AnyConsultancyBy.ToLower().Contains(filters.Text)
                    || (i.Organization != null && i.Organization.Name.ToLower().Contains(filters.Text))
                    || (i.Standard != null && i.Standard.Name.ToLower().Contains(filters.Text))
                );
            }

            if (filters.AuditLanguage.HasValue)
            {
                items = items.Where(i => i.AuditLanguage == filters.AuditLanguage);
            }

            if (filters.Status.HasValue && filters.Status != ApplicationStatusType.Nothing)
            {
                items = items.Where(i => i.Status == filters.Status);
            }
            else
            { 
                if (filters.IncludeDeleted == null) filters.IncludeDeleted = false;
                items = (bool)filters.IncludeDeleted
                    ? items.Where(i => i.Status != ApplicationStatusType.Nothing)
                    : items.Where(i => i.Status != ApplicationStatusType.Nothing 
                        && i.Status != ApplicationStatusType.Deleted);
            }

            // Order

            switch (filters.Order)
            { 
                //TODO: AQUI VOY
            }

            var pagedItems = PagedList<Application>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets 
    }
}