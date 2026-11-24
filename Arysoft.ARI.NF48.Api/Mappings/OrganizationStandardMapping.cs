using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class OrganizationStandardMapping
    {
        public static async Task<IEnumerable<OrganizationStandardItemListDto>> OrganizationStandardToListDto(IEnumerable<OrganizationStandard> items)
        {
            var itemsDto = new List<OrganizationStandardItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(await OrganizationStandardToItemListDto(item));
            }

            return itemsDto;
        } // OrganizationStandardToListDto

        /// <summary>
        /// Para LISTADOS: no accede a la BD. El último ciclo activo (o el último inactivo)
        /// de cada estándar se resuelve en memoria con los AuditCycles recibidos (los de la
        /// organización dueña, ya cargados).
        /// </summary>
        public static IEnumerable<OrganizationStandardItemListDto> OrganizationStandardToListDto(
            IEnumerable<OrganizationStandard> items,
            IEnumerable<AuditCycle> auditCycles)
        {
            var itemsDto = new List<OrganizationStandardItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(OrganizationStandardToItemListDto(
                    item,
                    GetLastOrActiveAuditCycle(item, auditCycles)));
            }

            return itemsDto;
        } // OrganizationStandardToListDto (con AuditCycles)

        /// <summary>
        /// Ruta individual: consulta a la BD el último ciclo activo (o inactivo) del estándar.
        /// Para LISTADOS usar la sobrecarga que recibe los AuditCycles ya cargados.
        /// </summary>
        public static async Task<OrganizationStandardItemListDto> OrganizationStandardToItemListDto(OrganizationStandard item)
        {
            AuditCycle lastOrActiveAuditCycle;

            using (var auditCycleRepository = new Repositories.AuditCycleRepository())
            {
                lastOrActiveAuditCycle = await auditCycleRepository
                    .GetLastOrActiveByStandardAndOrganizationAsync(item.OrganizationID, item.StandardID ?? Guid.Empty);
            }

            return OrganizationStandardToItemListDto(item, lastOrActiveAuditCycle);
        } // OrganizationStandardToItemListDto

        public static OrganizationStandardItemListDto OrganizationStandardToItemListDto(
            OrganizationStandard item,
            AuditCycle lastOrActiveAuditCycle)
        {
            return new OrganizationStandardItemListDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                StandardID = item.StandardID ?? Guid.Empty,
                ExtraInfo = item.ExtraInfo,
                OrganizationName = item.Organization != null
                    ? item.Organization.Name
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
                Status = item.Status,
                // LAST OR ACTIVE AUDIT CYCLE
                AuditCycleName = lastOrActiveAuditCycle?.Name,
                AuditCycleType = lastOrActiveAuditCycle?.CycleType ?? AuditCycleType.Nothing,
                AuditCycleStart = lastOrActiveAuditCycle?.StartDate,
                AuditCycleEnd = lastOrActiveAuditCycle?.EndDate
            };
        } // OrganizationStandardToItemListDto

        public static async Task<OrganizationStandardItemDetailDto> OrganizationStandardToItemDetailDto(OrganizationStandard item)
        {
            return new OrganizationStandardItemDetailDto
            {
                ID = item.ID,
                OrganizationID = item.OrganizationID,
                StandardID = item.StandardID ?? Guid.Empty,
                ExtraInfo = item.ExtraInfo,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Organization = item.Organization != null
                    ? await OrganizationMapping.OrganizationToItemListDto(item.Organization)
                    : null,
                Standard = item.Standard != null
                    ? StandardMapping.StandardToItemListDto(item.Standard)
                    : null
            };
        } // OrganizationStandardToItemDetailDto

        public static OrganizationStandard ItemCreateDtoToOrganizationStandard(OrganizationStandardPostDto itemDto)
        {
            return new OrganizationStandard
            {
                OrganizationID = itemDto.OrganizationID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemCreateDtoToOrganizationStandard

        public static OrganizationStandard ItemEditDtoToOrganizationStandard(OrganizationStandardPutDto itemDto)
        {
            return new OrganizationStandard
            {
                ID = itemDto.ID,
                StandardID = itemDto.StandardID,
                ExtraInfo = itemDto.ExtraInfo,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToOrganizationStandard

        public static OrganizationStandard ItemDeleteDtoToOrganizationStandard(OrganizationStandardDeleteDto itemDto)
        {
            return new OrganizationStandard
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToOrganizationStandard

        // PRIVATE

        /// <summary>
        /// Mismo criterio que AuditCycleRepository.GetLastOrActiveByStandardAndOrganizationAsync
        /// pero en memoria: el ciclo Active con EndDate más reciente y, si no hay ninguno,
        /// el Inactive con EndDate más reciente. Sin estándar o sin ciclos devuelve null.
        /// </summary>
        private static AuditCycle GetLastOrActiveAuditCycle(
            OrganizationStandard item,
            IEnumerable<AuditCycle> auditCycles)
        {
            var standardID = item.StandardID ?? Guid.Empty;

            if (auditCycles == null || standardID == Guid.Empty)
                return null;

            var cycles = auditCycles
                .Where(c => c.OrganizationID == item.OrganizationID && c.StandardID == standardID)
                .ToList();

            return cycles
                    .Where(c => c.Status == StatusType.Active)
                    .OrderByDescending(c => c.EndDate)
                    .FirstOrDefault()
                ?? cycles
                    .Where(c => c.Status == StatusType.Inactive)
                    .OrderByDescending(c => c.EndDate)
                    .FirstOrDefault();
        } // GetLastOrActiveAuditCycle
    }
}