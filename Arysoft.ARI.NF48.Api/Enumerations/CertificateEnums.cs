namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum CertificateValidityStatusType
    { 
        Nothing,
        Success,
        Warning,
        Danger
    } // CertificateValidityStatusType

    public enum CertificateOrderType
    {
        Nothing,
        Date,
        Status,
        ExpireStatus,
        DateDesc,
        StatusDesc,
        ExpireStatusDesc,
    } // CertificateOrderType

    public enum CertificateFilterDateType
    { 
        Nothing,
        StartDate,
        DueDate,
        PrevAuditDate,
        NextAuditDate
    } // CertificateFilterDateType
}