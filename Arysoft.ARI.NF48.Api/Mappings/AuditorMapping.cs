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
                ValidityStatus = item.ValidityStatus ?? AuditorDocumentValidityType.Nothing, // GetAuditorValidityStatus(item),
                RequiredStatus = item.RequiredStatus ?? AuditorDocumentRequiredType.Nothing, // GetAuditorRequiredStatus(item),
                DocumentsCount = item.Documents != null
                    ? item.Documents.Count
                    : 0,
                Standards = item.AuditorStandards != null
                    ? AuditorStandardMapping.AuditorStandardToListDto(item.AuditorStandards
                        .Where(a => a.Status != StatusType.Nothing))
                    : null
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
                ValidityStatus = item.ValidityStatus ?? AuditorDocumentValidityType.Nothing, // GetAuditorValidityStatus(item),
                RequiredStatus = item.RequiredStatus ?? AuditorDocumentRequiredType.Nothing, // GetAuditorRequiredStatus(item),
                Documents = item.Documents != null
                    ? AuditorDocumentMapping.AuditorDocumentToListDto(item.Documents
                        .Where(d => d.Status != StatusType.Nothing)
                        .OrderBy(d => d.CatAuditorDocument.DocumentType)
                        .ThenBy(d => d.CatAuditorDocument.Order))
                    : null,
                Standards = item.AuditorStandards != null
                    ? AuditorStandardMapping.AuditorStandardToListDto(item.AuditorStandards
                        .Where(a => a.Status != StatusType.Nothing))
                    : null
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

        //private static AuditorDocumentValidityType GetAuditorValidityStatus(Auditor item)
        //{
        //    AuditorDocumentValidityType validityStatus = AuditorDocumentValidityType.Nothing;

        //    if (item.Documents != null)
        //    { 
        //        var documents = AuditorDocumentMapping.AuditorDocumentToListDto(item.Documents);
                
        //        if (documents != null && documents.Count() > 0)
        //        {
        //            validityStatus = AuditorDocumentValidityType.Success;

        //            foreach (var document in documents)
        //            {
        //                if (document.ValidityStatus == AuditorDocumentValidityType.Danger && document.Status == StatusType.Active)
        //                {
        //                    validityStatus = AuditorDocumentValidityType.Danger;
        //                    break;
        //                }
        //            }

        //            if (validityStatus != AuditorDocumentValidityType.Danger)
        //            {
        //                foreach (var document in documents)
        //                {
        //                    if (document.ValidityStatus == AuditorDocumentValidityType.Warning && document.Status == StatusType.Active)
        //                    {
        //                        validityStatus = AuditorDocumentValidityType.Warning;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return validityStatus;
        //} // GetAuditorValidityStatus

        //private static AuditorDocumentRequiredType GetAuditorRequiredStatus(Auditor item) 
        //{
        //    var catAuditorDocumentRepository = new CatAuditorDocumentRepository();
        //    var requiredStatus = AuditorDocumentRequiredType.Success;

        //    var catAuditorDocuments = catAuditorDocumentRepository.Gets()
        //        .Where(m => m.Status == StatusType.Active && m.IsRequired != null && (bool)m.IsRequired)
        //        .ToList();

        //    if (catAuditorDocuments != null)
        //    {
        //        //requiredStatus = AuditorDocumentRequiredType.Success;

        //        // Hiring
        //        foreach (var catAuditorDocument in catAuditorDocuments
        //            .Where(cad => cad.StandardID == null || cad.StandardID == System.Guid.Empty))
        //        {
        //            if (item.Documents == null || !item.Documents
        //                .Where(d =>
        //                    d.CatAuditorDocumentID == catAuditorDocument.ID
        //                    && d.Status == StatusType.Active)
        //                .Any())
        //            { 
        //                requiredStatus = AuditorDocumentRequiredType.Danger;
        //                break;
        //            }
        //        }

        //        // Revisar por cada standard asociado al usuario
        //        if (item.AuditorStandards != null && requiredStatus != AuditorDocumentRequiredType.Danger)
        //        {
        //            foreach (var auditorStandard in item.AuditorStandards
        //                .Where(aus => aus.Status == StatusType.Active))
        //            {
        //                foreach (var catAuditorDocument in catAuditorDocuments
        //                    .Where(cad => cad.StandardID == auditorStandard.StandardID))
        //                {
        //                    if (item.Documents == null || !item.Documents
        //                        .Where(d =>
        //                            d.CatAuditorDocumentID == catAuditorDocument.ID
        //                            && d.Status == StatusType.Active)
        //                        .Any())
        //                            {
        //                                requiredStatus = AuditorDocumentRequiredType.Danger;
        //                                break;
        //                            }
        //                }

        //                if (requiredStatus == AuditorDocumentRequiredType.Danger) break;
        //            }
        //        }
        //    }

        //    return requiredStatus;
        //} // GetAuditorRequiredStatus
    }
}