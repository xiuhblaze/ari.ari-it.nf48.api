using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Repositories;
using Arysoft.ARI.NF48.Api.Tools;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class AuditorMapping
    {
        public static IEnumerable<AuditorItemListDto> AuditorToListDto(IEnumerable<Auditor> items)
        {
            var itemsDto = new List<AuditorItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(AuditorToItemListDto(item));
            }

            return itemsDto;
        } // AuditorToListDto

        public static AuditorItemListDto AuditorToItemListDto(Auditor item)
        {
            return new AuditorItemListDto
            {
                ID = item.ID,
                FullName = Strings.FullName(item.FirstName, item.MiddleName, item.LastName),
                Email = item.Email,
                Phone = item.Phone,
                Address = item.Address,
                PhotoFilename = item.PhotoFilename,
                FeePayment = item.FeePayment,
                IsLeadAuditor = item.IsLeadAuditor,
                Status = item.Status,
                ValidityStatus = GetAuditorValidityStatus(item),
                RequiredStatus = GetAuditorRequiredStatus(item)
            };
        } // AuditorToItemListDto

        public static AuditorItemDetailDto AuditorToItemDetailDto(Auditor item)
        {
            return new AuditorItemDetailDto
            {
                ID = item.ID,
                FirstName = item.FirstName,
                MiddleName = item.MiddleName,
                LastName = item.LastName,
                Email = item.Email,
                Phone = item.Phone,
                Address = item.Address,
                PhotoFilename = item.PhotoFilename,
                FeePayment = item.FeePayment,
                IsLeadAuditor = item.IsLeadAuditor,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                ValidityStatus = GetAuditorValidityStatus(item),
                RequiredStatus = GetAuditorRequiredStatus(item),
                Documents = item.Documents != null
                    ? AuditorDocumentMapping.AuditorDocumentToListDto(item.Documents)
                    : null,
            };
        } // AuditorToItemDetailDto

        public static Auditor ItemAddDtoToAuditor(AuditorPostDto itemDto)
        {
            return new Auditor
            {
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToAuditor

        public static Auditor ItemEditDtoToAuditor(AuditorPutDto itemDto)
        {
            return new Auditor
            {
                ID = itemDto.ID,
                FirstName = itemDto.FirstName,
                MiddleName = itemDto.MiddleName,
                LastName = itemDto.LastName,
                Email = itemDto.Email,
                Phone = itemDto.Phone,
                Address = itemDto.Address,
                FeePayment = itemDto.FeePayment,
                IsLeadAuditor = itemDto.IsLeadAuditor,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToAuditor

        public static Auditor ItemDeleteDtoToAuditor(AuditorDeleteDto itemDto)
        {
            return new Auditor
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToAuditor

        // PRIVATE

        private static AuditorDocumentValidityType GetAuditorValidityStatus(Auditor item)
        {
            var documents = AuditorDocumentMapping.AuditorDocumentToListDto(item.Documents);
            AuditorDocumentValidityType validityStatus = AuditorDocumentValidityType.Nothing;

            if (documents != null && documents.Count() > 0)
            {
                validityStatus = AuditorDocumentValidityType.Success;

                foreach (var document in documents)
                {
                    if (document.ValidityStatus == AuditorDocumentValidityType.Danger)
                    {
                        validityStatus = AuditorDocumentValidityType.Danger;
                        break;
                    }
                }

                if (validityStatus != AuditorDocumentValidityType.Danger)
                {
                    foreach (var document in documents)
                    {
                        if (document.ValidityStatus == AuditorDocumentValidityType.Warning)
                        {
                            validityStatus = AuditorDocumentValidityType.Warning;
                            break;
                        }
                    }
                }
            }

            return validityStatus;
        } // GetAuditorValidityStatus

        private static AuditorDocumentRequiredType GetAuditorRequiredStatus(Auditor item) 
        {
            var catAuditorDocumentRepository = new CatAuditorDocumentRepository();

            AuditorDocumentRequiredType requiredStatus = AuditorDocumentRequiredType.Nothing;
            var catAuditorDocuments = catAuditorDocumentRepository.Gets()
                .Where(m => (bool)m.IsRequired && m.Status == StatusType.Active)
                .ToList();

            if (catAuditorDocuments != null)
            {
                requiredStatus = AuditorDocumentRequiredType.Success;

                foreach (var catAuditorDocument in catAuditorDocuments)
                {
                    if (!item.Documents
                        .Where(d =>
                            d.CatAuditorDocumentID == catAuditorDocument.ID
                            && d.Status == StatusType.Active)
                        .Any())
                    { 
                        requiredStatus = AuditorDocumentRequiredType.Danger;
                        break;
                    }
                }
            }

            return requiredStatus;
        } // GetAuditorRequiredStatus
    }
}