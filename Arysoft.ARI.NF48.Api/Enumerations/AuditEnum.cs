namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum AuditStepType
    {
        Nothing,
        Stage1,
        Stage2,             // Inicial
        Surveillance1,
        Surveillance2,
        Recertification,    // Inicial de ciclo
        Transfer,           // Inicial de ciclo de transferencia - se realiza cuando un cliente cambia de certificadora, puede recibir cualquier tipo de documentación
        Special,            // Auditoria especial - puede recibir cualquier tipo de documentación sin orden aparente, funciona para survey 3...
        Surveillance3,      // Solo algunas empresas solicitan auditorias semestrales
        Surveillance4,      // En estos caso es Año 1: rr y S1, año 2: S2 y S3, año 3: S4 y S5
        Surveillance5
    }

    public enum AuditStatusType
    { 
        Nothing,
        Scheduled,  // Agendada - Aun no llega su fecha de ejecución, permite subir documentos
        Confirmed,  // Confirmada - El cliente ya confirmo la fecha y los auditores, estan en linea los documentos requeridos
        InProcess,  // En proceso - La auditoria esta dentro de las fechas de ejecución
        Finished,   // Terminado - Posterior a la fecha de auditoria, es necesario subir la documentación requerida
        Completed,  // Completed - Indica que toda la documentación ha sido cubierta
        Closed,     // Closed - Audioria terminada, ya no se puede actualizar información
        Canceled,   // Cancelada - En cualquier Status la auditoria puede ser cancelada, es necesario indicar la razón
        Deleted     // Eliminada - Registro eliminado logicamente, solo para administradores
    } // AuditStatusType

    public enum AuditOrderType
    {
        Nothing,
        Date,
        Status,
        DateDesc,
        StatusDesc
    } // AuditOrderType

    public enum AuditWarningType
    {
        Nothing,
        // 1. Falta documentacion antes de iniciar la auditoria
        MissingDocuments,
        // 2. Falta documentacion a una semana de la auditoria
        MissingDocumentsWeek,
        // 3. Falta documentación despues de terminar la auditoria
        MissingDocumentsAfter,
        // 4. Falta el plan de acción
        MissingActionPlan,
        // 5. Falta documentación a un mes de terminar la auditoria
        MissingDocumentsAfterMonth,
    } // AuditWarningType

    public enum AuditNextAuditOwnerType
    {
        Nothing,
        Organization, // La organización es la dueña de la siguiente auditoria
        AuditCycle,   // El ciclo de auditoria es el dueño de la siguiente auditoria
        Auditor       // El auditor es el dueño de la siguiente auditoria
    } // AuditNextAuditOwnerType
}