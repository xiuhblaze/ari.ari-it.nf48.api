namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum AuditCycleDocumentType
    {
        Nothing,
        AppForm,        // Application form
        ADC,            // Audit day calculation
        Proposal,
        Contract,
        AuditProgramme, // Confirmation letter
        Certificate,
        Survey,
        Other
    } // AuditCycleDocumentType

    public enum AuditCycleDocumentOrderType
    {
        Nothing,
        Version,
        DocumentType,
        Updated,
        VersionDesc,        
        DocumentTypeDesc,
        UpdatedDesc
    } // AuditCycleDocumentOrderType
}