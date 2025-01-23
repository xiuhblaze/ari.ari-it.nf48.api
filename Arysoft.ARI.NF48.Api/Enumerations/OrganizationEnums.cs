namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum OrganizationStatusType
    {
        Nothing,
        New,
        Approved,
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
        LegalEntity,
        Status,
        CertificatesValidityStatus,
        Updated,
        FolioDesc,
        NameDesc,
        LegalEntityDesc,
        StatusDesc,
        CertificatesValidityStatusDesc,
        UpdatedDesc
    }
}