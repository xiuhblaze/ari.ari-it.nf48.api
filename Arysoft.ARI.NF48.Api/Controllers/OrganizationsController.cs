using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class OrganizationsController : ApiController
    {
        private AriContext db = new AriContext();

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<Organization>>))]
        public IHttpActionResult GetOrganizations([FromUri] OrganizationQueryFilters filters)
        { 
            var items = db.Organizations.AsEnumerable();

            // Filters

            if (!string.IsNullOrEmpty(filters.Text))
            { 
                filters.Text = filters.Text.ToLower().Trim();
                items = items.Where(e => 
                    e.Name.ToLower().Contains(filters.Text)
                    || e.LegalEntity.ToLower().Contains(filters.Text)
                    || e.Website.ToLower().Contains(filters.Text)
                    || e.Phone.ToLower().Contains(filters.Text)
                );
            }

            if (filters.Status != null && filters.Status != OrganizationStatusType.Nothing)
            {
                items = items.Where(e => e.Status == filters.Status);
            }
            else
            {
                items = items.Where(e => e.Status != OrganizationStatusType.Nothing);
            }

            // Order

            switch (filters.Order)
            {
                case OrganizationOrderType.Name:
                    items = items.OrderBy(e => e.Name);
                    break;
                case OrganizationOrderType.LegalEntity:
                    items = items.OrderBy(e => e.LegalEntity);
                    break;
                case OrganizationOrderType.Status:
                    items = items.OrderBy(e => e.Status)
                        .ThenBy(e => e.Name);
                    break;
                case OrganizationOrderType.NameDesc:
                    items = items.OrderByDescending(e => e.Name);
                    break;
                case OrganizationOrderType.LegalEntityDesc:
                    items = items.OrderByDescending(e => e.LegalEntity);
                    break;
                case OrganizationOrderType.StatusDesc:
                    items = items.OrderByDescending(e => e.Status)
                        .ThenByDescending(e => e.Name);
                    break;
                default:
                    items = items.OrderBy(e => e.Name);
                    break;
            }

            return Ok(items);
        }
    }
}
