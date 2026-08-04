namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum MD5TableType
    { 
        Nothing,
        QMS,    // Annex A - Quality Management System
        EMS,    // Annex B - Environmental Management System
        OHSMS,  // Annex C - Occupational Health and Safety Management System
        FTE,    // Table B.1 - ISO22003-1:2022(E) Full Time Equivalent
    }

    public enum MD5OrderType
    {
        Nothing,
        StartValue,
        Days,
        TableType,
        StartValueDesc,
        DaysDesc,
        TableTypeDesc,
    }
}