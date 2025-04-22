using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class AuditMapping
    {
        public static IEnumerable<AuditItemListDto> AuditToListDto(IEnumerable<Audit> items)
        { 
            var itemsDto = new List<AuditItemListDto>();
            foreach (var item in items)
            {
                itemsDto.Add(AuditToItemListDto(item));
            }
            return itemsDto;
        } // AuditToListDto

        public static AuditItemListDto AuditToItemListDto(Audit item)
        {
            return new AuditItemListDto
            { 
                ID = item.ID,
                AuditCycleID = item.AuditCycleID,
                Description = item.Description,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                HasWitness = item.HasWitness,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                OrganizationName = item.AuditCycle != null && item.AuditCycle.Organization != null
                    ? item.AuditCycle.Organization.Name
                    : string.Empty,
                AuditCycleName = item.AuditCycle != null
                    ? item.AuditCycle.Name
                    : string.Empty,
                AuditorsCount = item.AuditAuditors != null
                    ? item.AuditAuditors.Where(aa =>
                        aa.Status != StatusType.Nothing
                        && aa.Status != StatusType.Deleted)
                        .Count()
                    : 0,
                DocumentsCount = item.AuditDocuments != null
                    ? item.AuditDocuments.Where(ad =>
                        ad.Status != StatusType.Nothing
                        && ad.Status != StatusType.Deleted)
                        .Count()
                    : 0,
                NotesCount = item.Notes != null
                    ? item.Notes.Where(n =>
                        n.Status != StatusType.Nothing
                        && n.Status != StatusType.Deleted)
                        .Count()
                    : 0,
                Auditors = item.AuditAuditors != null
                    ? AuditAuditorMapping.AuditAuditorToListDto(item.AuditAuditors.Where(aa =>
                        aa.Status != StatusType.Nothing
                        && aa.Status != StatusType.Deleted))
                    : null,
                Standards = item.AuditStandards != null
                    ? AuditStandardMapping.AuditStandardToListDto(item.AuditStandards.Where(asd =>
                        asd.Status != StatusType.Nothing
                        && asd.Status != StatusType.Deleted))
                    : null
            };
        } // AuditToItemListDto

        public static AuditItemDetailDto AuditToItemDetailDto(Audit item)
        {
            return new AuditItemDetailDto
            { 
                ID = item.ID,
                AuditCycleID = item.AuditCycleID,
                Description = item.Description,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                HasWitness = item.HasWitness,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                AuditCycle = item.AuditCycle != null
                    ? AuditCycleMapping.AuditCycleToItemListDto(item.AuditCycle)
                    : null,
                Auditors = item.AuditAuditors != null
                    ? AuditAuditorMapping.AuditAuditorToListDto(item.AuditAuditors.Where(aa =>
                        aa.Status != StatusType.Nothing
                        && aa.Status != StatusType.Deleted))
                    : null,
                Documents = item.AuditDocuments != null
                    ? AuditDocumentMapping.AuditDocumentToListDto(item.AuditDocuments.Where(ad =>
                        ad.Status != StatusType.Nothing
                        && ad.Status != StatusType.Deleted))
                    : null,
                Notes = item.Notes != null
                    ? NoteMapping.NotesToListDto(item.Notes.Where(n =>
                        n.Status != StatusType.Nothing
                        && n.Status != StatusType.Deleted))
                    : null,
                Standards = item.AuditStandards != null
                    ? AuditStandardMapping.AuditStandardToListDto(item.AuditStandards.Where(asd =>
                        asd.Status != StatusType.Nothing
                        && asd.Status != StatusType.Deleted))
                    : null
            };
        } // AuditToItemDetailDto

        public static Audit ItemAddDtoToAudit(AuditPostDto itemDto)
        {
            return new Audit
            { 
                AuditCycleID = itemDto.AuditCycleID,
                UpdatedUser = itemDto.UpdatedUser,
            };
        } // ItemAddDtoToAudit

        public static Audit ItemEditDtoToAudit(AuditPutDto itemDto)
        {
            return new Audit
            {
                ID = itemDto.ID,                
                Description = itemDto.Description,
                StartDate = itemDto.StartDate,
                EndDate = itemDto.EndDate,
                HasWitness = itemDto.HasWitness,
                ExtraInfo = itemDto.ExtraInfo,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser,
            };
        } // ItemEditDtoToAudit

        public static Audit ItemDeleteDtoToAudit(AuditDeleteDto itemDto)
        {
            return new Audit
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser,
            };
        } // ItemDeleteDtoToAudit
    }
}