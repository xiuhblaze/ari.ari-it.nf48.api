using Arysoft.ARI.NF48.Api.Enumerations;
using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class AuditorDocumentMapping
    {
        public static IEnumerable<AuditorDocumentItemListDto> AuditorDocumentToListDto(IEnumerable<AuditorDocument> items)
        {
            var itemsDto = new List<AuditorDocumentItemListDto>();

            foreach (var item in items)
            {
                itemsDto.Add(AuditorDocumentToItemListDto(item));
            }

            return itemsDto;
        } // AuditorDocumentToListDto

        public static AuditorDocumentItemListDto AuditorDocumentToItemListDto(AuditorDocument item)
        {
            string auditorFullName = string.Empty;

            if (item.Auditor != null)
            {
                auditorFullName = item.Auditor.FirstName;
                auditorFullName += string.IsNullOrEmpty(item.Auditor.MiddleName)
                    ? string.Empty
                    : $" {item.Auditor.MiddleName}";
                auditorFullName += string.IsNullOrEmpty(item.Auditor.LastName)
                    ? string.Empty
                    : $" {item.Auditor.LastName}";
            }

            return new AuditorDocumentItemListDto
            {
                ID = item.ID,
                AuditorID = item.AuditorID,
                CatAuditorDocumentID = item.CatAuditorDocumentID,
                Filename = item.Filename,
                StartDate = item.StartDate,
                DueDate = item.DueDate,
                Observations = item.Observations,
                Type = item.Type,
                Status = item.Status,
                ValidityStatus = item.ValidityStatus ?? AuditorDocumentValidityType.Nothing, //  GetValidityStatus(item),
                AuditorFullName = auditorFullName,
                CatDescription = item.CatAuditorDocument != null
                    ? $"{item.CatAuditorDocument.Name ?? ""} {item.CatAuditorDocument.Description ?? ""}".Trim()
                    : string.Empty
            };
        } // AuditorDocumentToItemListDto

        public static AuditorDocumentItemDetailDto AuditorDocumentToItemDetailDto(AuditorDocument item)
        {
            return new AuditorDocumentItemDetailDto
            {
                ID = item.ID,
                AuditorID = item.AuditorID,
                CatAuditorDocumentID = item.CatAuditorDocumentID,
                Filename = item.Filename,
                StartDate = item.StartDate,
                DueDate = item.DueDate,
                Observations = item.Observations,
                Type = item.Type,
                Status = item.Status,
                ValidityStatus = item.ValidityStatus ?? AuditorDocumentValidityType.Nothing, //  GetValidityStatus(item),
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser,
                Auditor = item.Auditor != null
                    ? AuditorMapping.AuditorToItemListDto(item.Auditor)
                    : null,
                CatAuditorDocument = item.CatAuditorDocument != null
                    ? CatAuditorDocumentMapping.CatAuditorDocumentToItemListDto(item.CatAuditorDocument)
                    : null
            };
        } // AuditorDocumentToItemDetailDto

        public static AuditorDocument ItemAddDtoToAuditorDocument(AuditorDocumentPostDto itemDto)
        {
            return new AuditorDocument
            {
                AuditorID = itemDto.AuditorID,
                CatAuditorDocumentID = itemDto.CatAuditorDocumentID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToAuditorDocument

        public static AuditorDocument ItemEditDtoToAuditorDocument(AuditorDocumentPutDto itemDto)
        {
            return new AuditorDocument
            {
                ID = itemDto.ID,
                StartDate = itemDto.StartDate,
                DueDate = itemDto.DueDate,
                Observations = itemDto.Observations,
                Type = itemDto.Type,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToAuditorDocument

        public static AuditorDocument ItemDeleteDtoToAuditorDocument(AuditorDocumentDeleteDto itemDto)
        {
            return new AuditorDocument
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToAuditorDocument

        //public static AuditorDocumentValidityType GetValidityStatus(AuditorDocument document)
        //{
        //    DateTime? dueDate = document.DueDate;
        //    AuditorDocumentValidityType validityStatus = AuditorDocumentValidityType.Nothing;

        //    if (dueDate != null)
        //    {
        //        DateTime currentDate = DateTime.Today;
        //        DateTime? warningDate = GetWarningDate(document);

        //        if (currentDate >= dueDate)
        //        {
        //            validityStatus = AuditorDocumentValidityType.Danger;
        //        }
        //        else if (warningDate != null && currentDate >= warningDate)
        //        {
        //            validityStatus = AuditorDocumentValidityType.Warning;
        //        }
        //        else 
        //        {
        //            validityStatus = AuditorDocumentValidityType.Success;
        //        }
        //    }

        //    return validityStatus;
        //} // GetValidityStatus

        //private static DateTime? GetWarningDate(AuditorDocument document)
        //{
        //    if (document.CatAuditorDocument == null
        //        || document.CatAuditorDocument.WarningEvery == null
        //        || document.DueDate == null)
        //    {
        //        return null;
        //    }

        //    DateTime? warningDate = null;
        //    DateTime dueDate = document.DueDate ?? new DateTime();
        //    int every = document.CatAuditorDocument.WarningEvery ?? 0;

        //    switch (document.CatAuditorDocument.WarningPeriodicity)
        //    {
        //        case CatAuditorDocumentPeriodicityType.Days:
        //            warningDate = dueDate.AddDays(every * -1);
        //            break;
        //        case CatAuditorDocumentPeriodicityType.Months:
        //            warningDate = dueDate.AddMonths(every * -1);
        //            break;
        //        case CatAuditorDocumentPeriodicityType.Years:
        //            warningDate = dueDate.AddYears(every * -1);
        //            break;
        //    }

        //    return warningDate;
        //} // GetWarningDate
    }
}