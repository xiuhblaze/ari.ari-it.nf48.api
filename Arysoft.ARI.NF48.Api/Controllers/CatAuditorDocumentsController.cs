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
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class CatAuditorDocumentsController : ApiController
    {
        private readonly CatAuditorDocumentService _service;

        // CONSTRUCTOR

        public CatAuditorDocumentsController()
        {
            _service = new CatAuditorDocumentService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<CatAuditorDocumentItemListDto>>))]
        public IHttpActionResult GetCatAuditorDocuments([FromUri] CatAuditorDocumentQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = CatAuditorDocumentMapping.CatAuditorDocumentToListDto(items);
            var response = new ApiResponse<IEnumerable<CatAuditorDocumentItemListDto>>(itemsDto)
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
        } // GetCatAuditorDocuments

        [HttpGet]
        [ResponseType(typeof(ApiResponse<CatAuditorDocumentItemDetailDto>))]
        public async Task<IHttpActionResult> GetCatAuditorDocument(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = CatAuditorDocumentMapping.CatAuditorDocumentToItemDetailDto(item);
            var reponse = new ApiResponse<CatAuditorDocumentItemDetailDto>(itemDto);

            return Ok(reponse);
        } // GetCatAuditorDocument

        [ResponseType(typeof(ApiResponse<CatAuditorDocumentItemDetailDto>))]
        public async Task<IHttpActionResult> PostCatAuditorDocument([FromBody] CatAuditorDocumentPostDto itemPostDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = CatAuditorDocumentMapping.ItemAddDtoToCatAuditorDocument(itemPostDto);
            item = await _service.AddAsync(item);
            var itemDto = CatAuditorDocumentMapping.CatAuditorDocumentToItemDetailDto(item);
            var response = new ApiResponse<CatAuditorDocumentItemDetailDto>(itemDto);

            return Ok(response);
        } // PostCatAuditorDocument

        [ResponseType(typeof(ApiResponse<CatAuditorDocumentItemDetailDto>))]
        public async Task<IHttpActionResult> PutCatAuditorDocument(Guid id, [FromBody] CatAuditorDocumentPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = CatAuditorDocumentMapping.ItemEditDtoToCatAuditorDocument(itemEditDto);
            item = await _service.UpdateAsync(item);
            var itemDto = CatAuditorDocumentMapping.CatAuditorDocumentToItemDetailDto(item);
            var response = new ApiResponse<CatAuditorDocumentItemDetailDto>(itemDto);

            return Ok(response);
        } // PutCatAuditorDocument

        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteCatAuditorDocument(Guid id, [FromBody] CatAuditorDocumentDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = CatAuditorDocumentMapping.ItemDeleteDtoToCatAuditorDocument(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteCatAuditorDocument
    }
}
