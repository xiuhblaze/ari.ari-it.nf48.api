using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Mappings;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;
using Arysoft.ARI.NF48.Api.Services;
using Arysoft.ARI.NF48.Api.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class SitesController : ApiController
    {   
        private readonly SiteService _siteService;

        // CONSTRUCTOR

        public SitesController()
        {
            _siteService = new SiteService();
        }

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<SiteItemListDto>>))]
        public IHttpActionResult GetSites([FromUri] SiteQueryFilters filters)
        {
            var items = _siteService.Gets(filters);
            var itemsDto = SiteMapping.SiteToListDto(items);
            var response = new ApiResponse<IEnumerable<SiteItemListDto>>(itemsDto)
            {
                Meta = new Metadata
                {
                    TotalCount = items.TotalCount,
                    PageSize = items.PageSize,
                    CurrentPage = items.CurrentPage,
                    TotalPages = items.TotalPages,
                    HasPreviousPage = items.HasPreviousPage,
                    HasNextPage = items.HasNextPage
                }
            };
            
            return Ok(response);
        } // GetSites

        [ResponseType(typeof(ApiResponse<SiteItemDetailDto>))]
        public async Task<IHttpActionResult> GetSite(Guid id)
        {
            var item = await _siteService.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = SiteMapping.SiteToItemDetailDto(item);
            var response = new ApiResponse<SiteItemDetailDto>(itemDto);

            return Ok(response);
        } // GetShift

        // POST: api/Site
        [ResponseType(typeof(ApiResponse<SiteItemDetailDto>))]
        public async Task<IHttpActionResult> PostSite([FromBody] SitePostDto siteDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var itemToAdd = SiteMapping.ItemAddDtoToSite(siteDto);
            var item = await _siteService.AddAsync(itemToAdd);
            var itemDto = SiteMapping.SiteToItemDetailDto(item);
            var response = new ApiResponse<SiteItemDetailDto>(itemDto);

            return Ok(response);
        } // PostSite

        // PUT: api/Site/5
        [ResponseType(typeof(ApiResponse<SiteItemDetailDto>))]
        public async Task<IHttpActionResult> PutSite(Guid id, [FromBody] SitePutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var itemToEdit = SiteMapping.ItemEditDtoToSite(itemEditDto);
            var item = await _siteService.UpdateAsync(itemToEdit);
            var itemDto = SiteMapping.SiteToItemDetailDto(item);
            var response = new ApiResponse<SiteItemDetailDto>(itemDto);
                        
            return Ok(response);
        } // PutSite

        // DELETE: api/Site/5
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteSite(Guid id, [FromBody] SiteDeleteDto itemDeleteDto)
        {
            if (!ModelState.IsValid) throw new BusinessException(Strings.GetModelStateErrors(ModelState));
            if (id != itemDeleteDto.ID) throw new BusinessException("ID mismatch");

            var item = SiteMapping.ItemDeleteDtoToSite(itemDeleteDto);
            await _siteService.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteSite

        // PRIVATE

        //private async Task DeleteTmpByUserAsync(string username)
        //{
        //    var items = await db.Sites
        //        .Where(o =>
        //            o.UpdatedUser == username
        //            && o.Status == StatusType.Nothing)
        //        .ToListAsync();

        //    foreach (var item in items)
        //    {
        //        db.Entry(item).State = EntityState.Deleted;
        //    }
        //}
    }
}
