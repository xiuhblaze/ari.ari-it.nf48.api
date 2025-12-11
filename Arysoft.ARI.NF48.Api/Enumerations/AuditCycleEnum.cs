namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum AuditCycleType    
    {
        Nothing,
        Initial,
        Recertification,
        Transfer
    } // AuditCycleType

    public enum AuditCycleOrderType
    {
        Nothing,
        Date,
        Status,
        DateDesc,
        StatusDesc
    } // AuditCycleOrderType

    public enum AuditCyclePeriodicityType
    {
        Nothing,
        Annual,     // Anual
        Biannual    // Semestral
    } // AuditCyclePeriodicityType
}