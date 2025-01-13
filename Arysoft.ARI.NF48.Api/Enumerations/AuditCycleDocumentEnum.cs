namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum AuditCycleDocumentType
    {
        Nothing,
        AppForm,    // Application form
        ADC,        // Audit day calculation
        Proposal,
        Contract,
        AuditProgramme,
        CDC,        // Certification decision checklist
        Certificate,
        Survey,
        Viaticos,
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