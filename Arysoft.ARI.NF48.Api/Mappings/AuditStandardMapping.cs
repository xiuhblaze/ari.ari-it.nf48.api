using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class AuditStandardMapping
    {
        public static async Task<IEnumerable<AuditStandardItemListDto>> AuditStandardToListDto(IEnumerable<AuditStandard> items)
        {
            var itemsDto = new List<AuditStandardItemListDto>();
            foreach (var item in items)
            {
                itemsDto.Add(await AuditStandardToItemListDto(item));
            }
            return itemsDto;
        } // AuditStandarToListDto

        /// <summary>
        /// Para LISTADOS: no accede a la BD. El status de OrganizationStandard se resuelve
        /// en memoria con los OrganizationStandards recibidos (ya cargados por lote).
        /// </summary>
        public static IEnumerable<AuditStandardItemListDto> AuditStandardToListDto(
            IEnumerable<AuditStandard> items,
            IEnumerable<OrganizationStandard> organizationStandards)
        {
            var itemsDto = new List<AuditStandardItemListDto>();
            foreach (var item in items)
            {
                itemsDto.Add(AuditStandardToItemListDto(
                    item,
                    GetOrganizationStandardStatus(item, organizationStandards)));
            }
            return itemsDto;
        } // AuditStandardToListDto (con OrganizationStandards)

        /// <summary>
        /// Ruta individual: consulta a la BD el status de OrganizationStandard. Para
        /// LISTADOS usar la sobrecarga que recibe ese status ya resuelto.
        /// </summary>
        public static async Task<AuditStandardItemListDto> AuditStandardToItemListDto(AuditStandard item)
        {
            var organizationStandardStatus = StatusType.Nothing;

            using (var organizationStandardRepository = new OrganizationStandardRepository())
            {
                var organizationStandard = await organizationStandardRepository
                    .GetByOrganizationAndStandardAsync(
                        item.AuditCycle != null && item.AuditCycle.OrganizationID != Guid.Empty
                            ? item.AuditCycle.OrganizationID
                            : Guid.Empty,
                        item.AuditCycle?.StandardID ?? Guid.Empty
                    );

                if (organizationStandard != null)
                    organizationStandardStatus = organizationStandard.Status;
            }

            return AuditStandardToItemListDto(item, organizationStandardStatus);
        } // AuditStandardToItemListDto

        public static AuditStandardItemListDto AuditStandardToItemListDto(
            AuditStandard item,
            StatusType organizationStandardStatus)
        {
            return new AuditStandardItemListDto
            {
                ID = item.ID,
                AuditCycleID = item.AuditCycleID,
                AuditID = item.AuditID,
                StandardID = item.StandardID,
                Step = item.Step,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                AuditCycleName = item.AuditCycle != null
                    ? item.AuditCycle.Name
                    : string.Empty,
                AuditCycleType = item.AuditCycle != null && item.AuditCycle.CycleType.HasValue
                    ? item.AuditCycle.CycleType.Value
                    : AuditCycleType.Nothing,
                AuditDescription = item.Audit != null
                    ? item.Audit.Description
                    : string.Empty,
                StandardName = item.Standard != null
                    ? item.Standard.Name
                    : string.Empty,
                StandardBase = item.Standard != null
                    ? item.Standard.StandardBase ?? StandardBaseType.Nothing
                    : StandardBaseType.Nothing,
                StandardStatus = item.Standard != null
                    ? item.Standard.Status
                    : StatusType.Nothing,
                OrganizationStandardStatus = organizationStandardStatus,
                AuditorsCount = item.AuditAuditors != null
                    ? item.AuditAuditors.Count
                    : 0,
                DocumentsCount = item.AuditDocuments != null
                    ? item.AuditDocuments.Count
                    : 0
            };
        } // AuditStandardToItemListDto

        public static async Task<AuditStandardItemDetailDto> AuditStandardToItemDetailDto(AuditStandard item)
        {
            return new AuditStandardItemDetailDto
            {
                ID = item.ID,
                AuditCycleID = item.AuditCycleID,
                AuditID = item.AuditID,
                StandardID = item.StandardID,
                Step = item.Step,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                AuditCycle = item.AuditCycle != null
                    ? AuditCycleMapping.AuditCycleToItemListDto(item.AuditCycle)
                    : null,
                Audit = item.Audit != null
                    ? await AuditMapping.AuditToItemListDto(item.Audit)
                    : null,
                Standard = item.Standard != null
                    ? StandardMapping.StandardToItemListDto(item.Standard)
                    : null,
                Auditors = item.AuditAuditors != null
                    ? AuditAuditorMapping.AuditAuditorToListDto(item.AuditAuditors
                        .Where(aa => aa.Status != StatusType.Nothing))
                    : null,
                Documents = item.AuditDocuments != null
                    ? AuditDocumentMapping.AuditDocumentToListDto(item.AuditDocuments
                        .Where(ad => ad.Status != StatusType.Nothing))
                    : null
            };
        } // AuditStandardToItemDetailDto

        public static AuditStandard ItemAddDtoToAuditStandard(AuditStandardPostDto itemDto)
        {
            return new AuditStandard
            {
                AuditID = itemDto.AuditID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToAuditStandard

        public static AuditStandard ItemEditDtoToAuditStandard(AuditStandardPutDto itemDto)
        {
            return new AuditStandard
            {
                ID = itemDto.ID,
                AuditCycleID = itemDto.AuditCycleID,
                // StandardID = itemDto.StandardID, // ahora este debe de ser el mismo que el del AuditCycle, no se va a pedir
                Step = itemDto.Step,
                ExtraInfo = itemDto.ExtraInfo,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToAuditStandard

        public static AuditStandard ItemDeleteDtoToAuditStandard(AuditStandardDeleteDto itemDto)
        {
            return new AuditStandard
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToAuditStandard

        // PRIVATE

        /// <summary>
        /// Mismo criterio que OrganizationStandardRepository.GetByOrganizationAndStandardAsync
        /// (organización y estándar del AuditCycle), pero sobre los OrganizationStandards
        /// que ya están en memoria. Sin AuditCycle o sin coincidencia devuelve Nothing.
        /// </summary>
        private static StatusType GetOrganizationStandardStatus(
            AuditStandard item,
            IEnumerable<OrganizationStandard> organizationStandards)
        {
            if (organizationStandards == null || item.AuditCycle == null)
                return StatusType.Nothing;

            var organizationID = item.AuditCycle.OrganizationID;
            var standardID = item.AuditCycle.StandardID ?? Guid.Empty;

            if (organizationID == Guid.Empty || standardID == Guid.Empty)
                return StatusType.Nothing;

            var organizationStandard = organizationStandards.FirstOrDefault(os =>
                os.OrganizationID == organizationID
                && os.StandardID == standardID);

            return organizationStandard != null
                ? organizationStandard.Status
                : StatusType.Nothing;
        } // GetOrganizationStandardStatus
    }
}