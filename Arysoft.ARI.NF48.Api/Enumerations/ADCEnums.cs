namespace Arysoft.ARI.NF48.Api.Enumerations
{
    public enum ADCStatusType
    {
        Nothing,    // Nuevo registro - temporal sino se guarda, se elimina
        New,        // Nuevo registro almacenado con la información mínima
        Review,     // Registro enviado a revisión
        Rejected,   // Registro rechazado por el revisor, no se puede modificar
        Active,     // Registro aprobado y activo
        Inactive,   // Registro inactivo, ya no está en uso, no modificable
        Cancel,     // En algún momento el registro fué cancelado
        Deleted     // Eliminación lógica
    }

    public enum ADCOrderType
    {
        Nothing,        // No ordenar
        Description,    // Ordenar por descripción
        Created,        // Ordenar por fecha de creación
        DescriptionDesc,
        CreatedDesc,
    }

    public enum ADCAlertType { 
        Nothing,            // No hay alerta
        EmployeesMistmatch, // Alerta de discrepancia en el número de empleados
        ConceptValueDecreaseExceeded, // Alerta de decremento de valor del concepto excedido
        MD11ReductionExceeded, // Alerta de reducción del MD11 excedido
    }
}