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

        public string Justification { get; set; }

        public string SignerName { get; set; }

        public string SignerPosition { get; set; }

        public string SignedFilename { get; set; }

        public CurrencyCodeType? CurrencyCode { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal? TaxRate { get; set; }

        public bool? IncludeTravelExpenses { get; set; }

        public string ExtraInfo { get; set; }

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

        public int ADCCount { get; set; }

        public int ProposalAuditsCount { get; set; }

        public int NotesCount { get; set; }

        public List<string> Standards { get; set; }

        // NOT MAPPED

        public List<ProposalAlertType> Alerts { get; set; }

    } // ProposalItemListDto

    public class ProposalItemDetailDto
    {
        public Guid ID { get; set; }

        public Guid AuditCycleID { get; set; }

        public CycleYearType? CycleYear { get; set; }

        public string Justification { get; set; }

        public string SignerName { get; set; }

        public string SignerPosition { get; set; }

        public DateTime? SendToSignDate { get; set; }

        public string SignedFilename { get; set; }

        public CurrencyCodeType? CurrencyCode { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal? TaxRate { get; set; }

        public bool? IncludeTravelExpenses { get; set; }

        public string ExtraInfo { get; set; }

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

        public ICollection<ADCItemListDto> ADCs { get; set; }

        public ICollection<ProposalAuditItemDto> ProposalAudits { get; set; }

        public ICollection<NoteItemDto> Notes { get; set; }

        // RELATIONS EXTRA FIELDS

        public OrganizationItemProposalDto Organization { get; set; }

        // Tienen que ser los seleccionados en los AppForm's
        public ICollection<SiteItemListDto> Sites { get; set; }

        // Tienen que ser los seleccionados en los AppForm's
        public ICollection<ContactItemListDto> Contacts { get; set; }

        // Tienen que ser de los AppForm's
        public ICollection<string> Scopes { get; set; }

        // Tienen que ser de los ADC's
        public ICollection<int> TotalEmployees { get; set; }

        // Tienen que ser de TODOS los ADC's junto con sus ADCSiteAudits
        public ICollection<ADCSiteItemListDto> ADCSites { get; set; }

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

    public class ProposalADCDto
    { 
        [Required(ErrorMessage = "The Proposal ID is required")]
        public Guid? ProposalID { get; set; }

        [Required(ErrorMessage = "The ADC ID is required")]
        public Guid? ADCID { get; set; }

        [Required(ErrorMessage = "The User that updates is required")]
        [StringLength(50, ErrorMessage = "The User name must be less than 50 characters")]
        public string UpdatedUser { get; set; }
    }

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

        public decimal? ExchangeRate { get; set; }

        [Required(ErrorMessage = "The tax rate is required")]
        public decimal? TaxRate { get; set; }

        [Required(ErrorMessage = "Indicate whether travel expenses are included")]
        public bool? IncludeTravelExpenses { get; set; }

        [StringLength(1000, ErrorMessage = "The exta info must be less than 1000 characters")]
        public string ExtraInfo { get; set; }

        [Required(ErrorMessage = "The Status value is required")]
        public ProposalStatusType Status { get; set; }

        [Required(ErrorMessage = "The User that updates is required")]
        [StringLength(50, ErrorMessage = "The User name must be less than 50 characters")]
        public string UpdatedUser { get; set; }
    } // ProposalUpdateDto

    public class ProposalWithAuditListDto
    { 
        public ProposalUpdateDto Proposal { get; set; }

        public List<ProposalAuditUpdateDto> ProposalAudits { get; set; }
    } // ProposalWithAuditListDto

    public class ProposalDeleteDto
    {
        [Required(ErrorMessage = "The ID is required to delete")]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The User that deletes is required")]
        [StringLength(50, ErrorMessage = "The User name must be less than 50 characters")]
        public string UpdatedUser { get; set; }
    } // ProposalDeleteDto
}