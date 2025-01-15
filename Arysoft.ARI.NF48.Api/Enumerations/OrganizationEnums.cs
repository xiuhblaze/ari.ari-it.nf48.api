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

    public enum OrganizationCertificatesStatusType
    {
        Nothing,
        Success,
        Warning,
        Danger
    }

    public enum OrganizationOrderType
    { 
        Nothing,
        Name,
        LegalEntity,
        Status,
        CertificatesStatus,
        Updated,
        NameDesc,
        LegalEntityDesc,
        StatusDesc,
        CertificatesStatusDesc,
        UpdatedDesc
    }
}