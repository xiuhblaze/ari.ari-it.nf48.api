namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum CalendarBlockType
    {
        Nothing,
        Audit,
        Course,
        Meeting,
        Training,
        Vacation,
        NonWorkingDay,
        Holiday,
        SickLeave,
        PersonalLeave,
        Other
    } // CalendarBlockType

    public enum CalendarBlockOwnerType
    {
        Nothing,
        Auditor,    
        User
    } // CalendarBlockOwnerType
}