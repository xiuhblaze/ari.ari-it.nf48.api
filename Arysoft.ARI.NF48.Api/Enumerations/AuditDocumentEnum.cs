namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum AuditDocumentType
    {
        Nothing,
        AuditPlan,
        OaCM,                   // Opening and closing meeting
        AuditReport,
        NCmay,                  // Non conformity major
        NCmin,                  // Non conformity minor
        TechReport,
        FsscIntegrityLetter,    // Solo para FSSC
        FsscAuditPlanSigned,    // Solo para FSSC
        Other
    } // AuditDocumentType
}
      