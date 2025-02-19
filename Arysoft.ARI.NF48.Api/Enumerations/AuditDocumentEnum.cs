namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum AuditDocumentType
    {
        Nothing,
        AuditPlan,
        OaCM,                   // Opening and closing meeting
        AuditReport,
        FsscIntegrityLetter,    // Solo para FSSC
        FsscAuditPlanSigned,    // Solo para FSSC
        ActionPlan,             // Action plan & evidence
        NCCloseReport,          // Non conformities close report
        TechReport,             // No for FSSC
        CDC,                    // Certification decision checklist - TODO: Ver si se va a quedar aquí
        FsscScreenShot,         // Solo para FSSC
        TravelExpenses,         // Viaticos
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
      