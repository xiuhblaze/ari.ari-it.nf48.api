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
        CDC,            // Certification decision checklist - TODO: Ver si se va a quedar aquí
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