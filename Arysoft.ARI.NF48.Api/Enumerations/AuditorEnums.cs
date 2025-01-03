namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum AuditorDocumentStatusType
    {
        Nothing,
        Success, // Filtra por todos los auditores que tengan bien su documentación
        Warning, // Filtra por los auditores que al menos tienen un documento por vencer
        Danger   // Filtra por los auditores que al menos tienen un documento vencido o les falta un documento requerido
    }

    public enum AuditorLeaderType
    {
        Nothing,
        Leader,
        Regular
    }

    public enum AuditorOrderType
    {
        Nothing,
        FirstName,
        IsLeader,
        Updated,
        FirstNameDesc,
        IsLeaderDesc,
        UpdatedDesc,
    }
}