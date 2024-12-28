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
    public class AuditorStandardService
    {
        private readonly BaseRepository<AuditorStandard> _repository;

        // CONSTRUCTOR

        public AuditorStandardService()
        {
            _repository = new BaseRepository<AuditorStandard>();
        }

        // METHODS

        public PagedList<AuditorStandard> Gets(AuditorStandardQueryFilters filters)
        {
            var items = _repository.Gets();

            // Filters

            if (filters.AuditorID != null && filters.AuditorID != Guid.Empty)
            {
                items = items.Where(e => e.AuditorID == filters.AuditorID);
            }

            if (filters.StandardID != null && filters.StandardID != Guid.Empty)
            {
                items = items.Where(e => e.StandardID == filters.StandardID);
            }

            if (!string.IsNullOrEmpty(filters.Text))
            {
                filters.Text = filters.Text.Trim().ToLower();
                items = items.Where(e =>
                    e.Comments != null && e.Comments.ToLower().Contains(filters.Text)
                );
            }

            if (filters.Status != null && filters.Status != StatusType.Nothing)
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

            // Order

            switch (filters.Order)
            {
                case AuditorStandardOrderType.Auditor:
                    items = items.OrderBy(e => e.Auditor.FirstName)
                        .ThenBy(e => e.Auditor.MiddleName); 
                    break;
                case AuditorStandardOrderType.Standard:
                    items = items.OrderBy(e => e.Standard.Name);
                    break;
                case AuditorStandardOrderType.Updated:
                    items = items.OrderBy(e => e.Updated);
                    break;
                case AuditorStandardOrderType.AuditorDesc:
                    items = items.OrderByDescending(e => e.Auditor.FirstName)
                        .ThenByDescending(e => e.Auditor.MiddleName);
                    break;
                case AuditorStandardOrderType.StandardDesc:
                    items = items.OrderByDescending(e => e.Standard.Name);
                    break;
                case AuditorStandardOrderType.UpdatedDesc:
                    items = items.OrderByDescending(e => e.Updated);
                    break;
                default:
                    items = items.OrderBy(e => e.Standard.Name);
                    break;
            }

            var pagedItems = PagedList<AuditorStandard>
                .Create(items, filters.PageNumber, filters.PageSize);

            return pagedItems;
        } // Gets

    }
}