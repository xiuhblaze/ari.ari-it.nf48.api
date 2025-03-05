using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class AuditAuditorMapping
    {
        public static IEnumerable<AuditAuditorItemListDto> AuditAuditorToListDto(IEnumerable<AuditAuditor> items)
        {
            var itemsDto = new List<AuditAuditorItemListDto>();
            foreach (var item in items)
            {
                itemsDto.Add(AuditAuditorToItemListDto(item));
            }
            return itemsDto;
        } // AuditAuditorToListDto

        public static AuditAuditorItemListDto AuditAuditorToItemListDto(AuditAuditor item)
        {
            return new AuditAuditorItemListDto
            {
                ID = item.ID,
                AuditID = item.AuditID,
                AuditorID = item.AuditorID,
                IsLeader = item.IsLeader,
                IsWitness = item.IsWitness,
                Comments = item.Comments,
                Status = item.Status,
                AuditDescription = item.Audit != null
                    ? item.Audit.Description
                    : string.Empty,
                AuditorName = item.Auditor != null
                    ? Tools.Strings.FullName(item.Auditor.FirstName, item.Auditor.MiddleName, item.Auditor.LastName)
                    : string.Empty,
                StandardsNames = item.AuditStandards?
                    .Where(ads => ads.Status != StatusType.Nothing)
                    .Select(ads => ads.Standard.Name)
            };
        } // AuditAuditorToItemListDto

        public static AuditAuditorItemDetailDto AuditAuditorToItemDetailDto(AuditAuditor item)
        {
            return new AuditAuditorItemDetailDto
            {
                ID = item.ID,
                AuditID = item.AuditID,
                AuditorID = item.AuditorID,
                IsLeader = item.IsLeader,
                IsWitness = item.IsWitness,
                Comments = item.Comments,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Audit = item.Audit != null
                    ? AuditMapping.AuditToItemListDto(item.Audit)
                    : null,
                Auditor = item.Auditor != null
                    ? AuditorMapping.AuditorToItemListDto(item.Auditor)
                    : null,
                AuditStandards = item.AuditStandards?
                    .Where(ads => ads.Status != StatusType.Nothing)
                    .Select(ads => AuditStandardMapping.AuditStandardToItemListDto(ads))
            };
        } // AuditAuditorToItemDetailDto

        public static AuditAuditor ItemAddDtoToAuditAuditor(AuditAuditorPostDto itemDto)
        {
            return new AuditAuditor
            {
                AuditID = itemDto.AuditID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToAuditAuditor

        public static AuditAuditor ItemEditDtoToAuditAuditor(AuditAuditorPutDto itemDto)
        {
            return new AuditAuditor
            {
                ID = itemDto.ID,
                AuditorID = itemDto.AuditorID,
                IsLeader = itemDto.IsLeader,
                IsWitness = itemDto.IsWitness,
                Comments = itemDto.Comments,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToAuditAuditor

        public static AuditAuditor ItemDeleteDtoToAuditAuditor(AuditAuditorDeleteDto itemDto)
        {
            return new AuditAuditor
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToAuditAuditor
    } 
}