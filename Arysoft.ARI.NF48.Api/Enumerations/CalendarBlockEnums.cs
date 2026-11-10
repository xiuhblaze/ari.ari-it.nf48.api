namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum CalendarBlockType
    {
        Nothing,
        Audit,          // Audit
        Course,         // Curso de Improve
        Meeting,        // Reunión de personal
        Training,       // Capacitación o curso
        Vacation,       // Vacaciones de la persona, siempre debe de tener OwnerID
        NonWorkingDay,  // Puente, cierre de empresa, dia inhábil no oficial
        Holiday,        // Festivo oficial
        SickLeave,      // Incapacidad médica
        PersonalLeave,  // Permiso persona, trámites, duelo
        Other           
    } // CalendarBlockType

    public enum CalendarBlockOwnerType
    {
        Nothing,
        Auditor,    
        User
    } // CalendarBlockOwnerType

    public enum CalendarBlockOrderType
    { 
        Nothing,
        Description,
        StartDate,
        BlockType,
        DescriptionDesc,
        StartDateDesc,
        BlockTypeDesc
    } // CalendarBlockOrderType
}