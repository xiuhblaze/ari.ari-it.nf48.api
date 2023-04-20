using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.Mappings
{
    public class ContactMappings
    {
        public static Contact PostToContact(ContactPostDto contactPostDto) 
        {
            var contact = new Contact
            {
                OrganizationID = contactPostDto.OrganizationID,
                UpdatedUser = contactPostDto.UpdatedUser
            };

            return contact;
        }

        public static Contact PutToContact(ContactPutDto contactDto)
        {
            var contact = new Contact
            {
                ContactID = contactDto.ContactID,
                FirstName = contactDto.FirstName,
                LastName = contactDto.LastName,
                Phone = contactDto.Phone,
                PhoneExtensions = contactDto.PhoneExtensions,
                Email = contactDto.Email,
                Position = contactDto.Position,
                Status = contactDto.Status,
                UpdatedUser = contactDto.UpdatedUser,
            };

            return contact;
        }
    }
}