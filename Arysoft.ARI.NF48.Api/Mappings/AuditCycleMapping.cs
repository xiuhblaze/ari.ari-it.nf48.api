using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class AuditCycleMapping
    {
        public static IEnumerable<AuditCycleItemListDto> AuditCyclesToListDto(IEnumerable<AuditCycle> items)
        {
            var itemsDto = new List<AuditCycleItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(AuditCycleToItemListDto(item));
            }

            return itemsDto;
        } // AuditCycleToListDto

        public static AuditCycleItemListDto AuditCycleToItemListDto(AuditCycle item)
        {
            return new AuditCycleItemListDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                StandardID = item.StandardID,
                Name = item.Name,
                CycleType = item.CycleType,
                InitialStep = item.InitialStep,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                Periodicity = item.Periodicity,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                OrganizationName = item.Organization != null
                    ? item.Organization.Name
                    : string.Empty,
                AuditsCount = item.AuditStandards != null
                    ? item.AuditStandards.Where(asd => 
                        asd.Audit != null
                        && asd.Audit.Status != AuditStatusType.Nothing
                        && asd.Audit.Status != AuditStatusType.Deleted)
                        .Select(asd => asd.AuditID)
                        .Count()
                    : 0,
                //AuditCycleStandards = item.AuditCycleStandards != null
                //    ? AuditCycleStandardMapping.AuditCycleStandardsToListDto(
                //        item.AuditCycleStandards.Where(acs => 
                //            acs.Status != StatusType.Nothing
                //            && acs.Status != StatusType.Deleted))
                //    : null,
                DocumentsCount = item.AuditCycleDocuments != null
                    ? item.AuditCycleDocuments.Where(acd => 
                        acd.Status != StatusType.Nothing
                        && acd.Status != StatusType.Deleted)
                        .Count()
                    : 0
            };
        } // AuditCycleToItemListDto

        public static AuditCycleItemDetailDto AuditCycleToItemDetailDto(AuditCycle item)
        {
            return new AuditCycleItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                StandardID = item.StandardID,
                Name = item.Name,
                CycleType = item.CycleType,
                InitialStep = item.InitialStep,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                Periodicity = item.Periodicity,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Organization = item.Organization != null
                    ? OrganizationMapping.OrganizationToItemListDto(item.Organization)
                    : null,
                AuditStandards = item.AuditStandards != null
                    ? AuditStandardMapping.AuditStandardToListDto(
                        item.AuditStandards.Where(asd => 
                            asd.Status != StatusType.Nothing
                            && asd.Status != StatusType.Deleted))
                    : null,
                //AuditCycleStandards = item.AuditCycleStandards != null
                //    ? AuditCycleStandardMapping.AuditCycleStandardsToListDto(
                //        item.AuditCycleStandards.Where(acs => 
                //            acs.Status != StatusType.Nothing
                //            && acs.Status != StatusType.Deleted))
                //    : null,
                AuditCycleDocuments = item.AuditCycleDocuments != null
                    ? AuditCycleDocumentMapping.AuditCycleDocumentsToListDto(
                        item.AuditCycleDocuments.Where(acd => 
                            acd.Status != StatusType.Nothing 
                            && acd.Status != StatusType.Deleted))
                    : null,
            };
        } // AuditCycleToItemDetailDto

        public static AuditCycle ItemAddDtoToAuditCycle(AuditCyclePostDto itemDto)
        {
            return new AuditCycle
            {
                OrganizationID = itemDto.OrganizationID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToAuditCycle

        public static AuditCycle ItemEditDtoToAuditCycle(AuditCyclePutDto itemDto)
        {
            return new AuditCycle
            {
                ID = itemDto.ID,
                StandardID = itemDto.StandardID,
                Name = itemDto.Name,
                CycleType = itemDto.CycleType,
                InitialStep = itemDto.InitialStep,
                StartDate = itemDto.StartDate,
                EndDate = itemDto.EndDate,
                Periodicity = itemDto.Periodicity,
                ExtraInfo = itemDto.ExtraInfo,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToAuditCycle

        public static AuditCycle ItemDeleteDtoToAuditCycle(AuditCycleDeleteDto itemDto)
        {
            return new AuditCycle
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToAuditCycle
    }
}