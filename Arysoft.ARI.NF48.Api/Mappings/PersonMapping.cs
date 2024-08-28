using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class PersonMapping
    {
        public static IEnumerable<PersonItemListDto> PersonToListDto(IEnumerable<Person> items)
        {
            var itemsDto = new List<PersonItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(PersonToItemListDto(item));
            }

            return itemsDto;
        } // PersonaToListDto

        public static PersonItemListDto PersonToItemListDto(Person item)
        {
            return new PersonItemListDto
            {
                ID = item.ID,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                Phone = item.Phone,
                PhoneAlt = item.PhoneAlt,
                LocationDescription = item.LocationDescription,
                Status = item.Status                
            };
        } // PersonToItemListDto

        public static PersonItemDetailDto PersonToItemDetailDto(Person item)
        {
            return new PersonItemDetailDto
            {
                ID = item.ID,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                Phone = item.Phone,
                PhoneAlt = item.PhoneAlt,
                LocationDescription = item.LocationDescription,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser
            };
        } // PersonToItemDetailDto

        public static Person ItemAddDtoToPerson(PersonPostDto itemDto)
        {
            return new Person
            {
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToPerson

        public static Person ItemEditDtoToPerson(PersonPutDto itemDto)
        {
            return new Person
            {
                ID = itemDto.ID,
                FirstName = itemDto.FirstName,
                LastName = itemDto.LastName,
                Email = itemDto.Email,
                Phone = itemDto.Phone,
                PhoneAlt = itemDto.PhoneAlt,
                LocationDescription = itemDto.LocationDescription,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser,
            };
        } // ItemEditDtoToPerson

        public static Person ItemDeleteDtoToPerson(PersonDeleteDto itemDto)
        {
            return new Person
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToPerson

    }
}