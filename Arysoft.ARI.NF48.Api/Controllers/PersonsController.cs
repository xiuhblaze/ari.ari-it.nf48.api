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
    public class PersonsController : ApiController
    {
        private readonly PersonService _personService;

        // CONSTRUCTOR

        public PersonsController()
        {
            _personService = new PersonService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<PersonItemListDto>>))]
        public IHttpActionResult GetPersons([FromUri] PersonQueryFilters filters)
        {
            var items = _personService.Gets(filters);
            var itemsDto = PersonMapping.PersonToListDto(items);
            var response = new ApiResponse<IEnumerable<PersonItemListDto>>(itemsDto)
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
        } // GetPersons

        // GET: api/Person/5
        [ResponseType(typeof(ApiResponse<PersonItemDetailDto>))]
        public async Task<IHttpActionResult> GetPerson(Guid id)
        {
            var item = await _personService.GetAsync(id)
                ?? throw new BusinessException("Item not found");
            var itemDto = PersonMapping.PersonToItemDetailDto(item);
            var response = new ApiResponse<PersonItemDetailDto>(itemDto);

            return Ok(response);
        } // GetPerson

        // POST: api/Person
        [ResponseType(typeof(ApiResponse<PersonItemDetailDto>))]
        public async Task<IHttpActionResult> PostPerson([FromBody] PersonPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var itemToAdd = PersonMapping.ItemAddDtoToPerson(itemAddDto);
            var item = await _personService.AddAsync(itemToAdd);
            var itemDto = PersonMapping.PersonToItemDetailDto(item);
            var response = new ApiResponse<PersonItemDetailDto>(itemDto);

            return Ok(response);
        } // PostPerson

        // PUT: api/person/5
        [ResponseType(typeof(ApiResponse<PersonItemDetailDto>))]
        public async Task<IHttpActionResult> PutPerson(Guid id, [FromBody] PersonPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var itemToEdit = PersonMapping.ItemEditDtoToPerson(itemEditDto);
            var item = await _personService.UpdateAsync(itemToEdit);
            var itemDto = PersonMapping.PersonToItemDetailDto(item);
            var response = new ApiResponse<PersonItemDetailDto>(itemDto);

            return Ok(response);
        } // PutPerson

        // DELETE: api/person/5
        public async Task<IHttpActionResult> DeletePerson(Guid id, [FromBody] PersonDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");

            var item = PersonMapping.ItemDeleteDtoToPerson(itemDelDto);
            await _personService.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeletePerson
    }
}
