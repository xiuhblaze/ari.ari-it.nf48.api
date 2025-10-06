namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum StatusType
    {
        Nothing,
        Active,
        Inactive,
        Deleted
    } // StatusType

    public enum DefaultValidityStatusType
    {
        Nothing,
        Success,
        Warning,
        Danger
    } // DefaultValidityStatusType

    public enum LanguageType // Probablemente no se necesite xBlaze 20250325
    {
        Nothing,
        Spanish,
        English,
        Other
    }

    public enum BoolType
    { 
        Nothing,
        True,
        False
    }

    public enum CurrencyCodeType
    {
        Nothing,
        USD,
        MXN,
        EUR,
        Other
    }
}