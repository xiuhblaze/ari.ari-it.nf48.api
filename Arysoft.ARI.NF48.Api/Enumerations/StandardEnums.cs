namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum StandardsOrderType
    {
        Nothing,
        Name,
        Status,
        Update,
        NameDesc,
        StatusDesc,
        UpdateDesc,
    }

    public enum StandardBaseType
    { 
        Nothing,
        ISO9k,
        ISO14K,
        ISO22K,
        ISO27K,
        ISO37K,
        ISO45K,
        FSSC22K,    // Food Safety System Certification 22000
        HACCP,      // Hazard Analysis and Critical Control Points
        GMarkets,   // Global Markets Program
        SQF,        // Safe Quality Food
    }
}