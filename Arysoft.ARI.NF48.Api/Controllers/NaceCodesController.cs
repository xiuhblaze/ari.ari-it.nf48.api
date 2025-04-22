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
    public class NaceCodesController : ApiController
    {
        private readonly NaceCodeService _naceCodeService;

        // CONSTRUCTOR

        public NaceCodesController()
        {
            _naceCodeService = new NaceCodeService();
        }

        // ENDPOINTS

        // GET: api/NaceCodes
        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<NaceCode>>))]
        public IHttpActionResult GetNaceCodes([FromUri]NaceCodeQueryFilters filters)
        {
            var items = _naceCodeService.Gets(filters);
            var itemsDto = NaceCodeMapping.NaceCodeToListDto(items);
            var response = new ApiResponse<IEnumerable<NaceCodeItemListDto>>(itemsDto)
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
        } // GetNaceCodes

        // GET: api/NaceCodes/5
        [ResponseType(typeof(ApiResponse<NaceCode>))]
        public async Task<IHttpActionResult> GetNaceCode(Guid id)
        {
            var item = await _naceCodeService.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = NaceCodeMapping.NaceCodeToItemDetailDto(item);
            var response = new ApiResponse<NaceCodeItemDetailDto>(itemDto);
            
            return Ok(response);
        } // GetNaceCode

        // POST: api/NaceCodes
        [ResponseType(typeof(ApiResponse<NaceCodeItemDetailDto>))]
        public async Task<IHttpActionResult> PostNaceCode(NaceCodePostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var itemToAdd = NaceCodeMapping.ItemAddDtoToNaceCode(itemAddDto);
            var item = await _naceCodeService.AddAsync(itemToAdd);
            var itemDto = NaceCodeMapping.NaceCodeToItemDetailDto(item);
            var response = new ApiResponse<NaceCodeItemDetailDto>(itemDto);

            return Ok(response);
        } // PostNaceCode

        // PUT: api/NaceCodes/5
        [ResponseType(typeof(ApiResponse<NaceCode>))]
        public async Task<IHttpActionResult> PutNaceCode(Guid id, [FromBody] NaceCodePutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var itemToEdit = NaceCodeMapping.ItemEditDtoToNaceCode(itemEditDto);
            var item = await _naceCodeService.UpdateAsync(itemToEdit);
            var itemDto = NaceCodeMapping.NaceCodeToItemDetailDto(item);
            var response = new ApiResponse<NaceCodeItemDetailDto>(itemDto);

            return Ok(response);
        } // PutNaceCode

        // DELETE: api/NaceCodes/5
        [ResponseType(typeof(NaceCode))]
        public async Task<IHttpActionResult> DeleteNaceCode(Guid id, [FromBody] NaceCodeDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var itemToDelete = NaceCodeMapping.ItemDeleteDtoToNaceCode(itemDelDto);
            await _naceCodeService.DeleteAsync(itemToDelete);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteNaceCode
    }
}