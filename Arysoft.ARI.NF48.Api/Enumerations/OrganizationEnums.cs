namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum OrganizationStatusType
    {
        Nothing,
        Prospect,
        Active,
        Inactive,
        Deleted
    }

    // Estatus que deben de llevar las Organizaciones
    // Prospecto,
    // Cliente, -> Active

    public enum OrganizationOrderType
    { 
        Nothing,
        Folio,
        Name,
        Status,
        CertificatesValidityStatus,
        Updated,
        FolioDesc,
        NameDesc,
        StatusDesc,
        CertificatesValidityStatusDesc,
        UpdatedDesc
    }
}