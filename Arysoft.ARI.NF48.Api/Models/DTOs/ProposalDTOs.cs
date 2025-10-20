using Arysoft.ARI.NF48.Api.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Models.DTOs
{
    public class ProposalItemListDto // HACK: Ver si se necesitan todos los campos realmente
    {
        public Guid ID { get; set; }

        public Guid AuditCycleID { get; set; }

        public Guid AppFormID { get; set; }

        public Guid ADCID { get; set; }

        public Guid? MD5ID { get; set; }

        public string ActivitiesScope { get; set; }

        public int? TotalEmployees { get; set; }

        public string Justification { get; set; }

        public string SignerName { get; set; }

        public string SignerPosition { get; set; }

        public string SignedFilename { get; set; }

        public CurrencyCodeType? CurrencyCode { get; set; }

        // INTERNAL

        public string CreatedBy { get; set; }

        public DateTime? ReviewDate { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public DateTime? SignRequestDate { get; set; }

        public string HistoricalDataJSON { get; set; }

        public ProposalStatusType Status { get; set; }

        // RELATIONS

        public string OrganizationName { get; set; }

        public string AuditCycleName { get; set; }

        public string AppFormStandardName { get; set; }

        public string MD5Range { get; set; }

        public int NotesCount { get; set; }

        // NOT MAPPED

        public List<ProposalAlertType> Alerts { get; set; }

    } // ProposalItemListDto

    public class ProposalItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid AuditCycleID { get; set; }

        public Guid AppFormID { get; set; }

        public Guid ADCID { get; set; }

        public Guid? MD5ID { get; set; }

        public string ActivitiesScope { get; set; } // Para detectar si se modifica el alcance

        public int? TotalEmployees { get; set; } // Para detectar si se modifica el número de empleados

        public string Justification { get; set; }

        public string SignerName { get; set; }

        public string SignerPosition { get; set; }

        public DateTime? SendToSignDate { get; set; }

        public string SignedFilename { get; set; }

        public CurrencyCodeType? CurrencyCode { get; set; }

        // INTERNAL

        public string CreatedBy { get; set; }

        public DateTime? ReviewDate { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public DateTime? SignRequestDate { get; set; }

        public string HistoricalDataJSON { get; set; }

        public ProposalStatusType Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string UpdatedUser { get; set; }

        // RELATIONS

        public AuditCycleItemListDto AuditCycle { get; set; }

        public AppFormItemListDto AppForm { get; set; }

        public ADCItemListDto ADC { get; set; }

        public MD5ItemListDto MD5 { get; set; }

        // ProposalAudits goes here

        public ICollection<NoteItemDto> Notes { get; set; }

        // NOT MAPPED

        public List<ProposalAlertType> Alerts { get; set; }

    } // ProposalItemDetailDto

    public class ProposalCreateDto
    {
        [Required(ErrorMessage = "The Audit Cycle ID is required")]
        public Guid? AuditCycleID { get; set; }

        [Required(ErrorMessage = "The User that creates is required")]
        [StringLength(50, ErrorMessage = "The User name must be less than 50 characters")]
        public string UpdatedUser { get; set; }
    } // ProposalCreateDto

    public class ProposalUpdateDto
    {
        [Required(ErrorMessage = "The ID is required to update")]
        public Guid? ID { get; set; }

        [Required(ErrorMessage = "The Justification is required")]
        public string Justification { get; set; }

        [StringLength(150, ErrorMessage = "The signer name must be less than 150 characters")]
        public string SignerName { get; set; }

        [StringLength(100, ErrorMessage = "The signer position must be less than 100 characters")]
        public string SignerPosition { get; set; }

        [Required(ErrorMessage = "The currency code is required")]
        public CurrencyCodeType? CurrencyCode { get; set; }

        [Required(ErrorMessage = "The Status value is required")]
        public ProposalStatusType Status { get; set; }

        [Required(ErrorMessage = "The User that updates is required")]
        [StringLength(50, ErrorMessage = "The User name must be less than 50 characters")]
        public string UpdatedUser { get; set; }
    } // ProposalUpdateDto

    public class ProposalDeleteDto
    {
        [Required(ErrorMessage = "The ID is required to delete")]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The User that deletes is required")]
        [StringLength(50, ErrorMessage = "The User name must be less than 50 characters")]
        public string UpdatedUser { get; set; }
    } // ProposalDeleteDto
}