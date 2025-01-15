using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class CertificateMapping
    {
        public static IEnumerable<CertificateItemListDto> CertificatesToListDto(IEnumerable<Certificate> items)
        {
            var itemsDto = new List<CertificateItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(CertificateToItemListDto(item));
            }

            return itemsDto;
        } // CertificatesToListDto

        public static CertificateItemListDto CertificateToItemListDto(Certificate item)
        {
            return new CertificateItemListDto
            {
                ID = item.ID,
                StartDate = item.StartDate,
                DueDate = item.DueDate,
                Comments = item.Comments,
                Filename = item.Filename,
                PrevAuditDate = item.PrevAuditDate,
                PrevAuditNote = item.PrevAuditNote,
                NextAuditDate = item.NextAuditDate,
                NextAuditNote = item.NextAuditNote,
                Status = item.Status,
                OrganizationName = item.Organization != null
                    ? item.Organization.Name
                    : string.Empty,
                StandardName = item.Standard != null
                    ? item.Standard.Name
                    : string.Empty,
                ValidityStatus = item.ValidityStatus
            };
        } // CertificateToItemListDto

        public static CertificateItemDetailDto CertificateToItemDetailDto(Certificate item)
        {
            return new CertificateItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                StandardID = item.StandardID,
                StartDate = item.StartDate,
                DueDate = item.DueDate,
                Comments = item.Comments,
                Filename = item.Filename,
                PrevAuditDate = item.PrevAuditDate,
                PrevAuditNote = item.PrevAuditNote,
                NextAuditDate = item.NextAuditDate,
                NextAuditNote = item.NextAuditNote,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Organization = item.Organization != null
                    ? OrganizationMapping.OrganizationToItemListDto(item.Organization)
                    : null,
                Standard = item.Standard != null
                    ? StandardMapping.StandardToItemListDto(item.Standard)
                    : null,
                ValidityStatus = item.ValidityStatus
            };
        } // CertificateToItemDetailDto

        public static Certificate ItemAddDtoToCertificate(CertificatePostDto itemDto)
        {
            return new Certificate
            {
                OrganizationID = itemDto.OrganizationID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToCertificate

        public static Certificate ItemEditDtoToCertificate(CertificatePutDto itemDto)
        {
            return new Certificate
            {
                ID = itemDto.ID,
                StandardID = itemDto.StandardID,
                StartDate = itemDto.StartDate,
                DueDate = itemDto.DueDate,
                Comments = itemDto.Comments,
                PrevAuditDate = itemDto.PrevAuditDate,
                PrevAuditNote = itemDto.PrevAuditNote,
                NextAuditDate = itemDto.NextAuditDate,
                NextAuditNote = itemDto.NextAuditNote,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToCertificate

        public static Certificate ItemDeleteDtoToCertificate(CertificateDeleteDto itemDto)
        {
            return new Certificate
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToCertificate
    }
}