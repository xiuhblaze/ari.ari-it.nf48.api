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
    public class CompaniesController : ApiController
    {
        private readonly CompanyService _service;

        // CONSTRUCTOR

        public CompaniesController()
        {
            _service = new CompanyService();
        }

        // END POINTS

        [HttpGet]
        [ResponseType(typeof(ApiResponse<IEnumerable<CompanyItemListDto>>))]
        public IHttpActionResult GetCompanies([FromUri] CompanyQueryFilters filters)
        {
            var items = _service.Gets(filters);
            var itemsDto = CompanyMapping.CompanyToListDto(items);
            var response = new ApiResponse<IEnumerable<CompanyItemListDto>>(itemsDto)
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
        } // GetCompanies

        [ResponseType(typeof(ApiResponse<CompanyItemDetailDto>))]
        public async Task<IHttpActionResult> GetCompany(Guid id)
        {
            var item = await _service.GetAsync(id)
                ?? throw new BusinessException("Item not found");            
            var itemDto = CompanyMapping.CompanyToItemDetailDto(item);
            var response = new ApiResponse<CompanyItemDetailDto>(itemDto);

            return Ok(response);
        } // GetCompany

        [HttpPost]
        [ResponseType(typeof(ApiResponse<CompanyItemDetailDto>))]
        public async Task<IHttpActionResult> PostCompany([FromBody] CompanyPostDto itemAddDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var item = CompanyMapping.ItemAddDtoToCompany(itemAddDto);
            item = await _service.AddAsync(item);
            var itemDto = CompanyMapping.CompanyToItemDetailDto(item);
            var response = new ApiResponse<CompanyItemDetailDto>(itemDto);

            return Ok(response);
        } // PostCompany

        [HttpPut]
        [ResponseType(typeof(ApiResponse<CompanyItemDetailDto>))]
        public async Task<IHttpActionResult> PutCompany(Guid id, [FromBody] CompanyPutDto itemEditDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemEditDto.ID)
                throw new BusinessException("ID mismatch");

            var item = CompanyMapping.ItemEditDtoToCompany(itemEditDto);            
            item = await _service.UpdateAsync(item);
            var itemDto = CompanyMapping.CompanyToItemDetailDto(item);
            var response = new ApiResponse<CompanyItemDetailDto>(itemDto);

            return Ok(response);
        } // PutCompany

        [HttpDelete]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> DeleteCompany(Guid id, [FromBody] CompanyDeleteDto itemDelDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDelDto.ID)
                throw new BusinessException("ID mismatch");
                       
            var item = CompanyMapping.ItemDeleteDtoToCompany(itemDelDto);
            await _service.DeleteAsync(item);
            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // DeleteCompany
    }
}
