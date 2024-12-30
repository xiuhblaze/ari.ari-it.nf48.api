using Antlr.Runtime;
using Arysoft.ARI.NF48.Api.CustomEntities;
using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Mappings;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.QueryFilters;
using Arysoft.ARI.NF48.Api.Response;
using Arysoft.ARI.NF48.Api.Services;
using Arysoft.ARI.NF48.Api.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class AuditorStandardsController : ApiController
    {
        private readonly AuditorStandardService _service;

        // CONSTRUCTOR

        public AuditorStandardsController()
        {
            _service = new AuditorStandardService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<AuditorStandardItemListDto>>))]
        public IHttpActionResult GetAuditorStandardDocuments([FromUri] AuditorStandardQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = AuditorStandardMapping.AuditorStandardToListDto(items);
            var response = new ApiResponse<IEnumerable<AuditorStandardItemListDto>>(itemsDto) 
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
        } // GetAuditorStandardDocuments

        [ResponseType(typeof(ApiResponse<AuditorStandardItemDetailDto>))]
        public async Task<IHttpActionResult> GetAuditorStandard(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = AuditorStandardMapping.AuditorStandardToItemDetailDto(item);
            var response = new ApiResponse<AuditorStandardItemDetailDto>(itemDto);

            return Ok(response);
        } // GetAuditorStandard

        [HttpPost]
        [ResponseType(typeof(ApiResponse<AuditorStandardItemDetailDto>))]
        public async Task<IHttpActionResult> PostAuditorStandard([FromBody] AuditorStandardPostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = AuditorStandardMapping.ItemAddDtoToAuditorStandard(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = AuditorStandardMapping.AuditorStandardToItemDetailDto(item);
            var response = new ApiResponse<AuditorStandardItemDetailDto>(itemDto);

            return Ok(response);
        } // PostAuditorStandard

        [HttpPut]
        [ResponseType(typeof(ApiResponse<AuditorStandardItemDetailDto>))]
        public async Task<IHttpActionResult> PutAuditorStandard(Guid id, [FromBody] AuditorStandardPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = AuditorStandardMapping.ItemEditDtoToAuditorStandard(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = AuditorStandardMapping.AuditorStandardToItemDetailDto(item);
            var response = new ApiResponse<AuditorStandardItemDetailDto>(itemDto);

            return Ok(response);
        } // PutAuditorStandard

        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteAuditorStandard(Guid id, [FromBody] AuditorStandardDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = AuditorStandardMapping.ItemDeleteDtoToAuditorStandard(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteAuditorStandard 
    }
}
