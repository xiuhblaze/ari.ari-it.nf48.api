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
    public class NotesController : ApiController
    {
        private readonly NoteService _service;

        // CONSTRUCTOR

        public NotesController()
        {
            _service = new NoteService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<NoteItemDto>>))]
        public IHttpActionResult GetNotes([FromUri] NoteQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = NoteMapping.NotesToListDto(items);
            var response = new ApiResponse<IEnumerable<NoteItemDto>>(itemsDto)
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
        } // GetNotes

        [ResponseType(typeof(ApiResponse<NoteItemDto>))]
        public async Task<IHttpActionResult> GetNote(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = NoteMapping.NoteToItemDto(item);
            var response = new ApiResponse<NoteItemDto>(itemDto);

            return Ok(response);
        } // GetNote

        [HttpPost]
        [ResponseType(typeof(ApiResponse<NoteItemDto>))]
        public async Task<IHttpActionResult> PostNote([FromBody] NotePostDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = NoteMapping.ItemAddDtoToNote(itemDto);
            item = await _service.AddAsync(item);
            var itemDtoResponse = NoteMapping.NoteToItemDto(item);
            var response = new ApiResponse<NoteItemDto>(itemDtoResponse);

            return Ok(response);
        } // PostNote

        [HttpPut]
        [ResponseType(typeof(ApiResponse<NoteItemDto>))]
        public async Task<IHttpActionResult> PutNote(Guid id, [FromBody] NotePutDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NoteMapping.ItemEditDtoToNote(itemDto);
            item = await _service.UpdateAsync(item);
            var itemDtoResponse = NoteMapping.NoteToItemDto(item);
            var response = new ApiResponse<NoteItemDto>(itemDtoResponse);

            return Ok(response);
        } // PutNote

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteNote(Guid id, [FromBody] NoteDeleteDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.ID)
                throw new BusinessException("ID mismatch");

            var item = NoteMapping.ItemDeleteDtoToNote(itemDto);
            await _service.DeleteAsync(item);            
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteNote
    }
}
