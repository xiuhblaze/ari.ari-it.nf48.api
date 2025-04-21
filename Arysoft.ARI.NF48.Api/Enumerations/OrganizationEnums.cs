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

    // Estatus que deben de llevar las Organizaciones
    // Applicant,
    // Cliente, -> Active

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