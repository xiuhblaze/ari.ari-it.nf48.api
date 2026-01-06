namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum OrganizationStatusType
    {
        Nothing,
        Applicant,      // La empresa se encuentra en negociación, no es cliente aun (prospecto)
        Active,         // Cliente activo, se pueden generar ciclos y agendar auditorias
        Inactive,       // Cliente inactivo, no se pueden generar ciclos ni agendar auditorias
        Deleted         // Registro eliminado logicamente
    }

    public enum  OrganizationCertificateCycleAlertType
    {
        Nothing,
        Active,             // Filtrar por todas las organizaciones que tienen al menos un ciclo activo
        LastYear,           // Filtrar por todas las organizaciones que tienen al menos un ciclo que le queda un año o menos para expirar
        LeftThreeMonths,    // Filtrar por todas las organizaciones que tienen al menos un ciclo que le quedan tres meses o menos para expirar
        Expired,            // Filtrar por todas las organizaciones que tienen al menos un ciclo expirado
    }

    public enum OrganizationOrderType
    { 
        Nothing,
        Folio,
        Name,
        Status,
        FolderFolio,
        Updated,
        FolioDesc,
        NameDesc,
        StatusDesc,
        FolderFolioDesc,
        UpdatedDesc
    }
}