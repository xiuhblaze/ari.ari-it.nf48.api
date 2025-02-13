namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum AuditDocumentType
    {
        Nothing,
        AuditPlan,
        OaCM,                   // Opening and closing meeting
        AuditReport,
        ActionPlan,             // Action plan & evidence
        NCCloseReport,          // Non conformities close report
        TechReport,
        CDC,                    // Certification decision checklist - TODO: Ver si se va a quedar aquí
        FsscIntegrityLetter,    // Solo para FSSC
        FsscAuditPlanSigned,    // Solo para FSSC
        FsscScreenShot,         // Solo para FSSC
        Other
    } // AuditDocumentType

    public enum AuditDocumentOrderType
    {
        Nothing,
        DocumentType,
        Standard,
        DocumentTypeDesc,
        StandardDesc
    } // AuditDocumentOrderType
}
      