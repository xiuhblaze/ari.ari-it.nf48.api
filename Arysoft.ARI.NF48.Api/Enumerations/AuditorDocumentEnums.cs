namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum AuditorDocumentType
    {
        Nothing,
        Exam,
        Other
    }

    public enum AuditorDocumentValidityType
    {
        Nothing,
        Success,    // Toda la documentacion esta en orden
        Warning,    // Al menos un documento esta por vencer
        Danger      // Al menos un documento esta vencido
    }

    public enum AuditorDocumentRequiredType
    {
        Nothing,
        Success,    // Toda la documentación requerida se encuentra
        Danger      // Falta al menos un documento requerido
    }

    public enum AuditorDocumentOrderType
    {
        Nothing,
        StartDate,
        Updated,
        StartDateDesc,
        UpdatedDesc,
    }
}